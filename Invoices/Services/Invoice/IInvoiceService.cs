using System.Collections.Generic;
using Invoices.Models;

namespace Invoices.Services.Invoice
{
    public interface IInvoiceService
    {
        InvoiceResponse CreateInvoice(string customerName, decimal amount);
        InvoiceResponse GetInvoiceById(int id);
        InvoiceListResponse GetAllInvoices();
        InvoiceResponse UpdateInvoice(int id, string customerName, decimal amount);
        InvoiceResponse ChangeInvoiceStatus(int id, string newStatus);
        InvoiceResponse SendInvoice(int id);
        BaseResponse DeleteInvoice(int id);
    }
}