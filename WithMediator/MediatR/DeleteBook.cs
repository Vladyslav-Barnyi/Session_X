using MediatR;
using WithMediator.Repositories.Interfaces;

namespace WithMediator.MediatR;

public record DeleteBookCommand(Guid Id) : IRequest<bool>;

public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, bool>
{
    private readonly IBookRepository _bookRepository;

    public DeleteBookHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<bool> Handle(DeleteBookCommand request, CancellationToken ct)
    {
        return await _bookRepository.RemoveAsync(request.Id,ct);
    }
}
