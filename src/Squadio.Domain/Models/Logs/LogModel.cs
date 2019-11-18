using System;

namespace Squadio.Domain.Models.Logs
{
    public class LogModel
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTime Date { get; set; }
        public string Exception { get; set; }
    }
}