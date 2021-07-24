using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace WebApplication2
{
    public class TokenAuthenticationOptions : AuthenticationSchemeOptions
    {
        public TokenAuthenticationOptions()
        {
            this.Claims = new List<Claim>();
        }

        public const string Scheme = "Bearer";

        public virtual ClaimsIdentity Identity { get; set; }

        public IEnumerable<Claim> Claims { get; set; }
    }

    public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
    {
        public TokenAuthenticationHandler(IOptionsMonitor<TokenAuthenticationOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string accessToken = Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(accessToken)) {
                // 웹 요청이 Module로 들어가기 전에 세션을 검증 해볼 수 있다
                // return AuthenticateResult.Fail("time expired");
            }
            return AuthenticateResult.NoResult();
        }
    }
}
