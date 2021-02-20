using AutoMapper;
using Squadio.Common.Models.Pages;

namespace Squadio.BLL.Mapping.Profiles
{
    public class PageMapperProfile : Profile
    {
        public PageMapperProfile()
        {
            CreateMap(typeof(PageModel<>), typeof(PageModel<>));
        }
    }
}