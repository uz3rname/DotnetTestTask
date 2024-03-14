using System.ComponentModel.DataAnnotations.Schema;

namespace DotnetTestTask.Models
{
    public class Invoice : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Timestamp { get; set; }
        public Guid InvoiceCreatorId { get; set; }
        public InvoiceCreator InvoiceCreator { get; set; } = null!;
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = [];
    }
}