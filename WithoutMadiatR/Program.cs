using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WithoutMadiatR.Db;
using WithoutMadiatR.Entities;
using WithoutMadiatR.Repositories;
using WithoutMadiatR.Repositories.Interfaces;
using WithoutMadiatR.Services;
using WithoutMadiatR.Services.Interfaces;
using WithoutMadiatR.Validators;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BookContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IValidator<Book>, BookValidator>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

