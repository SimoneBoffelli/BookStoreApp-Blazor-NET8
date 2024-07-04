using AutoMapper;
// Alias per la classe Author (evita conflitti con la classe Author e il nameplate Author)
using AuthorEntity = BookStoreApp.API.Data.Author;
using BookStoreApp.API.Models.Author;

// Alias per la classe Book (evita conflitti con la classe Book e il nameplate Book)
using BookEntity = BookStoreApp.API.Data.Book;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Models.User;
using BookStoreApp.API.Data;

namespace BookStoreApp.API.Models.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // Mappatura tra classe Author e DtoAuthor
            // permette di mappare i campi di Author con quelli di DtoAuthorCreate (tra database e data transfer object)
            CreateMap<DtoAuthorCreate, AuthorEntity>().ReverseMap(); // usare nome variabile definita in using
            CreateMap<DtoAuthorReadOnly, AuthorEntity>().ReverseMap();
            CreateMap<DtoAuthorUpdate, AuthorEntity>().ReverseMap();

            // Mappatura tra classe Book e DtoBook
            CreateMap<DtoBookCreate, BookEntity>().ReverseMap();
            CreateMap<DtoBookReadOnly, BookEntity>().ReverseMap();
            CreateMap<DtoBookUpdate, BookEntity>().ReverseMap();
            CreateMap<DtoBookDetails, BookEntity>().ReverseMap();

            // mappatura tra BookEntity e DtoBook per visualizzare il nome dell'autore
            CreateMap<BookEntity, DtoBookReadOnly>()
                .ForMember(q => q.AuthorName, 
                d => d.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}"))
                .ReverseMap();
            CreateMap<BookEntity, DtoBookDetails>()
                .ForMember(q => q.AuthorName,
                d => d.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}"))
                .ReverseMap();

            // mappatura tra User e DtoUser
            CreateMap<ApiUser, DtoUser>().ReverseMap();
        }
    }
}
