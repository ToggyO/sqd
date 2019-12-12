using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class FileImageCreateDTOMapper: IMapper<FileImageCreateDTO, ResourceImageCreateDTO>
    {
        private readonly IMapper _mapper;

        public FileImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ResourceImageCreateDTO Map(FileImageCreateDTO item)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                item.File.CopyTo(stream);
                bytes = stream.ToArray();
            }

            return new ResourceImageCreateDTO
            {
                ContentType = item.File.ContentType,
                Bytes = bytes
            };
        }
    }
    
    public class EnumerableFileImageCreateDTOMapper : IMapper<IEnumerable<FileImageCreateDTO>, IEnumerable<ResourceImageCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableFileImageCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ResourceImageCreateDTO> Map(IEnumerable<FileImageCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<FileImageCreateDTO, ResourceImageCreateDTO>(x));
            return result;
        }
    }
}