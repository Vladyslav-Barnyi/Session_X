using MediatR;
using WithMediator.Entities;
using WithMediator.Repositories.Interfaces;

namespace WithMediator.MediatR;

public record GetBookQuery(string IdOrTitle) : IRequest<Book?>;

public class GetBookHandler : IRequestHandler<GetBookQuery, Book?>
{
    private readonly IBookRepository _bookRepository;

    public GetBookHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<Book?> Handle(GetBookQuery q, CancellationToken ct)
    {
        var result = Guid.TryParse(q.IdOrTitle, out var id)
            ? await _bookRepository.GetByIdAsync(id,ct)
            : await _bookRepository.GetByTitleAsync(q.IdOrTitle,ct);

        return result;
    }
}