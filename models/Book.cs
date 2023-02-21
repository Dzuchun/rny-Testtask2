using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rny_Testtask2.models
{
    [Table(name: "book")]
    public class Book
    {
        [Key]
        [Column(name: "id")]
        public int Id { get; set; }

        [Column(name: "title")]
        public string? Title { get; set; }

        [Column(name: "cover")]
        public string? Cover { get; set; }

        [Column(name: "content")]
        public string? Content { get; set; }

        [Column(name: "author")]
        public string? Author { get; set; }

        [Column(name: "genre")]
        public string? Genre { get; set; }

        public IEnumerable<Rating> Ratings { get; set; } = new List<Rating>();

        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
    }
}