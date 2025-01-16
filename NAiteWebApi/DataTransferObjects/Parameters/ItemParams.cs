using NAiteEntities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;
using NAiteWebApi.DataTransferObjects.Responses;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemParams
    {
    }

    public class UpdateItemParams
    {
        public string Id { get; set; } = null!;

        [StringLength(500)]
        public string? ValueText { get; set; } = null!;

        public int? ValueInt { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? ValueDecimal { get; set; }

        public DateTime? ValueDateTime { get; set; }

        public FileRes[]? Files { get; set; } 

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
