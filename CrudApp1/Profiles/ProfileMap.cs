using AutoMapper;
using CrudApp1.Models;
using CrudApp1.Models.Viewmodels;

namespace CrudApp1.Profiles
{
    public class ProfileMap:Profile
    {
        public ProfileMap()
        {
            //CreateMap<ProductViewModel, Product>().
            //    ForMember(dest => dest.Id, opt => opt.Ignore()).
            //    ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)).
            //    ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)).
            //    ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.UnitInStock)).
            //    ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.ImageUrl))
            //    ;

            //CreateMap<Product, ProductViewModel>().
               
            //    ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)).
            //    ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice)).
            //    ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.UnitInStock)).
            //    ForMember(dest => dest.UnitInStock, opt => opt.MapFrom(src => src.ImageUrl))
            //    ;


            CreateMap<Product, ProductViewModel>().ReverseMap();

            CreateMap<CreateProdVm, Product>().ForMember(dest => dest.ImageUrl, opt => opt.Ignore());
        }
    }
}
