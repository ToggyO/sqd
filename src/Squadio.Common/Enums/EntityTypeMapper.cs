using System;
using Squadio.Domain.Enums;
using Mapper;

namespace Squadio.Common.Enums
{
    public class EntityTypeMapper : IMapper<InviteEntityType, EntityType>
    {
        private readonly IMapper _mapper;

        public EntityTypeMapper(IMapper mapper)
        {
            _mapper = mapper;
        }
    
        public EntityType Map(InviteEntityType item)
        {
            
            switch (item)
            {
                case InviteEntityType.Company:
                    return EntityType.Company;
                case InviteEntityType.Team:
                    return EntityType.Team;
                case InviteEntityType.Project:
                    return EntityType.Project;
                default:
                    throw new Exception("Error mapping enum");
            }
        }
    }
}