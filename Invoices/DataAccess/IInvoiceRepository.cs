using System.Collections.Generic;
using Invoices.Models;

namespace Invoices.DataAccess
{
    public interface IInvoiceRepository
    {
        InvoiceDto Create(string customerName, decimal amount);
        InvoiceDto GetById(int id);
        List<InvoiceDto> GetAll();
        InvoiceDto Update(int id, string customerName, decimal amount);
        InvoiceDto UpdateStatus(int id, string newStatus);
        void SoftDelete(int id);
    }
}