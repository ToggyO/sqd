namespace Squadio.Common.Models.Rabbit
{
    public class RabbitConnectionModel
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public string ConnectionString => 
            $"host={Host};username={Username};password={Password}";
        public string ConnectionStringWithPort => 
            $"host={Host}:{Port};username={Username};password={Password}";
    }
}