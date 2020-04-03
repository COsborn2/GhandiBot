using System.ComponentModel.DataAnnotations.Schema;

namespace GhandiBot.Data.Models
{
    [Table("Logs")]
    public class Logs
    {
        [Column("Id")]
        public int Id { get; set; }
        
        [Column("Date")]
        public string Date { get; set; }
        
        [Column("Level")]
        public string Level { get; set; }
        
        [Column("Message")]
        public string Message { get; set; }
        
        [Column("CallSite")]
        public string CallSite { get; set; }
        
        [Column("Exception")]
        public string Exception { get; set; }
        
        [Column("Logger")]
        public string Logger { get; set; }
    }
}