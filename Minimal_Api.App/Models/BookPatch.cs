namespace Minimal_Api.App.Models;

public record BookPatch(string Isbn,string? Title, string? Author, string? ShortDescription, int? PageCount, DateTime? ReleaseDate);