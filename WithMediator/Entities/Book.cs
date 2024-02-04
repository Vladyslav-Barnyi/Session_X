namespace WithMediator.Entities;

public class Book
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required int PublicationYear { get; set; }
}