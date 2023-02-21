using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using rny_Testtask2.dto;
using rny_Testtask2.DTO;
using rny_Testtask2.Infrastructure;
using rny_Testtask2.models;

namespace rny_Testtask2.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BooksController : ControllerBase
    {

        private readonly ILogger<BooksController> _logger;
        private readonly IConfiguration _config;
        private readonly DataContext _dataContext;

        public BooksController(ILogger<BooksController> logger, IConfiguration config, DataContext dataContext)
        {
            _logger = logger;
            _config = config;
            _dataContext = dataContext;
        }

        private static Dictionary<string, Func<BookDTO, object>> orders = new()
        {
            { "title", b => b.title },
            { "author", b => b.author },
        };

        // GET api/books?order=...
        [HttpGet("books")]
        public IActionResult GetAllBooks([FromQuery(Name = "order")] string order)
        {
            var orderDelegate = orders.GetValueOrDefault(order, null);
            if (orderDelegate is null)
            {
                // request specifies bad ordering
                return BadRequest($"\"order\" parameter must be one of: [{string.Join(", ", orders.Keys)}]");
            }

            // request is ok, I guess
            return Ok (
                this._dataContext.Books
                // include ratings (so I could calculate average)
                .Include(b => b.Ratings)
                // include reviews (so I could count them)
                .Include(b => b.Reviews)
                // remap to BookDTO
                .Select(b => BookDTO.Create(b))
                // order by whatever user specified
                .OrderBy(orderDelegate)
                .AsEnumerable());
        }

        // GET api/?order=...
        [HttpGet("recommended")]
        public IActionResult GetRecommendedBooks([FromQuery(Name = "genre")] string genre)
        {
            if (genre is null)
            {
                return BadRequest("\"genre\" parameter must be specified for this querry");
            }

            return Ok(
                this._dataContext.Books
                // include ratings (so I could calculate average)
                .Include(b => b.Ratings)
                // include reviews (so I could count them)
                .Include(b => b.Reviews)
                // apparently, string.equals is not supported by LINQ, so yeah...
                .AsEnumerable()
                // filter by genre
                .Where(b => b.Genre == genre)
                // remap to BookDTO
                .Select(b => BookDTO.Create(b))
                // filter by number of reviews
                .Where(b => b.reviewsNumber > 10)
                // order by rating
                .OrderByDescending(b => b.rating)
                // select top 10
                .Take(10));
        }

        // GET api/books/id
        [HttpGet("books/{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _dataContext.Books
                // include ratings (to calculate average rating)
                .Include(b => b.Ratings)
                // include reviews (to collect them into an array)
                .Include(b => b.Reviews)
                // select book searched
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null)
            {
                return NotFound($"Book with id {id} was not found.");
            }


            return Ok(BookDetailsDTO.Create(book));
        }

        // DELETE api/books/id?secret=...
        [HttpDelete("books/{id}")]
        public async Task<IActionResult> DeleteBook([FromQuery(Name = "secret")] string secret, int id)
        {
            if (secret is null || !_config["SecretDeleteKey"].Equals(secret))
            {
                return BadRequest("\"secret\" in not correct.");
            }

            var book = await _dataContext.Books
                // find book searched
                .FindAsync(id);

            if (book is null)
            {
                return NotFound($"Book with id {id} was not found.");
            }

            _dataContext.Remove(book);
            _dataContext.SaveChanges();
            return Ok();
        }

    }
}