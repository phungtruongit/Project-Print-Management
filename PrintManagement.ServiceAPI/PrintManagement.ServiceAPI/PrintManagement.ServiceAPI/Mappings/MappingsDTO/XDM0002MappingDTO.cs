using AutoMapper;
using ProjectX.DictionaryAPI.Infrastructure.DTOs;
using System;
using System.Linq;

namespace ProjectX.DictionaryAPI.Infrastructure {
    public class XDM0002MappingDTO : Profile {
        public XDM0002MappingDTO() {

            CreateMap<XDM0002, XDM0002DTO>()
                    .ForMember(dest => dest.DM000201, opts => opts.MapFrom(src => src.DM000201))
                    .ForMember(dest => dest.DM000202, opts => opts.MapFrom(src => src.DM000202))
                    .ForMember(dest => dest.DM000203, opts => opts.MapFrom(src => src.DM000203))
                    .ForMember(dest => dest.DM000204, opts => opts.MapFrom(src => src.DM000204))
                    .ForMember(dest => dest.DM000205, opts => opts.MapFrom(src => src.DM000205))

                    .ForMember(dest => dest.DM000209, opts => opts.MapFrom(src => src.DM000209))
                    .ForMember(dest => dest.DM000210, opts => opts.MapFrom(src => src.DM000210))
                    .ForMember(dest => dest.DM000211, opts => opts.MapFrom(src => src.DM000211))
                    .ForMember(dest => dest.DM000212, opts => opts.MapFrom(src => src.DM000212))
                    .ForMember(dest => dest.DM000213, opts => opts.MapFrom(src => src.DM000213))
                    .ForMember(dest => dest.DM000214, opts => opts.MapFrom(src => src.DM000214))
                    .ForMember(dest => dest.DM000215, opts => opts.MapFrom(src => src.DM000215))

                    .ForMember(dest => dest.DM000216, opts => opts.MapFrom(src => src.DM000216))
                    .ForMember(dest => dest.DM000217, opts => opts.MapFrom(src => src.DM000217))

                    .ForMember(dest => dest.DM000220, opts => opts.MapFrom(src => src.DM000220))
                    .ForMember(dest => dest.DM000221, opts => opts.MapFrom(src => src.DM000221))
                    .ForMember(dest => dest.DM000222, opts => opts.MapFrom(src => src.DM000222))
                    .ForMember(dest => dest.DM000223, opts => opts.MapFrom(src => src.DM000223))

                    .ForMember(dest => dest.DM000230, opts => opts.MapFrom(src => src.DM000230))
                    .ForMember(dest => dest.DM000231, opts => opts.MapFrom(src => src.DM000231))
                    .ForMember(dest => dest.DM000232, opts => opts.MapFrom(src => src.DM000232))

                    .ForMember(dest => dest.DM000235, opts => opts.MapFrom(src => src.DM000235))

                    .ForMember(dest => dest.DM000240, opts => opts.MapFrom(src => src.DM000240))
                    .ForMember(dest => dest.DM000241, opts => opts.MapFrom(src => src.DM000241))
                    .ForMember(dest => dest.DM000242, opts => opts.MapFrom(src => src.DM000242))
                    .ForMember(dest => dest.DM000243, opts => opts.MapFrom(src => src.DM000243))

                    .ForMember(dest => dest.DM000250, opts => opts.MapFrom(src => src.DM000250))
                    .ForMember(dest => dest.DM000251, opts => opts.MapFrom(src => src.DM000251))

                    .ForMember(dest => dest.DM000260, opts => opts.MapFrom(src => src.DM000260))
                    .ForMember(dest => dest.DM000261, opts => opts.MapFrom(src => src.DM000261))
                    .ForMember(dest => dest.DM000262, opts => opts.MapFrom(src => src.DM000262))
                    .ForMember(dest => dest.DM000263, opts => opts.MapFrom(src => src.DM000263))
                    .ForMember(dest => dest.DM000264, opts => opts.MapFrom(src => src.DM000264))
                    .ForMember(dest => dest.DM000265, opts => opts.MapFrom(src => src.DM000265))
                    .ForMember(dest => dest.DM000266, opts => opts.MapFrom(src => src.DM000266))
                    .ForMember(dest => dest.DM000267, opts => opts.MapFrom(src => src.DM000267))

                    //.ForMember(dest => dest.DM000210NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000210)))
                    //.ForMember(dest => dest.DM000211NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000211)))
                    //.ForMember(dest => dest.DM000212NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000212)))
                    //.ForMember(dest => dest.DM000213NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000213)))
                    //.ForMember(dest => dest.DM000214NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000214)))
                    //.ForMember(dest => dest.DM000215NavigationName, opts => opts.MapFrom(src => RefProcess.GetNameFromId(src.DM000215)))

                    .ForMember(dest => dest.DM000282, opts => opts.MapFrom(src => src.DM000282))
                    .ForMember(dest => dest.DM000283, opts => opts.MapFrom(src => src.DM000283))
                    .ForMember(dest => dest.DM000284, opts => opts.MapFrom(src => src.DM000284))
                    .ForMember(dest => dest.DM000294, opts => opts.MapFrom(src => src.DM000294))
                    .ForMember(dest => dest.DM000295, opts => opts.MapFrom(src => src.DM000295))
                    .ForMember(dest => dest.DM000296, opts => opts.MapFrom(src => src.DM000296))
                    .ForMember(dest => dest.DM000297, opts => opts.MapFrom(src => src.DM000297))
                    .ForMember(dest => dest.DM000298, opts => opts.MapFrom(src => src.DM000298))
                    .ForMember(dest => dest.DM000299, opts => opts.MapFrom(src => src.DM000299)).ReverseMap();
        }
    }
}
