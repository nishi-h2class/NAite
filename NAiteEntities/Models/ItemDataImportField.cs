using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("ItemDataImportFields")]
    public class ItemDataImportField
    {
        public int Id { get; set; }

        public string ItemDataImportId { get; set; } = null!;

        [StringLength(50)]
        public string? Tag { get; set; } = null!;

        public int Order { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ItemDataImport ItemDataImport { get; set; } = null!;
    }
}
