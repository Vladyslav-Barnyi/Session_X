using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WithMediator.Contracts.Reposnes;
using WithMediator.Contracts.Requests;
using WithMediator.Controllers;
using WithMediator.Entities;
using WithMediator.MediatR;

namespace TestWithMediatR
{
    public class BooksControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly BooksController _sut;

        public BooksControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _sut = new BooksController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnOkWithListOfBooks()
        {
            // Arrange
            var ct = new CancellationToken();
            var books = new List<GetBookResponse>
            {
                new GetBookResponse { Id = Guid.NewGuid(), Title = "Book 1", Price = 19.99m, PubYeat = 2023 },
                new GetBookResponse { Id = Guid.NewGuid(), Title = "Book 2", Price = 29.99m, PubYeat = 2021 }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllBooksQuery>(), ct)).ReturnsAsync(books);

            // Act
            var result = await _sut.GetAllBooks(ct);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult.StatusCode.Should().Be(200);
            
            var response = okResult.Value as List<GetBookResponse>;
            response.Should().NotBeNull();
            response!.Count.Should().Be(2);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenBookIsFound()
        {
            // Arrange
            var ct = new CancellationToken();
            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 19.99m, PublicationYear = 2023 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookQuery>(), ct)).ReturnsAsync(book);

            // Act
            var result = await _sut.Get(book.Id.ToString(), ct);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var response = okResult.Value as GetBookResponse;
            response.Should().NotBeNull();
            response!.Title.Should().Be(book.Title);
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenBookIsNotFound()
        {
            // Arrange
            var ct = new CancellationToken();

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBookQuery>(), ct)).ReturnsAsync((Book?)null);

            // Act
            var result = await _sut.Get("nonexistent", ct);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateBook_ShouldReturnEntityResponseWithCreatedBookId()
        {
            // Arrange
            var ct = new CancellationToken();
            var request = new CreateBookRequest { Title = "New Book", Price = 9.99m, PublicationYear = 2023 };
            var createdBook = new Book { Id = Guid.NewGuid(), Title = request.Title, Price = request.Price, PublicationYear = request.PublicationYear };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateBookCommand>(), ct)).ReturnsAsync(createdBook);

            // Act
            var result = await _sut.CreateBook(request, ct);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdBook.Id);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnEntityResponseWithUpdatedBookId()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();
            var request = new UpdateBookRequest { Title = "Updated Title", Price = 15, PublicationYear = new DateTime(2023, 1, 1) };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateBookCommand>(), ct)).ReturnsAsync(id);

            // Act
            var result = await _sut.UpdateBook(id, request, ct);

            // Assert
            var response = result.Value;
            response.Should().NotBeNull();
            response!.Id.Should().Be(id);
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnOk_WhenBookIsDeleted()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBookCommand>(), ct)).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteBook(id, ct);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNotFound_WhenBookIsNotDeleted()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteBookCommand>(), ct)).ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteBook(id, ct);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
