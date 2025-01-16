using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Responses;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class FileParams : QueryStringParameters
    {
        public FileParams()
        {
            OrderBy = nameof(NAiteEntities.Models.File.Name);
        }

        public string? Keyword { get; set; } = string.Empty;
    }
}
