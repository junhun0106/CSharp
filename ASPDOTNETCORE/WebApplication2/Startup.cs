using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Middleware;
using WebService;

namespace WebApplication2
{
    public class Startup
    {
        IServiceCollection _services;

        public Startup(IConfiguration configuration)
        {
            var config = new Config();
            // appsetting에 있는 값을 얻어 온다
            configuration.Bind("Config", config);
            // config.Flag == false

            // 특정 자리마다 변경해주어야 하는 값이 있다면, 아래와 같은 방법으로 해보자
            // ex - 환경 변수 등
            const string localSetting = "develop0";
            configuration.Bind($"DeveloperOverrides:{localSetting}", config);
            // config.Flag == true
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // 굳이 포함시키지 않아도 되지만, ViewPage 구성은 컨트롤러로 하는 것이 레퍼런스도 많고
            // 구글링하기도 편하다
            services.AddControllersWithViews();

            // .NET FRAMEWORK나 Nancy 라이브러리에서 넘어 왔거나 혹은 url 관리를 위해 구조체를 서로 공유하는 형태로 만들었다면 카터 모듈을 이용하는 것도 방법이다
            // DefaultJsonModelBinder 사용 시에 Body ReadAsync을 하지 못하도록 하기 때문에 주의 해야 한다!
            // ModelBinderHelper.cs 참고
            // services.AddCarter(c =>{
            //  c.WithModelBinder<DefaultJsonModelBinder>();
            //  c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
            // });

            // Grpc 사용
            // 테스트 프로젝트에서는 사용하지 않는다
            //services.AddGrpc(options => {
            //    options.IgnoreUnknownServices = true;
                  // gRPC에서 사용 시 에러 핸들링을 하기 위함
            //    options.Interceptors.Add<GrpcErrorHandleInterceptor>();
            //});
#if DEBUG
            services.AddTransient<ITestRepository, TestRepository_2>();
            services.AddTransient<ITestProvider, TestProvider>();
            // 인터페이스 불필요할 경우 아래와 같이 사용해도 된다
            // Transient로 하는 것이 권장 사항
            // services.AddTransient(typeof(TestRepository));

            // Scope에 경우 요청 1번과 라이프 사이클이 같다
            // 자주 호출되는 경우에는 스코프를 이용하여 성능 향상을 기대해 볼 수 있다
            // 예) 경우에 따라 웹 요청 여러 개를 뭉쳐서 1번의 요청으로 보내는 경우가 있다
            // 이때 어떤 요청들이 포함되어 있는지 알 수 없으므로 각각의 레포지토리를 호출하게 되는데
            // 이때 캐시해놓을 수 있는 것들은 스코프 내에 캐시하여 재사용이 가능 하다
            // services.AddScoped<ITestRepository, TestRepository_2>();

            // Config나 게임 메타데이터 같은 경우에는 싱글톤으로 활용해도 좋다
            // services.AddSingleton<ITestRepository, TestRepository_2>();
#else
            services.AddTransient<ITestRepository, TestRepository>();
#endif

            // Auth 검증은 아래와 같은 방식으로도 가능하다
            services.AddAuthentication(TokenAuthenticationOptions.Scheme).AddScheme<TokenAuthenticationOptions, TokenAuthenticationHandler>(TokenAuthenticationOptions.Scheme, _ => { });
            
            services.AddTransient<TestActionFilter>();
            
            services.AddAuthorization();

            _services = services;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment()) {
            //    app.UseDeveloperExceptionPage();
            //}

            // 레이턴시, exception handling 후처리 등에 활용 할 수 있다
            // 미들웨어 사용 시에 순서에 주의하자
            app.UseMiddleware<TestMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                //endpoints.MapCarter();
                //endpoints.MapGrpcService<PersistenterService>();
            });

            WarmupServices(app.ApplicationServices, _services);
        }

        private void WarmupServices(IServiceProvider container, IServiceCollection services)
        {
            // 웹 서버는 First Call이 굉장히 느리게 동작하는데
            // 가장 베스트는 모든 Url에 한번씩 미리 호출하는거지만
            // ServiceCollection에 서비스들을 한 번씩 호출하는 것만으로도 first call에 부담이 확 줄어든다
            Task.Run(() => {
                var sw = Stopwatch.StartNew();
                using var scope = container.CreateScope();
                foreach (var service in GetServices(services)) {
                    if (service.IsAbstract) continue;
                    scope.ServiceProvider.GetService(service);
                }
                sw.Stop();
            });

            IEnumerable<Type> GetServices(IServiceCollection services)
            {
                return services
                            .Where(descriptor => !descriptor.ServiceType.ContainsGenericParameters)
                            .Select(descriptor => descriptor.ServiceType)
                            .Distinct();
            }
        }
    }
}
