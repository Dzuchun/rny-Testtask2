namespace rny_Testtask2.DTO
{
    public class ReviewDto
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // No, I won't do that. That's the whole point -- both message AND reviewer are reired to put a review.
        public string Message { get; set; }
        public string Reviewer { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
