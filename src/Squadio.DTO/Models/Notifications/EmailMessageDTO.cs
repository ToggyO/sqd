namespace Squadio.DTO.Models.Notifications
{
    public class EmailMessageDTO
    {
        public string[] ToEmail { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public string FromName { get; set; }
        public bool IsHtml { get; set; }
    }
}