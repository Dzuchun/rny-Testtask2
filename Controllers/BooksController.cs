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
        private readonly IConfiguration _config;
        private readonly DataContext _dataContext;

        public BooksController(IConfiguration config, DataContext dataContext)
        {
            _config = config;
            _dataContext = dataContext;
        }

        private static Dictionary<string, Func<BookDto, object>> orders = new()
        {
            { "title", b => b.Title },
            { "author", b => b.Author },
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
                .Select(b => BookDto.CreateForList(b))
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
                .Select(b => BookDto.CreateForList(b))
                // filter by number of reviews
                .Where(b => b.ReviewsNumber > 10)
                // order by rating
                .OrderByDescending(b => b.Rating)
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


            return Ok(BookDto.CreateDetailed(book));
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
            await _dataContext.SaveChangesAsync();
            return Ok($"Book with id {id} was deleted.");
        }


        // POST api/books/save
        [HttpPost("books/save")]
        public async Task<IActionResult> SaveBook([FromBody] BookDto book)
        {
            // chack that book is not null
            if (book is null)
            {
                return BadRequest("This request must have a book represented in JSON.");
            }

            if (book.Id is not null)
            {
                // try aupdating book
                Book? _book = await _dataContext.Books.FindAsync(book.Id);
                if (_book is not null)
                {
                    // update specified fields
                    if (book.Author is not null) _book.Author = book.Author;
                    if (book.Content is not null) _book.Content = book.Content;
                    if (book.Cover is not null) _book.Cover = book.Cover;
                    if (book.Title is not null) _book.Title = book.Title;
                    if (book.Genre is not null) _book.Genre = book.Genre;
                    await _dataContext.SaveChangesAsync();
                    // book was updated
                    return Ok(new { id = book.Id });
                }
                else
                {
                    // update was requested, but no book is present
                    return NotFound($"Book with id {book.Id} could not be found.");
                    // TODO should I return "id" : "-1" instead....?
                }
            }
            else
            {
                // check if passed model has all required fields
                if (book.Author is null || book.Title is null || book.Genre is null || book.Cover is null || book.Content is null)
                {
                    // these fields are required to create a new book
                    return BadRequest("One of the requied fields is not specified.");
                    // TODO should I return "id" : "-1" instead....?
                }

                // add book
                Book _book = new()
                {
                    Author = book.Author,
                    Title = book.Title,
                    Genre = book.Genre,
                    Cover = book.Cover,
                    Content = book.Content,
                };
                _dataContext.Add(_book);
                await _dataContext.SaveChangesAsync();
                return Ok(new { id = _book.Id });
            }
        }

        // PUT api/books/id/review
        [HttpPut("books/{id}/review")]
        public async Task<IActionResult> PutReview([FromBody] ReviewDto review, int id)
        {
            // check if book exists
            Book? book = await _dataContext.Books.FindAsync(id);
            if (book is null)
            {
                // attempt to post a review on a non-existent book
                return BadRequest($"Book with id {id} was not found.");
                // TODO should I return "id" : "-1" instead....?
            }

            // book exists.
            Review _review = new()
            {
                BookId = id,
                Message = review.Message,
                Reviewer = review.Reviewer,
            };
            _dataContext.Add(_review);
            await _dataContext.SaveChangesAsync();
            return Ok(new { id = _review.Id });
        }

        // PUT api/books/id/review
        [HttpPut("books/{id}/rate")]
        public async Task<IActionResult> PutRate([FromBody] RateDto rate, int id)
        {

            // check if score is correct
            if (rate.Score < 1 || rate.Score > 5)
            {
                return BadRequest($"Rate score can only be from 1 to 5, but {rate.Score} was found.");
            }

            // check if book exists
            Book? book = await _dataContext.Books.FindAsync(id);
            if (book is null)
            {
                // attempt to add reting to a non-existent book
                return BadRequest($"Book with id {id} was not found.");
                // TODO should I return "id" : "-1" instead....?
            }

            // book exists.
            Rating _rating = new()
            {
                BookId = id,
                Score = rate.Score,
            };
            _dataContext.Add(_rating);
            await _dataContext.SaveChangesAsync();
            return Ok(new { id = _rating.Id });
        }
    }
}