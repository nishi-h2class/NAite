using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("ItemFiles")]
    public class ItemFile
    {
        public int Id { get; set; }

        [Required]
        public string ItemId { get; set; } = null!;

        [Required]
        public string FileId { get; set; } = null!;

        public virtual Item Item { get; set; } = null!;

        public virtual File File { get; set; } = null!;
    }
}
