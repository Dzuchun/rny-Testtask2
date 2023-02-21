using rny_Testtask2.DTO;
using rny_Testtask2.models;

namespace rny_Testtask2.dto
{
    public class BookDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string author { get; set; }
        public decimal rating { get; set; }
        public int reviewsNumber { get; set; }

        public static BookDTO Create(Book book)
        {
            return Create(book, DTOHelper.GetBookRating(book));
        }

        public static BookDTO Create(Book book, decimal averageRating)
        {
            return new BookDTO()
            {
                id = book.Id,
                title = book.Title,
                author = book.Author,
                rating = averageRating,
                reviewsNumber = book.Reviews.Count(),
            };
        }
    }
}
