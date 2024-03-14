using DotnetTestTask.Models;
using LargeXlsx;

namespace DotnetTestTask.Services
{
    public interface IXlsxGenerator
    {
        public Stream CreateInvoiceDocument(Invoice invoice);
    }

    public class XlsxGenerator : IXlsxGenerator
    {
        public Stream CreateInvoiceDocument(Invoice invoice)
        {
            var stream = new MemoryStream();
            var writer = new XlsxWriter(stream);
            writer.BeginWorksheet("Invoice")
                .BeginRow().AddMergedCell(1, 2).Write($"Created at: {invoice.Timestamp.ToString()}")
                .BeginRow().AddMergedCell(1, 2).Write($"Creator: {invoice.InvoiceCreator.Name}")
                .BeginRow().Write("Item").Write("Item");
            foreach (var item in invoice.InvoiceItems)
            {
                writer.BeginRow().Write(item.StoreItem.Name).Write(item.Amount);
            }
            writer.Dispose();
            stream.Position = 0;
            return stream;
        }
    }
}