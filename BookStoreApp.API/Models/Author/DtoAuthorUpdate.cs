using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.API.Models.Author
{
    public class DtoAuthorUpdate: DtoBase
    {
        // la lista di libri non viene mostrata ma eredita l'id da DtoBase
        // esempio di update di un autore personalizzato

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(250)]
        public string? Bio { get; set; }
    }
}
