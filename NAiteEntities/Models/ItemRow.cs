using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("ItemRows")]
    public class ItemRow
    {
        public ItemRow()
        {
            Items = new HashSet<Item>();
        }

        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        [Required]
        public int DefaultOrder { get; set; } = 0;

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ICollection<Item> Items { get; set; }
    }
}
