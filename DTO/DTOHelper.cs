using rny_Testtask2.models;

namespace rny_Testtask2.DTO
{
    public class DTOHelper
    {
        public static decimal GetBookRating(Book book)
        {
            return (decimal)book.Ratings.Select(r => r.Score).DefaultIfEmpty(0).Average();
        }
    }
}
