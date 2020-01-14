using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapper;

namespace Squadio.DTO.Resources
{
    public class FormFileCreateDTOMapper: IMapper<FormFileCreateDTO, FileCreateDTO>
    {
        private readonly IMapper _mapper;

        public FormFileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }

        public FileCreateDTO Map(FormFileCreateDTO item)
        {
            var result = new FileCreateDTO
            {
                ContentType = item.File.ContentType,
                Stream = new MemoryStream()
            };

            item.File.CopyTo(result.Stream);

            return result;
        }
    }
    
    public class EnumerableFormFileCreateDTOMapper : IMapper<IEnumerable<FormFileCreateDTO>, IEnumerable<FileCreateDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableFormFileCreateDTOMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<FileCreateDTO> Map(IEnumerable<FormFileCreateDTO> items)
        {
            var result = items.Select(x => _mapper.Map<FormFileCreateDTO, FileCreateDTO>(x));
            return result;
        }
    }
}