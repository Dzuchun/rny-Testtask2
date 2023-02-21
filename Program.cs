using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using rny_Testtask2.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add data
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseInMemoryDatabase("books_database");
});

// Add controllers
builder.Services.AddControllers().AddJsonOptions(options =>
{

});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// seed data
var dataContext = app.Services.CreateScope().ServiceProvider.GetRequiredService<DataContext>();
SeedData.SeedDatabase(dataContext);

app.UseHttpsRedirection();

app.MapControllers();

app.Run();