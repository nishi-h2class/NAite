using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("Files")]
    public class File
    {
        public File()
        {
            ItemFiles = new HashSet<ItemFile>();
        }

        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = null!;

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }

        public virtual ICollection<ItemFile> ItemFiles { get; set; }
    }
}
