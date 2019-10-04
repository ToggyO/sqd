namespace Squadio.Common.Settings
{
    public class EmailSettingsModel
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}