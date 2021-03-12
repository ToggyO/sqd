namespace Squadio.DTO.Models.Invites
{
    public class InviteDTO : InviteSimpleDTO
    {
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
    }
}