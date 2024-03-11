using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetTestTask.Models
{
    public class BaseEntity
    {
        [Key]
        [Column(TypeName = "uuid")]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}