using System.ComponentModel.DataAnnotations.Schema;

namespace GhandiBot.Data.Models
{
    [Table("FeatureOverride")]
    public class FeatureOverride : ModelBase
    {
        [Column("ServerId")]
        public ulong ServerId { get; set; }
        
        [Column("CommandIssuerId")]
        public ulong CommandIssuerId { get; set; }
        
        [Column("CommandType")]
        public string CommandType { get; set; }
    }
}