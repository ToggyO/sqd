﻿using System.IO;
using AutoMapper;
using Squadio.Common.Models.Resources;
using Squadio.Domain.Models.Resources;
using Squadio.DTO.Models.Resources;

namespace Squadio.BLL.Mapping.Profiles
{
    public class ResourceMapperProfile: Profile
    {
        public ResourceMapperProfile()
        {
            CreateMap<ByteImageCreateDTO, ImageCreateDTO>()
                .ForMember(
                    item => item.ContentType,
                    map => map.MapFrom(src => src.ContentType))
                .ForMember(
                    item => item.Stream,
                    map => map.MapFrom(src => new MemoryStream(src.Bytes)));
            
            CreateMap<FormImageCreateDTO, ImageCreateDTO>()
                .ForMember(
                    item => item.ContentType,
                    map => map.MapFrom(src => src.File.ContentType))
                .ForMember(
                    item => item.Stream, 
                    opt => opt.MapFrom((from, to) =>
                {
                    var mapped = new MemoryStream();
                    from.File.CopyTo(mapped);
                    return mapped;
                }));
            
            
            CreateMap<ResourceImageViewModel, ResourceImageDTO>();
            CreateMap<ResourceViewModel, ResourceDTO>();
            
            CreateMap<ResourceModel, ResourceImageViewModel>()
                .ForMember(
                    item => item,
                    map => map.MapFrom(src => new ResourceImageViewModel(src)));
            CreateMap<ResourceModel, ResourceViewModel>()
                .ForMember(
                    item => item,
                    map => map.MapFrom(src => new ResourceViewModel(src)));
            
            //TODO: Think about how to map correctly
            CreateMap<ResourceModel, ResourceImageDTO>().ConvertUsing((from, to) =>
            {
                var viewModel = new ResourceImageViewModel(from);
                return new ResourceImageDTO
                {
                    FormatUrls = viewModel.FormatUrls,
                    OriginalUrl = viewModel.OriginalUrl,
                    ResourceId = viewModel.ResourceId
                };
            });
            
            //TODO: Think about how to map correctly
            CreateMap<ResourceModel, ResourceDTO>().ConvertUsing((from, to) =>
            {
                var viewModel = new ResourceViewModel(from);
                return new ResourceDTO
                {
                    OriginalUrl = viewModel.OriginalUrl,
                    ResourceId = viewModel.ResourceId
                };
            });
        }
    }
}