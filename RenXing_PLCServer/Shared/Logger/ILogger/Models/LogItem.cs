using System.ComponentModel.DataAnnotations;

namespace Shared.Logger.ILogger.Models
{
    public class LogItem
    {
        [Key]
        public int Id { get; set; }

        public string Date { get; set; } = string.Empty;

        public string Grade { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? Source { get; set; }
    }
}
