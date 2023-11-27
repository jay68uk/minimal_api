namespace Minimal_Api.App.Models.Extensions;

public static class BookExtensions
{
        public static void ApplyPatch(this Book book, BookPatch patch)
        {
            if (patch.Title != null)
                book.Title = patch.Title;
            if (patch.Author != null)
                book.Author = patch.Author;
            if (patch.ShortDescription != null)
                book.ShortDescription = patch.ShortDescription;
            if (patch.PageCount.HasValue)
                book.PageCount = patch.PageCount.Value;
            if (patch.ReleaseDate.HasValue)
                book.ReleaseDate = patch.ReleaseDate.Value;
        }
}