using FluentAssertions;
using Moq;
using WithMediator.MediatR;
using WithMediator.Repositories.Interfaces;

namespace TestWithMediatR
{
    public class DeleteBookHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly DeleteBookHandler _sut;

        public DeleteBookHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _sut = new DeleteBookHandler(_bookRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTrue_WhenBookIsDeleted()
        {
            // Arrange
            var ct = new CancellationToken();
            var command = new DeleteBookCommand(Guid.NewGuid());

            _bookRepositoryMock.Setup(r => r.RemoveAsync(command.Id, ct)).ReturnsAsync(true);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().BeTrue("the book should be successfully deleted");
            _bookRepositoryMock.Verify(r => r.RemoveAsync(command.Id, ct), Times.Once, "RemoveAsync should be called once.");
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenBookCannotBeDeleted()
        {
            // Arrange
            var ct = new CancellationToken();
            var command = new DeleteBookCommand(Guid.NewGuid());

            _bookRepositoryMock.Setup(r => r.RemoveAsync(command.Id, ct)).ReturnsAsync(false);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().BeFalse("the book was not found or could not be deleted");
            _bookRepositoryMock.Verify(r => r.RemoveAsync(command.Id, ct), Times.Once, "RemoveAsync should be called once.");
        }
    }
}