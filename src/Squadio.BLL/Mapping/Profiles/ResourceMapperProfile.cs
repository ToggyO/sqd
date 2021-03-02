using System.IO;
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
            
            CreateMap<ResourceImageViewModel, ResourceImageDTO>()
                .ForMember(
                    item => item.ResourceId,
                    map => map.MapFrom(src => src.Id));
        }
    }
}