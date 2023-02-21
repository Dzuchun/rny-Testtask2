using rny_Testtask2.models;

namespace rny_Testtask2.DTO
{
    public class BookDetailsDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public string content { get; set; }
        public decimal rating { get; set; }
        public class Review
        {
            public int id { get; set; }
            public string message { get; set; }
            public string reviewer { get; set; }
        }

        public Review[] reviews { get; set; }

        public static BookDetailsDTO Create(Book book)
        {
            return new()
            {
                id = book.Id,
                title = book.Title,
                cover = book.Cover,
                content = book.Content,
                rating = DTOHelper.GetBookRating(book),
                reviews = book.Reviews.Select(r => new Review
                {
                    id = r.Id,
                    message = r.Message,
                    reviewer = r.Reviewer,
                }).ToArray(),
            };
        }
    }
}
