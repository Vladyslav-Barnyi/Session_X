using WithMediator.Contracts.Requests;
using WithMediator.Entities;
using WithMediator.MediatR;

namespace WithMediator.Extensions;

public static class RequestExtensions
{
    public static Book ToBook(this CreateBookRequest r)
    {
        var response = new Book
        {
            Id = Guid.NewGuid(),
            Title = r.Title,
            Price = r.Price,
            PublicationYear = r.PublicationYear
        };
        return response;
    }
    
    public static Book ToBook(this CreateBookCommand r)
    {
        var response = new Book
        {
            Id = Guid.NewGuid(),
            Title = r.Title,
            Price = r.Price,
            PublicationYear = r.PublicationYear
        };
        return response;
    }

    public static void Adapt(this Book book, UpdateBookRequest r)
    {
        book.Title = r.Title;
        book.Price = r.Price;
        book.PublicationYear = r.PublicationYear.Year;
    }
    
    public static void Adapt(this Book book, UpdateBookCommand r)
    {
        book.Title = r.Title;
        book.Price = r.Price;
        book.PublicationYear = r.PublicationYear;
    }
}