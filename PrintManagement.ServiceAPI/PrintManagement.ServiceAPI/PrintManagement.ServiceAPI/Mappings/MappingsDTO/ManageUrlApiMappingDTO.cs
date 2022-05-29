using AutoMapper;
using ProjectX.DictionaryAPI.Infrastructure.DTOs;
using System;
using System.Linq;

namespace ProjectX.DictionaryAPI.Infrastructure {
    public class ManageUrlApiMappingDTO : Profile {
        public ManageUrlApiMappingDTO() {

            CreateMap<ManageUrlApi, ManageUrlApiDTO>()
                    .ForMember(dest => dest.Oid, opts => opts.MapFrom(src => src.Oid))
                    .ForMember(dest => dest.UrlGetAll, opts => opts.MapFrom(src => src.UrlGetAll))
                    .ForMember(dest => dest.UrlGetById, opts => opts.MapFrom(src => src.UrlGetById))
                    .ForMember(dest => dest.UrlInsert, opts => opts.MapFrom(src => src.UrlInsert))
                    .ForMember(dest => dest.UrlUpdate, opts => opts.MapFrom(src => src.UrlUpdate))
                    .ForMember(dest => dest.UrlDelete, opts => opts.MapFrom(src => src.UrlDelete))
                    .ForMember(dest => dest.UrlAnother, opts => opts.MapFrom(src => src.UrlAnother))

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
