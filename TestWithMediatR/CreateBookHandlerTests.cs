using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using WithMediator.Entities;
using WithMediator.MediatR;
using WithMediator.Repositories.Interfaces;

namespace TestWithMediatR
{
    public class CreateBookHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IValidator<Book>> _bookValidatorMock;
        private readonly CreateBookHandler _sut;

        public CreateBookHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookValidatorMock = new Mock<IValidator<Book>>();
            _sut = new CreateBookHandler(_bookRepositoryMock.Object, _bookValidatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateBook_WhenValidationSucceeds()
        {
            // Arrange
            var ct = new CancellationToken();
            var command = new CreateBookCommand("Test Book", 19.99m, 2023);

            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = command.Title,
                Price = command.Price,
                PublicationYear = command.PublicationYear
            };

            // Setup the ToBook() extension method simulation
            _bookValidatorMock.Setup(v => v.ValidateAsync(It.IsAny<Book>(), ct))
                .ReturnsAsync(new ValidationResult());

            // Simulate the repository's CreateAsync method returning the created book.
            _bookRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Book>(), ct))
                .ReturnsAsync(book) // Correctly returns Task<Book>.
                .Callback<Book, CancellationToken>((b, _) =>
                {
                    b.Id = book.Id; // Simulate setting the Id on book creation.
                });

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().NotBeNull("Handler should return a book object.");
            result.Id.Should().Be(book.Id, "The returned book's Id should match the created book's Id.");
            result.Title.Should().Be(command.Title, "The title should match the command's title.");
            result.Price.Should().Be(command.Price, "The price should match the command's price.");
            result.PublicationYear.Should().Be(command.PublicationYear, "The publication year should match the command's year.");

            _bookRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Book>(), ct), Times.Once, "CreateAsync should be called once.");
        }
    }
}
