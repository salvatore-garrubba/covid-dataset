using AutoMapper;

using Covid.Importer.Domain.Models;
using Covid.Data.Domain.Models;
using System.Collections.Generic;

namespace Covid.Importer.Mapping
{
    public class InputToModelProfile : Profile
    {
        public InputToModelProfile()
        {            
            CreateMap<InputData, ConvertedData>()
            .ForMember(
                dest => dest.FileName,
                opts => opts.MapFrom(src => src.FileName)
            )
            .ForMember(
                dest => dest.Category,
                opts => opts.MapFrom(src => src)
            )
            .ForMember(
                dest => dest.Question,
                opts => opts.MapFrom(src => src)
            )
            .ForMember(
                dest => dest.Answer,
                opts => opts.MapFrom(src => src)
            ).ReverseMap();            

            CreateMap<InputData, Category>()
            .ForMember(
                    dest => dest.Id,
                    opts => opts.MapFrom(src => src.CategoryId)
            ).ReverseMap(); 

            CreateMap<InputData, Question>()
            .ForMember(
                dest => dest.Id,
                opts => opts.MapFrom(src => src.QuestionId)
            )
            .ForMember(
                dest => dest.Text,
                opts => opts.MapFrom(src => src.QuestionText)
            )
            .ForMember(
                dest => dest.CategoryId,
                opts => opts.MapFrom(src => src.CategoryId)
            ).ReverseMap();

            CreateMap<InputData, Answer>()
            .ForMember(
                dest => dest.Id,
                opts => opts.MapFrom(src => $"{src.QuestionId}_{src.Timestamp.Ticks}")
            )
            .ForMember(
                dest => dest.QuestionId,
                opts => opts.MapFrom(src => src.QuestionId)
            )
            .ForMember(
                dest => dest.Text,
                opts => opts.MapFrom(src => src.AnswerText)
            )
            .ForMember(
                dest => dest.Timestamp,
                opts => opts.MapFrom(src => src.Timestamp)
            )
            .ReverseMap();
            // CreateMap<IEnumerable<ConvertedData>, ImportResult>()
            // .ForMember(
            //     dest => dest.Data,
            //     opts => opts.MapFrom(src => src)
            // );
        }
    }
}