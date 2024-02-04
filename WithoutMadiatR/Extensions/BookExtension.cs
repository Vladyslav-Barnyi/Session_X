using WithoutMadiatR.Contracts.Reposnes;
using WithoutMadiatR.Entities;

namespace WithoutMadiatR.Extensions;

public static class BookExtension
{
    public static GetBookResponse ToGetBookResponse(this Book book)
    {
        var response = new GetBookResponse
        {
            Id = book.Id,
            Price = book.Price,
            Title = book.Title,
            PubYeat = book.PublicationYear
        };
        return response;
    }
}