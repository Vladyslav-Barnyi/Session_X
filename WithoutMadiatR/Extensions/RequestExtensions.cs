using WithoutMadiatR.Contracts.Requests;
using WithoutMadiatR.Entities;

namespace WithoutMadiatR.Extensions;

public static class RequestExtensions
{
    public static Book ToBook(this CreateBookRequest r)
    {
        var response = new Book
        {
            Id = Guid.NewGuid(),
            Title = r.Title,
            Price = r.Price,
            PublicationYear = r.PubYear
        };
        return response;
    }

    public static void Adapt(this Book book, UpdateBookRequest r)
    {
        book.Title = r.Title;
        book.Price = r.Price;
        book.PublicationYear = r.PublicationYear.Year;
    }
}