namespace BookStoreApp.API.Models.Author
{
    // eredita da DtoBase l'id
    public class DtoAuthorReadOnly : DtoBase
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
    }
}
