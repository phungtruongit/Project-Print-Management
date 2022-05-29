using AutoMapper;
using ProjectX.DictionaryAPI.Infrastructure.DTOs;
using System;
using System.Linq;

namespace ProjectX.DictionaryAPI.Infrastructure {
    public class ManageCategoryMappingDTO : Profile {
        public ManageCategoryMappingDTO() {

            CreateMap<ManageCategory, ManageCategoryDTO>()
                    .ForMember(dest => dest.Oid, opts => opts.MapFrom(src => src.Oid))
                    .ForMember(dest => dest.Code, opts => opts.MapFrom(src => src.Code))
                    .ForMember(dest => dest.Name, opts => opts.MapFrom(src => src.Name))
                    .ForMember(dest => dest.ScopeApplication, opts => opts.MapFrom(src => src.ScopeApplication))

                    //.ForMember(dest => dest.ManageUrlApis, opts => opts.MapFrom(src => src.ManageUrlApis))
                    //.ForMember(dest => dest.FieldDataMappings, opts => opts.MapFrom(src => src.FieldDataMappings))
                    //.ForMember(dest => dest.XDM0002, opts => opts.MapFrom(src => src.XDM0002))

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
