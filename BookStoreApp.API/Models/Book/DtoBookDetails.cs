namespace BookStoreApp.API.Models.Book
{
    public class DtoBookDetails : DtoBase
    {
        public string Title { get; set; }

        public int Year { get; set; }

        public string Isbn { get; set; } = null!;

        public string Summary { get; set; }

        public string Immage { get; set; }

        public decimal Price { get; set; }

        public int AuthorId { get; set; }

        public string AuthorName { get; set; }
    }
}
