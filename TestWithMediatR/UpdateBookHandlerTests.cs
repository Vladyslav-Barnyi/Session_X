using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using WithMediator.Entities;
using WithMediator.MediatR;
using WithMediator.Repositories.Interfaces;

namespace TestWithMediatR;

    public class UpdateBookHandlerTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IValidator<Book>> _bookValidatorMock;
        private readonly UpdateBookHandler _sut;

        public UpdateBookHandlerTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookValidatorMock = new Mock<IValidator<Book>>();
            _sut = new UpdateBookHandler(_bookRepositoryMock.Object, _bookValidatorMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateBook_WhenBookIsFoundAndValid()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();
            var command = new UpdateBookCommand(id, "Updated Book", 25.99m, 2023);
            var book = new Book { Id = id, Title = "Original Book", Price = 15.99m, PublicationYear = 2020 };

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id, ct)).ReturnsAsync(book);
            _bookValidatorMock.Setup(v => v.ValidateAsync(book, ct)).ReturnsAsync(new ValidationResult());
            _bookRepositoryMock.Setup(r => r.UpdateAsync(book, ct)).Returns(Task.CompletedTask);

            // Act
            var result = await _sut.Handle(command, ct);

            // Assert
            result.Should().Be(id, "the updated book's ID should be returned");
            _bookRepositoryMock.Verify(r => r.UpdateAsync(book, ct), Times.Once, "UpdateAsync should be called once.");
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenBookIsNotFound()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();
            var command = new UpdateBookCommand(id, "Updated Book", 25.99m, 2023);

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(id, ct)).ReturnsAsync((Book?)null);

            // Act
            Func<Task> act = async () => await _sut.Handle(command, ct);

            // Assert
            await act.Should().ThrowAsync<Exception>("the book was not found and cannot be updated");
        }
    }

