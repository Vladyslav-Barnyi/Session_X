using Microsoft.EntityFrameworkCore;
using WithoutMadiatR.Db;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Repositories.Interfaces;

namespace WithoutMadiatR.Repositories;

public class BookRepository : IBookRepository
{
    private readonly BookContext _dbContext;

    public BookRepository(BookContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Book>> GetAll(CancellationToken ct, int? page, int? pageSize)
    {
        var books = await _dbContext.Books.ToListAsync(ct);
        return books;
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<Book?> GetByTitleAsync(string title, CancellationToken ct)
    {
        return await _dbContext.Books.Where(b => b.Title == title).FirstOrDefaultAsync(ct);
    }

    public async Task<Book> CreateAsync(Book book, CancellationToken ct)
    {
        var result = await _dbContext.AddAsync(book, ct);
        await _dbContext.SaveChangesAsync(ct);
        return result.Entity;
    }

    public async Task UpdateAsync(Book book, CancellationToken ct)
    {
        _dbContext.Update(book);
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveAsync(Guid id, CancellationToken ct)
    {
        var book = await _dbContext.Books.Where(b => b.Id == id).FirstOrDefaultAsync(ct);

        if (book == null)
            return true;

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync(ct);
        return true;
    }
}