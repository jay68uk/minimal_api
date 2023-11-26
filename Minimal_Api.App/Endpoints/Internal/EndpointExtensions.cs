using System.Reflection;

namespace Minimal_Api.App.Endpoints.Internal;

public static class EndpointExtensions
{
    public static void AddEndpoints<TMarker>(this IServiceCollection services, IConfiguration configuration)
    {
        AddEndpoints(services, configuration,typeof(TMarker));
    }

    private static void AddEndpoints(this IServiceCollection services, IConfiguration configuration, Type typeMarker)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.AddEndpointServices))!
                .Invoke(null, new object?[] { services, configuration });
        }
    }
    
    public static void UseEndpoints<TMarker>(this IApplicationBuilder app)
    {
        UseEndpoints(app, typeof(TMarker));
    }
    
    public static void UseEndpoints(this IApplicationBuilder app, Type typeMarker)
    {
        var endpointTypes = GetEndpointTypesFromAssemblyContaining(typeMarker);

        foreach (var endpointType in endpointTypes)
        {
            endpointType.GetMethod(nameof(IEndpoints.DefineEndpoints))!
                .Invoke(null, new object?[] { app });
        }
    }
    
    private static IEnumerable<TypeInfo> GetEndpointTypesFromAssemblyContaining(Type typeMarker)
    {
        var endpointTypes = typeMarker.Assembly.DefinedTypes
            .Where(x => x.IsAbstract is false
                        && x.IsInterface is false
                        && typeof(IEndpoints).IsAssignableFrom(x));
        return endpointTypes;
    }

}