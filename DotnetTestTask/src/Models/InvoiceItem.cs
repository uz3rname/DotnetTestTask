namespace DotnetTestTask.Models
{
    public class InvoiceItem : BaseEntity
    {
        public int Amount { get; set; }

        public Guid StoreItemId { get; set; }
        public StoreItem StoreItem { get; set; } = null!;
        public Guid InvoiceId { get; set; }
        public Invoice Invoice { get; set; } = null!;
    }
}