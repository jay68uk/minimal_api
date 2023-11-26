namespace Minimal_Api.App.Endpoints.Internal;

public interface IEndpoints
{
    public static abstract void DefineEndpoints(IEndpointRouteBuilder app);

    public static abstract void AddEndpointServices(IServiceCollection services, IConfiguration configuration);
}