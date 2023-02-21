using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rny_Testtask2.models
{
    [Table(name: "review")]
    public class Review
    {
        [Key]
        [Column(name: "message")]
        public int Id { get; set; }

        [Column(name: "message")]
        public string Message { get; set; }

        [Column(name: "reviewer")]
        public string Reviewer { get; set; }

        [Column(name: "book_id")]
        [ForeignKey(name: "Book")]
        public int BookId { get; set; }

        public Book Book { get; set; }
    }
}
