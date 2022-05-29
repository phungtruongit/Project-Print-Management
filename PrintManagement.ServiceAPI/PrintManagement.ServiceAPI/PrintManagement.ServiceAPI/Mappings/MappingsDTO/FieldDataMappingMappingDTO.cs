using AutoMapper;
using System;
using System.Linq;

namespace ProjectX.DictionaryAPI.Infrastructure {
    public class FieldDataMappingMappingDTO : Profile {
        public FieldDataMappingMappingDTO() {

            CreateMap<FieldDataMapping, FieldDataMappingDTO>()
                    .ForMember(dest => dest.Oid, opts => opts.MapFrom(src => src.Oid))
                    .ForMember(dest => dest.IDCategory, opts => opts.MapFrom(src => src.IDCategory))
                    .ForMember(dest => dest.FieldDataMapped, opts => opts.MapFrom(src => src.FieldDataMapped))
                    .ForMember(dest => dest.FieldDataDescription, opts => opts.MapFrom(src => src.FieldDataDescription))
                    .ForMember(dest => dest.DataType, opts => opts.MapFrom(src => src.DataType))

                    .ForMember(dest => dest.CategoryNavigation, opts => opts.MapFrom(src => src.IDCategoryNavigation))

                    .ForMember(dest => dest.Status, opts => opts.MapFrom(src => src.Status))
                    .ForMember(dest => dest.Note, opts => opts.MapFrom(src => src.Note))
                    .ForMember(dest => dest.CreateDate, opts => opts.MapFrom(src => src.CreateDate))
                    .ForMember(dest => dest.CreateUser, opts => opts.MapFrom(src => src.CreateUser))
                    .ForMember(dest => dest.UpdateDate, opts => opts.MapFrom(src => src.UpdateDate))
                    .ForMember(dest => dest.UpdateUser, opts => opts.MapFrom(src => src.UpdateUser))
                    .ForMember(dest => dest.ValidUntilDate, opts => opts.MapFrom(src => src.ValidUntilDate))
                    .ForMember(dest => dest.IDDonVi, opts => opts.MapFrom(src => src.IDDonVi)).ReverseMap();
        }
    }
}
