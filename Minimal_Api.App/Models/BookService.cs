using Dapper;
using Minimal_Api.App.Data;
using Minimal_Api.App.Services;

namespace Minimal_Api.App.Models;

public class BookService : IBookService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public BookService(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> CreateAsync(Book book)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        var bookExists = await GetByIsbnAsync(book.Isbn);
        if (bookExists is not null)
        {
            return false;
        }

        const string sql =
            "INSERT INTO books (title, author, isbn, short_description, page_count, release_date) VALUES (@Title, @Author, @Isbn, @ShortDescription, @PageCount, @ReleaseDate)";
        var result = await connection.ExecuteAsync(sql, book);

        return result > 0;
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = """
                               SELECT
                                   isbn AS Isbn,
                                   title AS Title,
                                   author AS Author,
                                   short_description AS ShortDescription,
                                   page_count AS PageCount,
                                   release_date AS ReleaseDate
                               FROM books
                               WHERE isbn = @Isbn LIMIT 1
                           """;
        var result =
            await connection.QuerySingleOrDefaultAsync<Book>(
                sql, new { Isbn = isbn })!;

        return result;
    }

    public async Task<IEnumerable<Book>> GetAllAsync()
    {
        var connection = await _connectionFactory.CreateConnectionAsync();
        var result =
            await connection.QueryAsync<Book>(
                """
                    SELECT
                        isbn AS Isbn,
                        title AS Title,
                        author AS Author,
                        short_description AS ShortDescription,
                        page_count AS PageCount,
                        release_date AS ReleaseDate
                    FROM books
                """);

        return result;
    }

    public async Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        const string sql = """
                           SELECT
                               isbn AS Isbn,
                               title AS Title,
                               author AS Author,
                               short_description AS ShortDescription,
                               page_count AS PageCount,
                               release_date AS ReleaseDate
                           FROM books  
                           WHERE title LIKE '%' || @SearchTerm  || '%'
                           """;
        var result =
            await connection.QueryAsync<Book>(
                sql, new { SearchTerm = searchTerm })!;

        return result;
    }

    public async Task<bool> UpdateAsync(Book book)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        var bookExists = await GetByIsbnAsync(book.Isbn);
        if (bookExists is null)
        {
            return false;
        }

        const string sql =
            "UPDATE books SET title = @Title, author = @Author, short_description = @ShortDescription, page_count = @PageCount, release_date = @ReleaseDate WHERE isbn = @Isbn";
        var result =
            await connection.ExecuteAsync(
                sql, book);

        return result > 0;
    }

    public async Task<bool> DeleteAsync(string isbn)
    {
        var connection = await _connectionFactory.CreateConnectionAsync();

        var result = await connection.ExecuteAsync("DELETE FROM books WHERE isbn = @Isbn", new { Isbn = isbn });

        return result > 0;
    }
}