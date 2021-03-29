using AutoMapper;
using BookLibrary.Models;
using BookLibrary.ViewModels;

namespace BookLibrary.Data
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Book, BookViewModel>();

            CreateMap<BookViewModel, Book>();

            CreateMap<Rental, RentalViewModel>();

            CreateMap<RentalViewModel, Rental>();
        }
    }
}
