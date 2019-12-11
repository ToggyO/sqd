using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Common.Models.Resources;

namespace Squadio.DTO.Resources
{
    public class ResourceViewModelMapper: IMapper<ResourceViewModel, ResourceDTO>
    {
        private readonly IMapper _mapper;

        public ResourceViewModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ResourceDTO Map(ResourceViewModel item)
        {
            return new ResourceDTO
            {
                OriginalUrl = item.OriginalUrl,
                FormatUrls = new Dictionary<string, string>
                {
                    {"140", item.Url140},
                    {"360", item.Url360},
                    {"480", item.Url480},
                    {"720", item.Url720},
                    {"1080", item.Url1080},
                },
            };
        }
    }
    
    public class EnumerableResourceViewModelMapper : IMapper<IEnumerable<ResourceViewModel>, IEnumerable<ResourceDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableResourceViewModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ResourceDTO> Map(IEnumerable<ResourceViewModel> items)
        {
            var result = items.Select(x => _mapper.Map<ResourceViewModel, ResourceDTO>(x));
            return result;
        }
    }
}