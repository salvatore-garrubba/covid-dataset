using AutoMapper;
using Covid.API.Domain.Models;
using Covid.Data.Domain.Models;

namespace Covid.API.Mapping
{
    public class ApiToModelProfile : Profile
    {
        public ApiToModelProfile()
        {
            CreateMap<Category, CategoryResource>().ReverseMap();
            /*
            .ForMember(
                dest => dest.Id,
                opts => opts.MapFrom(src => src.Id)
            )
            .ReverseMap();
            */

            CreateMap<Question, QuestionResource>().ReverseMap();
            /*
            .ForMember(
                dest => dest.Id,
                opts => opts.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.CategoryId,
                opts => opts.MapFrom(src => src.CategoryId)
            ).ForMember(
                dest => dest.Text,
                opts => opts.MapFrom(src => src.Text)
            )
            .ReverseMap();
            */
        }
    }
}