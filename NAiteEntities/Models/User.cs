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
    [Table("Users")]
    public class User
    {
        [Key]
        [StringLength(32)]
        public string Id { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LoginId { get; set; } = null!;

        [Required]
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Authority { get; set; } = null!;

        [Required]
        public bool IsNotifyUpdateData { get; set; } = false;

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }
    }
}
