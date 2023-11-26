using Minimal_Api.App.Models;

namespace Minimal_Api.App.Services;

public interface IBookService
{
    public Task<bool> CreateAsync(Book book);

    public Task<Book?> GetByIsbnAsync(string isbn);

    public Task<IEnumerable<Book>> GetAllAsync();

    public Task<IEnumerable<Book>> SearchByTitleAsync(string searchTerm);

    public Task<bool> UpdateAsync(Book book);

    public Task<bool> DeleteAsync(string isbn);
}