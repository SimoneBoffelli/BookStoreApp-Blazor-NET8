using AutoMapper;
// Alias per la classe Author (evita conflitti con la classe Author e il nameplate Author)
using AuthorEntity = BookStoreApp.API.Data.Author;
using BookStoreApp.API.Models.Author;

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
        }
    }
}
