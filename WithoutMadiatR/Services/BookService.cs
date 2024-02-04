using FluentValidation;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Repositories.Interfaces;
using WithoutMadiatR.Services.Interfaces;

namespace WithoutMadiatR.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<Book> _bookValidator;

    public BookService(IBookRepository bookRepository, IValidator<Book> bookValidator)
    {
        _bookRepository = bookRepository;
        _bookValidator = bookValidator;
    }

    public async Task<Book> Create(Book book, CancellationToken ct)
    {
        var validationResult = await _bookValidator.ValidateAsync(book, ct);
        if (!validationResult.IsValid)
        {
            throw new Exception($"{validationResult.Errors}");
        }

        await _bookRepository.CreateAsync(book, ct);
        return book;
    }

    public async Task<Book> Update(Book book, CancellationToken ct)
    {
        var validationResult = await _bookValidator.ValidateAsync(book, ct);
        if (!validationResult.IsValid)
            throw new Exception($"{validationResult.Errors}");


        var existingBook = await _bookRepository.GetByIdAsync(book.Id, ct);
        if (existingBook is null)
            throw new Exception();


        await _bookRepository.UpdateAsync(book, ct);
        return book;
    }

    public async Task<Book?> GetById(Guid id, CancellationToken ct)
    {
        return await _bookRepository.GetByIdAsync(id,ct);
    }

    public async Task<Book?> GetByTitle(string title, CancellationToken ct)
    {
        return await _bookRepository.GetByTitleAsync(title,ct);
    }

    public async Task<IEnumerable<Book>> GetAll(CancellationToken ct)
    {
        return await _bookRepository.GetAll(ct);
    }
    
    public Task<bool> DeleteById(Guid id, CancellationToken ct)
    {
        return _bookRepository.RemoveAsync(id, ct);
    }
}
