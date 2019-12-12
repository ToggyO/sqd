using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Common.Models.Resources;

namespace Squadio.DTO.Resources
{
    public class ResourceImageViewModelMapper: IMapper<ResourceViewModel, ResourceImageDTO>
    {
        private readonly IMapper _mapper;

        public ResourceImageViewModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ResourceImageDTO Map(ResourceViewModel item)
        {
            return new ResourceImageDTO
            {
                ResourceId = item.Id,
                OriginalUrl = item.OriginalUrl,
                FormatUrls = new Dictionary<string, string>
                {
                    {"140", item.Url140},
                    {"360", item.Url360},
                    {"480", item.Url480},
                    {"720", item.Url720},
                    {"1080", item.Url1080},
                }
            };
        }
    }
    
    public class EnumerableResourceImageViewModelMapper : IMapper<IEnumerable<ResourceViewModel>, IEnumerable<ResourceImageDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableResourceImageViewModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ResourceImageDTO> Map(IEnumerable<ResourceViewModel> items)
        {
            var result = items.Select(x => _mapper.Map<ResourceViewModel, ResourceImageDTO>(x));
            return result;
        }
    }
}