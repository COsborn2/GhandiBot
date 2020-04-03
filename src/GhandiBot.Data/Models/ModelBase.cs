using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GhandiBot.Data.Models
{
    public class ModelBase
    {
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
    }
}