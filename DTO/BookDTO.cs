using rny_Testtask2.DTO;
using rny_Testtask2.models;
using System.ComponentModel.DataAnnotations.Schema;

namespace rny_Testtask2.dto
{
    public class BookDto
    {
        public int? Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public decimal? Rating { get; set; }
        public string? Genre { get; set; }
        public string? Cover { get; set; }
        public string? Content { get; set; }
        public int? ReviewsNumber { get; set; }
        public class Review
        {
            public int Id { get; set; }
            public string? Message { get; set; }
            public string? Reviewer { get; set; }
        }

        public Review[]? Reviews { get; set; }

        public static BookDto CreateForList(Book book)
        {
            return new BookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Rating = DTOHelper.GetBookRating(book),
                ReviewsNumber = book.Reviews.Count(),
            };
        }

        public static BookDto CreateDetailed(Book book)
        {
            return new()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Cover = book.Cover,
                Content = book.Content,
                Rating = DTOHelper.GetBookRating(book),
                Reviews = book.Reviews.Select(r => new Review
                {
                    Id = r.Id,
                    Message = r.Message,
                    Reviewer = r.Reviewer,
                }).ToArray(),
            };
        }
    }
}
