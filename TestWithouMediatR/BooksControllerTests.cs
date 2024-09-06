using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WithoutMadiatR.Contracts.Reposnes;
using WithoutMadiatR.Contracts.Requests;
using WithoutMadiatR.Controllers;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Services.Interfaces;

namespace TestWithoutMediatR
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _bookServiceMock;
        private readonly BooksController _sut;

        public BooksControllerTests()
        {
            _bookServiceMock = new Mock<IBookService>();
            _sut = new BooksController(_bookServiceMock.Object);
        }

        [Fact]
        public async Task GetAllBooks_ShouldReturnOk_WithListOfBooks()
        {
            // Arrange
            var ct = new CancellationToken();
            var books = new List<Book>
            {
                new Book { Id = Guid.NewGuid(), Title = "Book1", Price = 10.99m, PublicationYear = 2020 },
                new Book { Id = Guid.NewGuid(), Title = "Book2", Price = 15.99m, PublicationYear = 2021 }
            };

            _bookServiceMock.Setup(s => s.GetAll(ct)).ReturnsAsync(books);

            // Act
            var result = await _sut.GetAllBooks(ct);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedBooks = okResult.Value as IEnumerable<GetBookResponse>;
            returnedBooks.Should().NotBeNull();
            returnedBooks.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("0bede9e8-1569-4c3e-9129-9d3a506f4df2")] // Test with a valid GUID
        [InlineData("Sample Book Title")] // Test with a title
        public async Task Get_ShouldReturnOk_WithBookResponse_WhenBookExists(string idOrTitle)
        {
            // Arrange
            var ct = new CancellationToken();
            var book = new Book { Id = Guid.NewGuid(), Title = "Sample Book Title", Price = 20.99m, PublicationYear = 2023 };

            if (Guid.TryParse(idOrTitle, out var id))
            {
                _bookServiceMock.Setup(s => s.GetById(id, ct)).ReturnsAsync(book);
            }
            else
            {
                _bookServiceMock.Setup(s => s.GetByTitle(idOrTitle, ct)).ReturnsAsync(book);
            }

            // Act
            var result = await _sut.Get(idOrTitle, ct);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedBook = okResult.Value as GetBookResponse;
            returnedBook.Should().NotBeNull();
            returnedBook!.Title.Should().Be("Sample Book Title");
        }

        [Fact]
        public async Task Get_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var ct = new CancellationToken();
            var idOrTitle = "nonexistent-id-or-title";
            _bookServiceMock.Setup(s => s.GetById(It.IsAny<Guid>(), ct)).ReturnsAsync((Book)null);
            _bookServiceMock.Setup(s => s.GetByTitle(It.IsAny<string>(), ct)).ReturnsAsync((Book)null);

            // Act
            var result = await _sut.Get(idOrTitle, ct);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateBook_ShouldReturnEntityResponse_WithBookId()
        {
            // Arrange
            var ct = new CancellationToken();
            var request = new CreateBookRequest { Title = "New Book", Price = 12.99m, PubYear = 2022 };
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Price = request.Price,
                PublicationYear = request.PubYear
            };
            var createdBook = new Book { Id = book.Id, Title = "New Book", Price = 12.99m, PublicationYear = 2022 };

            _bookServiceMock.Setup(s => s.Create(It.IsAny<Book>(), ct)).ReturnsAsync(createdBook);

            // Act
            var result = await _sut.CreateBook(request, ct);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(createdBook.Id);
        }

        [Fact]
        public async Task UpdateBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();
            var request = new UpdateBookRequest { Title = "Updated Book", Price = 1599, PublicationYear = new DateTime(2023, 1, 1) };

            _bookServiceMock.Setup(s => s.GetById(id, ct)).ReturnsAsync((Book)null);

            // Act
            var result = await _sut.UpdateBook(id, request, ct);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnOk_WhenBookIsDeleted()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();

            _bookServiceMock.Setup(s => s.DeleteById(id, ct)).ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteBook(id, ct);

            // Assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Arrange
            var ct = new CancellationToken();
            var id = Guid.NewGuid();

            _bookServiceMock.Setup(s => s.DeleteById(id, ct)).ReturnsAsync(false);

            // Act
            var result = await _sut.DeleteBook(id, ct);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
