using WithoutMadiatR.Entities;

namespace WithoutMadiatR.Services.Interfaces;

public interface IBookService
{
    Task<Book> Create(Book book, CancellationToken ct);

    Task<Book> Update(Book book, CancellationToken ct);

    Task<Book?> GetById(Guid id, CancellationToken ct);

    Task<Book?> GetByTitle(string title, CancellationToken ct);

    Task<IEnumerable<Book>> GetAll(CancellationToken ct);
    Task<bool> DeleteById(Guid id, CancellationToken ct);
}