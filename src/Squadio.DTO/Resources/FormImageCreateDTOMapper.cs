using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class FormImageCreateDTOMapper: IMapper<FormImageCreateDTO, ImageCreateDTO>
    {
        private readonly IMapper _mapper;

        public FormImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ImageCreateDTO Map(FormImageCreateDTO item)
        {
            var result = new ImageCreateDTO
            {
                ContentType = item.File.ContentType,
                Stream = new MemoryStream()
            };

            item.File.CopyTo(result.Stream);

            return result;
        }
    }
    
    public class EnumerableFormImageCreateDTOMapper : IMapper<IEnumerable<FormImageCreateDTO>, IEnumerable<ImageCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableFormImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ImageCreateDTO> Map(IEnumerable<FormImageCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<FormImageCreateDTO, ImageCreateDTO>(x));
            return result;
        }
    }
}