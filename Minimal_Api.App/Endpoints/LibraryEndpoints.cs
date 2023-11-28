using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minimal_Api.App.Auth;
using Minimal_Api.App.Endpoints.Internal;
using Minimal_Api.App.Models;
using Minimal_Api.App.Services;

namespace Minimal_Api.App.Endpoints;

public class LibraryEndpoints : IEndpoints
{
    public static void DefineEndpoints(IEndpointRouteBuilder app)
    {

        app.MapPost("books", LibraryEndpointsDelegates.CreateBookAsync)
            .WithName("CreateBook")
            .Accepts<Book>("application/json")
            .Produces<Book>(201)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags("Books");

        app.MapGet("books", async (IBookService ds, string? searchTerm) =>
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                var results = await ds.GetAllAsync();
                return Results.Ok(results);
            }

            var foundBooks = await ds.SearchByTitleAsync(searchTerm);
            return Results.Ok(foundBooks);
        })
            .WithName("GetBooks")
            .Produces<IEnumerable<Book>>(200)
            .WithTags("Books");

        app.MapGet("books/{isbn}", async (IBookService ds, string isbn) =>
        {
            var result = await ds.GetByIsbnAsync(isbn);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
            .WithName("GetBook")
            .Produces<Book>(200)
            .Produces(404)
            .WithTags("Books");

        app.MapPut("/books/{isbn}", async (IBookService ds,IValidator<Book> validator, string isbn, Book bookModel) =>
        {
            bookModel.Isbn = isbn;
            var validationResult = await validator.ValidateAsync(bookModel);

            if (validationResult.IsValid is false)
            {
                return Results.BadRequest(validationResult.Errors);
            }
            var result = await ds.UpdateAsync(bookModel);

            return result? Results.Ok(bookModel): Results.NotFound();
        })
            .WithTags("Books");
        
        app.MapMethods("books", new []{"PATCH"}, LibraryEndpointsDelegates.PartialUpdateBookAsync)
            .AddEndpointFilter<BookPatchIsValidFilter>()
            .WithName("PatchBook")
            .Produces<Book>(200)
            .Produces<IEnumerable<ValidationFailure>>(400)
            .WithTags("Books");

        app.MapDelete(@"/books/{isbn}", 
            [Authorize(AuthenticationSchemes = ApiKeySchemeConstants.SchemeName)]
            async (IBookService ds, string isbn) =>
        {
            var deleted = await ds.DeleteAsync(isbn);
            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }

    public static void AddEndpointServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IBookService, BookService>();
    }
}

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