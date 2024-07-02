using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Book
{
    public class DtoBookReadOnly : DtoBase
    {
        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [Required]
        [StringLength(250)]
        public string? Summary { get; set; }

        public string? Immage { get; set; }

        [Required]
        public decimal? Price { get; set; }

        public int? AuthorId { get; set; }

        public string? AuthorName { get; set; }
    }
}
