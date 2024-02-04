using System.ComponentModel.DataAnnotations;

namespace WithoutMadiatR.Contracts.Requests;

public class CreateBookRequest
{
    [Required] public string Title { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public int PubYear { get; set; }

}