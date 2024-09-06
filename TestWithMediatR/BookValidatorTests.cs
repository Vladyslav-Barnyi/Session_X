

using FluentValidation.TestHelper;
using Moq;
using WithMediator.Entities;
using WithMediator.Repositories.Interfaces;
using WithMediator.Validators;

namespace TestWithMediatR
{
    public class BookValidatorTests
    {
        private readonly BookValidator _sut;

        public BookValidatorTests()
        {
            Mock<IBookRepository> bookRepositoryMock = new();
            _sut = new BookValidator(bookRepositoryMock.Object);
        }

        [Fact]
        public void Should_HaveError_When_IdIsEmpty()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.Empty,  // Invalid Guid
                Title = "Valid Title",
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_NotHaveError_When_IdIsValid()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),  // Valid Guid
                Title = "Valid Title",
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldNotHaveValidationErrorFor(x => x.Id);
        }

        [Fact]
        public void Should_HaveError_When_TitleIsEmpty()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "",  // Invalid Title
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_NotHaveError_When_TitleIsValid()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Valid Title",  // Valid Title
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_HaveError_When_PublicationYearIsInFuture()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Valid Title",
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year + 1  // Future Year (Invalid)
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldHaveValidationErrorFor(x => x.PublicationYear);
        }

        [Fact]
        public void Should_NotHaveError_When_PublicationYearIsInPastOrCurrentYear()
        {
            // Arrange
            var book = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Valid Title",
                Price = 10.99m,
                PublicationYear = DateTime.Now.Year  // Current Year (Valid)
            };

            // Act & Assert
            var result = _sut.TestValidate(book);
            result.ShouldNotHaveValidationErrorFor(x => x.PublicationYear);
        }
    }
}
