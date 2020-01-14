using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class ByteImageCreateDTOMapper: IMapper<ByteImageCreateDTO, ImageCreateDTO>
    {
        private readonly IMapper _mapper;

        public ByteImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public ImageCreateDTO Map(ByteImageCreateDTO item)
        {
            var result = new ImageCreateDTO
            {
                ContentType = item.ContentType,
                Stream = new MemoryStream(item.Bytes)
            };

            return result;
        }
    }
    
    public class EnumerableByteImageCreateDTOMapper : IMapper<IEnumerable<ByteImageCreateDTO>, IEnumerable<ImageCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableByteImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ImageCreateDTO> Map(IEnumerable<ByteImageCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<ByteImageCreateDTO, ImageCreateDTO>(x));
            return result;
        }
    }
}