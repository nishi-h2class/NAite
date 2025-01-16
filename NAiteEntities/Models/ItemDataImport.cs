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
    [Table("ItemDataImports")]
    public class ItemDataImport
    {
        public ItemDataImport()
        {
            ItemDataImportFields = new HashSet<ItemDataImportField>();
        }

        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        public string? UserId { get; set; } = null!;

        public bool? IsHeader { get; set; }

        [StringLength(255)]
        public string FileName { get; set; } = null!;

        [StringLength(255)]
        public string OriginalFileName { get; set; } = null!;

        [StringLength(50)]
        public string? FileType { get; set; } = null!;

        public int? Number { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }
        public DateTime? Reserved { get; set; }

        public DateTime? Imported { get; set; }

        public virtual User User { get; set; } = null!;

        public virtual ICollection<ItemDataImportField> ItemDataImportFields { get; set; }
    }
}
