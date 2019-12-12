using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class FileCreateDTOMapper: IMapper<FileCreateDTO, ResourceCreateDTO>
    {
        private readonly IMapper _mapper;

        public FileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public ResourceCreateDTO Map(FileCreateDTO item)
        {
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                item.File.CopyTo(stream);
                bytes = stream.ToArray();
            }

            return new ResourceCreateDTO
            {
                ContentType = item.File.ContentType,
                Bytes = bytes
            };
        }
    }
    
    public class EnumerableFileCreateDTOMapper : IMapper<IEnumerable<FileCreateDTO>, IEnumerable<ResourceCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableFileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<ResourceCreateDTO> Map(IEnumerable<FileCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<FileCreateDTO, ResourceCreateDTO>(x));
            return result;
        }
    }
}