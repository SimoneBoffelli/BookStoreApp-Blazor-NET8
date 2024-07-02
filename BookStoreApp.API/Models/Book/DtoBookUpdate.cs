using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Book
{
    public class DtoBookUpdate : DtoBase
    {
        [Required]
        [StringLength(50)]
        public string? Title { get; set; }

        [Required]
        // crea un range di valori per l'anno a partire dal 1000
        [Range(1000, int.MaxValue)]
        public int? Year { get; set; }

        [Required]
        [MinLength(10), MaxLength(13)]
        public string Isbn { get; set; } = null!;

        [Required]
        [StringLength(250, MinimumLength = 10)]
        public string? Summary { get; set; }

        public string? Immage { get; set; }

        [Required]
        // crea un range di valori per il prezzo evitando valori negativi
        [Range(0, int.MaxValue)]
        public decimal? Price { get; set; }
    }
}
