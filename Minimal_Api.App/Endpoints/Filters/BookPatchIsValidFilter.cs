using FluentValidation;
using Minimal_Api.App.Models;

namespace Minimal_Api.App.Endpoints.Filters;

public class BookPatchIsValidFilter : IEndpointFilter
{
    private readonly IValidator<BookPatch> _validator;

    public BookPatchIsValidFilter(IValidator<BookPatch> validator)
    {
        _validator = validator;
    }
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validationResult = await _validator.ValidateAsync(context.GetArgument<BookPatch>(0));

        if (validationResult.IsValid is false)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        return await next(context);
    }
}