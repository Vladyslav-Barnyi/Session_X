using Microsoft.EntityFrameworkCore;
using WithMediator.Db;
using WithMediator.Entities;
using WithMediator.Repositories;

namespace TestWithMediatR
{
    public class BookRepositoryTests
    {
        private BookContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BookContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var dbContext = new BookContext(options);
            dbContext.Database.OpenConnection();
            dbContext.Database.EnsureCreated();

            return dbContext;
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllBooks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book1 = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };
            var book2 = new Book { Id = Guid.NewGuid(), Title = "Book 2", Price = 12.99m, PublicationYear = 2010 };

            context.Books.Add(book1);
            context.Books.Add(book2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAll(CancellationToken.None, null, null);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(book.Id, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result?.Id);
        }

        [Fact]
        public async Task GetByTitleAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByTitleAsync(book.Title, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Title, result?.Title);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddNewBook()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };

            // Act
            var result = await repository.CreateAsync(book, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(book.Id, result.Id);

            var createdBook = await context.Books.FindAsync(book.Id);
            Assert.NotNull(createdBook);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyBook()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            book.Title = "Updated Title";

            // Act
            await repository.UpdateAsync(book, CancellationToken.None);

            // Assert
            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Title", updatedBook?.Title);
        }

        [Fact]
        public async Task RemoveAsync_ShouldDeleteBook_WhenBookExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new BookRepository(context);

            var book = new Book { Id = Guid.NewGuid(), Title = "Book 1", Price = 10.99m, PublicationYear = 2000 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.RemoveAsync(book.Id, CancellationToken.None);

            // Assert
            Assert.True(result);
            var deletedBook = await context.Books.FindAsync(book.Id);
            Assert.Null(deletedBook);
        }
    }
}
