using System.ComponentModel.DataAnnotations.Schema;

namespace GhandiBot.Data.Models
{
    [Table("Event")]
    public class Event
    {
        [Column("Name")]
        public string Name { get; set; }

        [Column("ServerId")]
        public ulong ServerId { get; set; }

        [Column("CommandIssuerId")]
        public ulong CommandIssuerId { get; set; }
    }
}
