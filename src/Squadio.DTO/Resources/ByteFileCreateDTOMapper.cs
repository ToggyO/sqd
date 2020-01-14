using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class ByteFileCreateDTOMapper: IMapper<ByteFileCreateDTO, FileCreateDTO>
    {
        private readonly IMapper _mapper;

        public ByteFileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public FileCreateDTO Map(ByteFileCreateDTO item)
        {
            var result = new FileCreateDTO
            {
                ContentType = item.ContentType,
                Stream = new MemoryStream(item.Bytes)
            };

            return result;
        }
    }
    
    public class EnumerableByteFileCreateDTOMapper : IMapper<IEnumerable<ByteFileCreateDTO>, IEnumerable<FileCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableByteFileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<FileCreateDTO> Map(IEnumerable<ByteFileCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<ByteFileCreateDTO, FileCreateDTO>(x));
            return result;
        }
    }
}