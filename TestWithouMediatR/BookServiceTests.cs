using FluentValidation;
using FluentValidation.Results;
using Moq;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Repositories.Interfaces;
using WithoutMadiatR.Services;
using WithoutMadiatR.Services.Interfaces;

namespace TestWithoutMediatR;

    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepositoryMock;
        private readonly Mock<IValidator<Book>> _bookValidatorMock;
        private readonly IBookService _sut;

        public BookServiceTests()
        {
            _bookRepositoryMock = new Mock<IBookRepository>();
            _bookValidatorMock = new Mock<IValidator<Book>>();
            _sut = new BookService(_bookRepositoryMock.Object, _bookValidatorMock.Object);
        }

        [Fact]
        public async Task Create_ShouldAddNewBook_WhenValidationPasses()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "New Book", Price = 9.99m, PublicationYear = 2023 };
            _bookValidatorMock.Setup(v => v.ValidateAsync(book, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.CreateAsync(book, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            // Act
            var result = await _sut.Create(book, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _bookRepositoryMock.Verify(r => r.CreateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUpdateBook_WhenValidationPassesAndBookExists()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Existing Book", Price = 19.99m, PublicationYear = 2020 };
            _bookValidatorMock.Setup(v => v.ValidateAsync(book, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            _bookRepositoryMock.Setup(r => r.UpdateAsync(book, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _sut.Update(book, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            _bookRepositoryMock.Verify(r => r.UpdateAsync(book, It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async Task Update_ShouldThrowException_WhenBookDoesNotExist()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Non-Existing Book", Price = 19.99m, PublicationYear = 2020 };
            _bookValidatorMock.Setup(v => v.ValidateAsync(book, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.Update(book, CancellationToken.None));
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetById_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Existing Book", Price = 19.99m, PublicationYear = 2020 };
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(book.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(book);

            // Act
            var result = await _sut.GetById(book.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result?.Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _sut.GetById(bookId, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Update_ShouldThrowException_WhenValidationFails()
        {
            // Arrange
            var book = new Book { Id = Guid.NewGuid(), Title = "Existing Book", Price = 19.99m, PublicationYear = 2020 };
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Title", "Title is required") };
            var validationResult = new ValidationResult(validationFailures);

            _bookValidatorMock.Setup(v => v.ValidateAsync(book, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.Update(book, CancellationToken.None));

            // Validate that the exception message contains the expected error
            _bookRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        }
        
        [Fact]
        public async Task GetByTitle_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var title = "Existing Book";
            var expectedBook = new Book { Id = Guid.NewGuid(), Title = title, Price = 19.99m, PublicationYear = 2020 };

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(title, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBook);

            // Act
            var result = await _sut.GetByTitle(title, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedBook.Id, result.Id);
            Assert.Equal(expectedBook.Title, result.Title);
            Assert.Equal(expectedBook.Price, result.Price);
            Assert.Equal(expectedBook.PublicationYear, result.PublicationYear);
        }
        
        [Fact]
        public async Task GetByTitle_ShouldReturnNull_WhenBookDoesNotExist()
        {
            // Arrange
            var title = "Non-Existing Book";

            _bookRepositoryMock.Setup(r => r.GetByTitleAsync(title, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _sut.GetByTitle(title, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
        [Fact]
        public async Task DeleteById_ShouldReturnTrue_WhenBookIsDeleted()
        {
            // Arrange
            var bookId = Guid.NewGuid();
            _bookRepositoryMock.Setup(r => r.RemoveAsync(bookId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.DeleteById(bookId, CancellationToken.None);

            // Assert
            Assert.True(result);
            _bookRepositoryMock.Verify(r => r.RemoveAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

