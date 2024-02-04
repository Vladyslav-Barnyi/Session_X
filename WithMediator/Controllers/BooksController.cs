using MediatR;
using Microsoft.AspNetCore.Mvc;
using WithMediator.Contracts.Reposnes;
using WithMediator.Contracts.Requests;
using WithMediator.Extensions;
using WithMediator.MediatR;

namespace WithMediator.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediatR;

    public BooksController(IMediator mediatR)
    {
        _mediatR = mediatR;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetBookResponse>>> GetAllBooks(CancellationToken ct)
    {
        var response = await _mediatR.Send(new GetAllBooksQuery(),ct);
        return Ok(response);
    }

    [HttpGet("{idOrTitle}")]
    public async Task<ActionResult<GetBookResponse>> Get([FromRoute] string idOrTitle,
        CancellationToken ct)
    {
        var result = await _mediatR.Send(new GetBookQuery(idOrTitle), ct);
        
        return result is not null ? Ok(result.ToGetBookResponse()) : NotFound();
    }

    [HttpPost]
    public async Task<EntityResponse> CreateBook([FromBody] CreateBookRequest r,
        CancellationToken ct)
    {
        var result = await _mediatR.Send(new CreateBookCommand(r.Title, r.Price, r.PublicationYear), ct);

        return new EntityResponse { Id = result.Id };
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<EntityResponse>> UpdateBook([FromRoute] Guid id,
        [FromBody] UpdateBookRequest r, CancellationToken ct)
    {
        var result = await _mediatR
            .Send(new UpdateBookCommand(id, r.Title, r.Price, r.PublicationYear.Year),ct);

        return new EntityResponse { Id = result };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id,
        CancellationToken ct)
    {
        var deleted = await _mediatR.Send(new DeleteBookCommand(id), ct);
        return deleted ? Ok() : NotFound();
    }
}