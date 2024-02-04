namespace WithMediator.Contracts.Requests;

public class UpdateBookRequest
{
    public required string Title { get; set; }
    public required int Price { get; set; }
    public required DateTime PublicationYear { get; set; }
}