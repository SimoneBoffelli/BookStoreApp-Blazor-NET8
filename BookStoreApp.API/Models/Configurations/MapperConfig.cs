using AutoMapper;
// Alias per la classe Author (evita conflitti con la classe Author e il nameplate Author)
using AuthorEntity = BookStoreApp.API.Data.Author;
using BookStoreApp.API.Models.Author;

// Alias per la classe Book (evita conflitti con la classe Book e il nameplate Book)
using BookEntity = BookStoreApp.API.Data.Book;
using BookStoreApp.API.Models.Book;

namespace BookStoreApp.API.Models.Configurations
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // Mappatura tra classe Author e DtoAuthorCreate
            // permette di mappare i campi di Author con quelli di DtoAuthorCreate (tra database e data transfer object)
            CreateMap<DtoAuthorCreate, AuthorEntity>().ReverseMap(); // usare nome variabile definita in using
            CreateMap<DtoAuthorReadOnly, AuthorEntity>().ReverseMap();
            CreateMap<DtoAuthorUpdate, AuthorEntity>().ReverseMap();

            //CreateMap<DtoAuthorCreate, AuthorEntity>().ReverseMap(); 
            CreateMap<DtoBookReadOnly, BookEntity>().ReverseMap();
            // mappatura tra BookEntity e DtoBookReadOnly per visualizzare il nome dell'autore
            CreateMap<BookEntity, DtoBookReadOnly>()
                .ForMember(q => q.AuthorName, 
                d => d.MapFrom(map => $"{map.Author.FirstName} {map.Author.LastName}"))
                .ReverseMap();
            //CreateMap<DtoAuthorUpdate, AuthorEntity>().ReverseMap();
        }
    }
}
