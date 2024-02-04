using WithMediator.Entities;

namespace WithMediator.Repositories.Interfaces;

public interface IBookRepository
{
    Task<List<Book>> GetAll(CancellationToken ct, int? page = 1, int? pageSize = 10);

    Task<Book?> GetByIdAsync(Guid id, 
        CancellationToken ct);

    Task<Book?> GetByTitleAsync(string title , 
        CancellationToken ct);

    Task<Book> CreateAsync(Book book, 
        CancellationToken ct);

    Task UpdateAsync(Book book, 
        CancellationToken ct);

    Task<bool> RemoveAsync(Guid id, 
        CancellationToken ct);
}