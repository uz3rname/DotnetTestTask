using DotnetTestTask.Components.Pages;
using DotnetTestTask.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetTestTask.Controllers
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IXlsxGenerator _xlsxGenerator;

        public FileController(
            IInvoiceService invoiceService,
            IXlsxGenerator xlsxGenerator
        ) : base()
        {
            _invoiceService = invoiceService;
            _xlsxGenerator = xlsxGenerator;
        }

        [HttpGet("Invoices/{Id}/Download")]
        public async Task<IActionResult> DownloadInvoice(Guid Id)
        {
            var invoice = await _invoiceService.GetById(Id);
            if (invoice is null)
            {
                return NotFound();
            }
            var stream = _xlsxGenerator.CreateInvoiceDocument(invoice);
            string fileName = $"{invoice.Id}.xlsx";
            return File(stream, "application/octet-stream", fileName);
        }
    }
}