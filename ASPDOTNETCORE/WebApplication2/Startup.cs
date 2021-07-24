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
            // appsetting�� �ִ� ���� ��� �´�
            configuration.Bind("Config", config);
            // config.Flag == false

            // Ư�� �ڸ����� �������־�� �ϴ� ���� �ִٸ�, �Ʒ��� ���� ������� �غ���
            // ex - ȯ�� ���� ��
            const string localSetting = "develop0";
            configuration.Bind($"DeveloperOverrides:{localSetting}", config);
            // config.Flag == true
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // ���� ���Խ�Ű�� �ʾƵ� ������, ViewPage ������ ��Ʈ�ѷ��� �ϴ� ���� ���۷����� ����
            // ���۸��ϱ⵵ ���ϴ�
            services.AddControllersWithViews();

            // .NET FRAMEWORK�� Nancy ���̺귯������ �Ѿ� �԰ų� Ȥ�� url ������ ���� ����ü�� ���� �����ϴ� ���·� ������ٸ� ī�� ����� �̿��ϴ� �͵� ����̴�
            // DefaultJsonModelBinder ��� �ÿ� Body ReadAsync�� ���� ���ϵ��� �ϱ� ������ ���� �ؾ� �Ѵ�!
            // ModelBinderHelper.cs ����
            // services.AddCarter(c =>{
            //  c.WithModelBinder<DefaultJsonModelBinder>();
            //  c.WithResponseNegotiator<NewtonsoftJsonResponseNegotiator>();
            // });

            // Grpc ���
            // �׽�Ʈ ������Ʈ������ ������� �ʴ´�
            //services.AddGrpc(options => {
            //    options.IgnoreUnknownServices = true;
                  // gRPC���� ��� �� ���� �ڵ鸵�� �ϱ� ����
            //    options.Interceptors.Add<GrpcErrorHandleInterceptor>();
            //});
#if DEBUG
            services.AddTransient<ITestRepository, TestRepository_2>();
            services.AddTransient<ITestProvider, TestProvider>();
            // �������̽� ���ʿ��� ��� �Ʒ��� ���� ����ص� �ȴ�
            // Transient�� �ϴ� ���� ���� ����
            // services.AddTransient(typeof(TestRepository));

            // Scope�� ��� ��û 1���� ������ ����Ŭ�� ����
            // ���� ȣ��Ǵ� ��쿡�� �������� �̿��Ͽ� ���� ����� ����� �� �� �ִ�
            // ��) ��쿡 ���� �� ��û ���� ���� ���ļ� 1���� ��û���� ������ ��찡 �ִ�
            // �̶� � ��û���� ���ԵǾ� �ִ��� �� �� �����Ƿ� ������ �������丮�� ȣ���ϰ� �Ǵµ�
            // �̶� ĳ���س��� �� �ִ� �͵��� ������ ���� ĳ���Ͽ� ������ ���� �ϴ�
            // services.AddScoped<ITestRepository, TestRepository_2>();

            // Config�� ���� ��Ÿ������ ���� ��쿡�� �̱������� Ȱ���ص� ����
            // services.AddSingleton<ITestRepository, TestRepository_2>();
#else
            services.AddTransient<ITestRepository, TestRepository>();
#endif

            // Auth ������ �Ʒ��� ���� ������ε� �����ϴ�
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

            // �����Ͻ�, exception handling ��ó�� � Ȱ�� �� �� �ִ�
            // �̵���� ��� �ÿ� ������ ��������
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
            // �� ������ First Call�� ������ ������ �����ϴµ�
            // ���� ����Ʈ�� ��� Url�� �ѹ��� �̸� ȣ���ϴ°�����
            // ServiceCollection�� ���񽺵��� �� ���� ȣ���ϴ� �͸����ε� first call�� �δ��� Ȯ �پ���
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
