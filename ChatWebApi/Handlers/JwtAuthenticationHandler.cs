using System;
using System.Text;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ChatWebApi.Helpers;
using ChatWebApi.Services;
using Newtonsoft.Json;

namespace ChatWebApi.Handlers
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private readonly AppSettings _appSettings;
        private readonly IUserService _userService;
 
        public JwtAuthenticationHandler(
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptions<AppSettings> appSettings,
            IUserService userService
            )
            : base(options, logger, encoder, clock)
        {
            _appSettings = appSettings.Value;
            _userService = userService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            return await Task.Run(() => {
                if (Request.Query.TryGetValue("access_token", out var accessToken))
                {
                    Request.Headers.Add("Authorization", $"Bearer {accessToken}");
                }

                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (String.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                SecurityToken validatedToken = null;
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out validatedToken);
                }
                catch { };

                if (validatedToken == null)
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }

                // attach user to context on successful jwt validation
                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Convert.ToInt32(jwtToken.Claims.First(x => x.Type == "id").Value);
                var identity = new ClaimsIdentity("JwtAuthentication");
                identity.AddClaim(new Claim("User", JsonConvert.SerializeObject(_userService.GetById(userId))));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
                return AuthenticateResult.Success(ticket);
            });
        }

    }

    public class JwtAuthenticationOptions : AuthenticationSchemeOptions
    {
    }
}