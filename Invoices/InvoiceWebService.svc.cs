using Invoices.Models;
using Invoices.Services.Invoice;

namespace Invoices
{
    public class InvoiceWebService : IInvoiceWebService
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceWebService()
        {
            _invoiceService = new InvoiceService();
        }

        public InvoiceResponse CreateInvoice(string customerName, decimal amount)
        {
            return _invoiceService.CreateInvoice(customerName, amount);
        }

        public InvoiceResponse GetInvoiceById(int id)
        {
            return _invoiceService.GetInvoiceById(id);
        }

        public InvoiceListResponse GetAllInvoices()
        {
            return _invoiceService.GetAllInvoices();
        }

        public InvoiceResponse UpdateInvoice(int id, string customerName, decimal amount)
        {
            return _invoiceService.UpdateInvoice(id, customerName, amount);
        }

        public InvoiceResponse ChangeInvoiceStatus(int id, string newStatus)
        {
            return _invoiceService.ChangeInvoiceStatus(id, newStatus);
        }

        public InvoiceResponse SendInvoice(int id)
        {
            return _invoiceService.SendInvoice(id);
        }

        public BaseResponse DeleteInvoice(int id)
        {
            return _invoiceService.DeleteInvoice(id);
        }
    }
}