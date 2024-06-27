namespace BookStoreApp.API.Models
{
    public class DtoBase
    {
        // in questa classe (superclasse) ci sono proprietà comuni a tutte le classi DtoAuthor (sottoclassi)
        public int Id { get; set; }
    }
}
