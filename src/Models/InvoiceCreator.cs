using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Models
{
    [Index(nameof(InvoiceCreator.Name), IsUnique = true)]
    public class InvoiceCreator : BaseEntity
    {
        public string Name { get; set; } = "";
        public ICollection<Invoice> Invoices { get; } = new List<Invoice>();
    }
}