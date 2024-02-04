namespace WithoutMadiatR.Contracts.Reposnes;

public class GetBookResponse
{
    public required Guid Id{ get; set; }
    public required string Title { get; set; }
    public required decimal Price { get; set; }
    public required int PubYeat { get; set; }
}