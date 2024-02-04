using Microsoft.EntityFrameworkCore;
using WithMediator.Entities;

namespace WithMediator.Db;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    
}