using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NAiteEntities.Models
{
    [Table("ItemDatas")]
    public class ItemData
    {
        public int Id { get; set; }

        public string Code { get; set; } = null!;

        public string Type { get; set; } = null!;

        public int Quantity { get; set; }

        public DateTime Date { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public DateTime? Deleted { get; set; }
    }
}
