using FluentAssertions;
using Moq;
using WithMediator.Entities;
using WithMediator.MediatR;
using WithMediator.Repositories.Interfaces;

namespace TestWithMediatR
{
    public class GetBookHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly GetBookHandler _sut;

        public GetBookHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _sut = new GetBookHandler(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnBook_WhenBookIsFoundById()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();
            var command = new GetBookQuery(id.ToString());
            var book = new Book { Id = id, Title = "Sample Book", Price = 10.99m, PublicationYear = 2023 };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id, ct)).ReturnsAsync(book);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().NotBeNull("the book should be found by its ID");
            result!.Id.Should().Be(id);
            _bookRepositoryMock.Verify(r => r.GetByIdAsync(id, ct), Times.Once, "GetByIdAsync should be called once.");
        }

        [Fact]
        public async Task Handle_ShouldReturnBook_WhenBookIsFoundByTitle()
        {
            // Arrange
            var ct = new CancellationToken();
            var command = new GetBookQuery("Sample Book");
            var book = new Book { Id = Guid.NewGuid(), Title = "Sample Book", Price = 10.99m, PublicationYear = 2023 };

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(command.IdOrTitle, ct)).ReturnsAsync(book);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().NotBeNull("the book should be found by its title");
            result!.Title.Should().Be("Sample Book");
            _bookRepositoryMock.Verify(r => r.GetByTitleAsync(command.IdOrTitle, ct), Times.Once, "GetByTitleAsync should be called once.");
        }

        [Fact]
        public async Task Handle_ShouldReturnNull_WhenBookIsNotFound()
        {
            // Arrange
            var ct = new CancellationToken();
            var command = new GetBookQuery("Nonexistent Book");

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(command.IdOrTitle, ct)).ReturnsAsync((Book?)null);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().BeNull("no book should be found with the given title");
        }
    }
}
