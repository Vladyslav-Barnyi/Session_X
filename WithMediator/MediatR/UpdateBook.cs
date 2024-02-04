using FluentValidation;
using MediatR;
using WithMediator.Entities;
using WithMediator.Extensions;
using WithMediator.Repositories.Interfaces;

namespace WithMediator.MediatR;

public record UpdateBookCommand(Guid Id, string Title, decimal Price, int PublicationYear) : IRequest<Guid>;

public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly IValidator<Book> _bookValidator;

    public UpdateBookHandler(IBookRepository bookRepository, IValidator<Book> bookValidator)
    {
        _bookRepository = bookRepository;
        _bookValidator = bookValidator;
    }

    public async Task<Guid> Handle(UpdateBookCommand r, CancellationToken ct)
    {
        var book = await _bookRepository.GetByIdAsync(r.Id,ct);
        if (book is null)
            throw new Exception();
        
        book.Adapt(r);

        var validationResult = await _bookValidator.ValidateAsync(book, ct);
        if (!validationResult.IsValid)
            throw new Exception($"{validationResult.Errors}");
        
        await _bookRepository.UpdateAsync(book,ct);
        return book.Id;
    }
}