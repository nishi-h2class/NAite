using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("ItemFields")]
    public class ItemField
    {
        public ItemField()
        {
            Items = new HashSet<Item>();
        }

        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = null!;

        [StringLength(50)]
        public string? FixedFieldType { get; set; } = null!;

        [Required]
        public bool IsSearch { get; set; }

        [Required]
        public int Order { get; set; } = 0;

        [StringLength(10)]
        public string? ExcelColumnName { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
