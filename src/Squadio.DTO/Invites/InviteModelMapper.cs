using System.Collections.Generic;
using System.Linq;
using Mapper;
using Squadio.Domain.Models.Invites;

namespace Squadio.DTO.Invites
{
    public class InviteModelMapper: IMapper<InviteModel, InviteDTO>
    {
        private readonly IMapper _mapper;

        public InviteModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public InviteDTO Map(InviteModel item)
        {
            return new InviteDTO
            {
                Email = item.Email,
                CreatedDate = item.CreatedDate
            };
        }
    }
    
    public class EnumerableInviteModelMapper : IMapper<IEnumerable<InviteModel>, IEnumerable<InviteDTO>>
    {
        private readonly IMapper _mapper;

        public EnumerableInviteModelMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public IEnumerable<InviteDTO> Map(IEnumerable<InviteModel> items)
        {
            var result = items.Select(x => _mapper.Map<InviteModel, InviteDTO>(x));
            return result;
        }
    }
}