using Microsoft.EntityFrameworkCore;

namespace DotnetTestTask.Models
{
    [Index(nameof(StoreItem.Name), IsUnique = true)]
    public class StoreItem : BaseEntity
    {
        public string Name { get; set; } = "";
        public int Amount { get; set; }
        public ICollection<InvoiceItem> InvoiceItems { get; } = [];
    }
}