namespace BookStoreApp.API.Models.Book
{
    public class DtoBookReadOnly : DtoBase
    {
        public string? Title { get; set; }

        public string? Summary { get; set; }

        public string? Immage { get; set; }

        public decimal? Price { get; set; }

        public int? AuthorId { get; set; }
        public string? AuthorName { get; set; }
    }
}
