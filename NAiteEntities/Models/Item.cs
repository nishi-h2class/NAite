using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace NAiteEntities.Models
{
    [Table("Items")]
    public class Item
    {
        public Item()
        {
            ItemFiles = new HashSet<ItemFile>();
        }

        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        public string ItemRowId { get; set; } = null!;

        public string ItemFieldId { get; set; } = null!;

        [StringLength(500)]
        public string? ValueText { get; set; } = null!;

        public int? ValueInt { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ValueDecimal { get; set; }

        public DateTime? ValueDateTime { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ItemRow ItemRow { get; set; } = null!;

        public virtual ItemField ItemField { get; set; } = null!;

        public virtual ICollection<ItemFile> ItemFiles { get; set; }
    }
}
