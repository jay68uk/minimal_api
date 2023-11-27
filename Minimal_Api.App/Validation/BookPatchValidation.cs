using FluentValidation;
using Minimal_Api.App.Models;

namespace Minimal_Api.App.Validation;

public class BookPatchValidation : AbstractValidator<BookPatch>
{
    public BookPatchValidation()
    {
        RuleFor(book => book.Isbn).Matches(@"^(97[89])(-\d{1,5})?(-\d{1,7})?(-\d{1,6})?(-\d{1})$")
            .WithMessage("Values is not a valid ISBN-13");

        RuleFor(book => book.PageCount).GreaterThan(0).When(book=>book.PageCount is not null);
    }
}