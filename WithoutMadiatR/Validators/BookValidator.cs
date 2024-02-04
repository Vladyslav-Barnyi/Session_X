using FluentValidation;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Repositories.Interfaces;

namespace WithoutMadiatR.Validators;

public class BookValidator : AbstractValidator<Book>
{
    public IBookRepository BookRepository { get; }

    public BookValidator(IBookRepository bookRepository)
    {
        BookRepository = bookRepository;
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.PublicationYear).LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(x => x.Title).NotEmpty();

    }
}