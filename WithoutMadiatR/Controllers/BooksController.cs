using Microsoft.AspNetCore.Mvc;
using WithoutMadiatR.Contracts.Reposnes;
using WithoutMadiatR.Contracts.Requests;
using WithoutMadiatR.Extensions;
using WithoutMadiatR.Services.Interfaces;

namespace WithoutMadiatR.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GetBookResponse>>> GetAllBooks(CancellationToken ct)
    {
        var books = await _bookService.GetAll(ct);
        var response = books.Select(b=>b.ToGetBookResponse());
        return Ok(response);
    }

    [HttpGet("{idOrTitle}")]
    public async Task<ActionResult<GetBookResponse>> Get([FromRoute] string idOrTitle,
        CancellationToken ct)
    {
        var result = Guid.TryParse(idOrTitle, out var id)
            ? await _bookService.GetById(id, ct)
            : await _bookService.GetByTitle(idOrTitle, ct);

        return result is not null ? Ok(result.ToGetBookResponse()) : NotFound();
    }

    [HttpPost]
    public async Task<EntityResponse> CreateBook([FromBody] CreateBookRequest r,
        CancellationToken ct)
    {
        var book = r.ToBook();
        var result = await _bookService.Create(book, ct);

        return new EntityResponse { Id = result.Id };
    }


    [HttpPut("{id}")]
    public async Task<ActionResult<EntityResponse>> UpdateBook([FromRoute] Guid id,
        [FromBody] UpdateBookRequest r, CancellationToken ct)
    {
        var book = await _bookService.GetById(id, ct);
        if (book is null)
            return NotFound();

        book.Adapt(r);
        var result = await _bookService.Update(book, ct);

        return new EntityResponse { Id = result.Id };
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook([FromRoute] Guid id,
        CancellationToken ct)
    {
        var deleted = await _bookService.DeleteById(id, ct);
        return deleted ? Ok() : NotFound();
    }
}