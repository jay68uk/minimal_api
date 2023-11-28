using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Minimal_Api.App.Models;
using Minimal_Api.App.Services;

namespace Minimal_Api.App.Endpoints.Internal;

internal static class LibraryEndpointsDelegates
{
    internal static async Task<IResult> CreateBookAsync(IValidator<Book> validator, Book bookModel, IBookService ds)
    {
        var validationResult = await validator.ValidateAsync(bookModel);

        if (validationResult.IsValid is false)
        {
            return Results.BadRequest(validationResult.Errors);
        }

        var result = await ds.CreateAsync(bookModel);

        if (result is false)
        {
            return Results.BadRequest(new List<ValidationFailure>
            {
                new("Isbn", "A book with the entered ISBN-13 already exists!")
            });
        }

        return Results.CreatedAtRoute("GetBook", new { isbn = bookModel.Isbn }, bookModel);
    }

    internal static async Task<IResult> PartialUpdateBookAsync([AsParameters] BookPatch bookModel, IBookService ds)
    {
        // var validationResult = await validator.ValidateAsync(bookModel);
        //
        // if (validationResult.IsValid is false)
        // {
        //     return Results.BadRequest(validationResult.Errors);
        // }
       
        var bookUpdate = await ds.PartialUpdateAsync(bookModel);

        return bookUpdate is null?  Results.NotFound() : Results.Ok(bookUpdate);

    }
}