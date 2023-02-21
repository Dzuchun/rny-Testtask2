using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using rny_Testtask2.Infrastructure;
using rny_Testtask2.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add data
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseInMemoryDatabase("books_database");
});

// Connect controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // Tell serialiser to not serialise null values
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

var app = builder.Build();

// Api is supposed to be on https, so why not redirect by refault?
app.UseHttpsRedirection();

// Seed the book data
var dataContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(dataContext);

// Add logging
app.UseMiddleware<MyLogger>();

// Map my controllers
app.MapControllers();

app.Run();