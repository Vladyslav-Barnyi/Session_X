using FluentValidation;
using WithMediator.Entities;
using WithMediator.Extensions;
using WithMediator.Repositories.Interfaces;
using MediatR;


namespace WithMediator.MediatR;

public record CreateBookCommand(string Title, decimal Price, int PublicationYear)
    : IRequest<Book>;

public class CreateBookHandler : IRequestHandler<CreateBookCommand, Book>
{
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<Book> _bookValidator;

    public CreateBookHandler(IBookRepository bookRepository, IValidator<Book> bookValidator)
    {
        _bookRepository = bookRepository;
        _bookValidator = bookValidator;
    }

    public async Task<Book> Handle(
        CreateBookCommand r, CancellationToken ct)
    {
        var book = r.ToBook();

        var validationResult = await _bookValidator.ValidateAsync(book, ct);
        if (!validationResult.IsValid)
            throw new Exception($"{validationResult.Errors}");

        await _bookRepository.CreateAsync(book,ct);
        return book;
    }
}