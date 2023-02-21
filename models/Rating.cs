using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rny_Testtask2.models
{
    [Table(name: "rating")]
    public class Rating
    {
        [Key]
        [Column(name: "id")]
        public int Id { get; set; }

        [Column(name: "score")]
        public int Score { get; set; }

        [Column(name: "book_id")]
        [ForeignKey(name: "Book")]
        public int BookId { get; set; }

        public Book? Book { get; set; }
    }
}
