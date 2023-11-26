using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Minimal_Api.App.Auth;

public class ApiKeyAuthHandler : AuthenticationHandler<ApiAuthKeySchemeOptions>
{
    public ApiKeyAuthHandler(IOptionsMonitor<ApiAuthKeySchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.ContainsKey(HeaderNames.Authorization) is false)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var header = Request.Headers[HeaderNames.Authorization].ToString();
        if (header != Options.ApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, "me@here.com"),
            new Claim(ClaimTypes.Name, "here.com"),
        };

        var claimsIdentity = new ClaimsIdentity(claims, "ApiKey");

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}