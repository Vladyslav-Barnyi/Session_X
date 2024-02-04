using Microsoft.EntityFrameworkCore;
using WithoutMadiatR.Entities;

namespace WithoutMadiatR.Db;

public class BookContext : DbContext
{
    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {
        
    }

    public DbSet<Book> Books { get; set; }
    
}