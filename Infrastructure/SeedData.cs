using rny_Testtask2.models;

namespace rny_Testtask2.Infrastructure
{
    public class SeedData
    {
        public static void SeedDatabase(DataContext context)
        {
            context.Database.EnsureCreated();

            // database is always empty, add data
            Book[] books = new Book[20];
            for (int i = 0; i < 20; i++)
            {
                books[i] = CreateDummyBook(i);
            }

            Review[] reviews = new Review[300];
            for (int i = 0; i < 300; i++)
            {
                reviews[i] = CreateDummyReview(books[Random.Shared.Next(20)]);
            }

            Rating[] ratings = new Rating[500];
            for (int i = 0; i < 500; i++)
            {
                ratings[i] = CreateDummyRating(books[Random.Shared.Next(20)]);
            }

            context.Books.AddRange(books);
            context.Reviews.AddRange(reviews);
            context.Ratings.AddRange(ratings);
            context.SaveChanges();
        }

        private static Book CreateDummyBook(int n)
        {
            return new Book()
            {
                Author = $"author{Random.Shared.Next(n)}",
                Title = $"title{n}",
                Content = $"content{n}",
                Genre = $"genre{Random.Shared.Next(5)}",
                Cover = $"cover{Random.Shared.Next(n)}",
            };
        }

        private static Review CreateDummyReview(Book book)
        {
            return new Review()
            {
                Book = book,
                Message = $"This book is a dummy rubbish, {Random.Shared.NextDouble()}",
                Reviewer = $"Genious no {Random.Shared.Next()}",
            };
        }

        private static Rating CreateDummyRating(Book book)
        {
            return new Rating()
            {
                Book = book,
                Score = Random.Shared.Next(100),
            };
        }
    }
}
