using Microsoft.AspNetCore.Authentication;

namespace Minimal_Api.App.Auth;

public class ApiAuthKeySchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = "SecretKey";

}