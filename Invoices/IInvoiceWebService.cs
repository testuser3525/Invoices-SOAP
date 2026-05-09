using System.ServiceModel;
using Invoices.Models;

namespace Invoices
{
    [ServiceContract]
    public interface IInvoiceWebService
    {
        [OperationContract]
        InvoiceResponse CreateInvoice(string customerName, decimal amount);

        [OperationContract]
        InvoiceResponse GetInvoiceById(int id);

        [OperationContract]
        InvoiceListResponse GetAllInvoices();

        [OperationContract]
        InvoiceResponse UpdateInvoice(int id, string customerName, decimal amount);

        [OperationContract]
        InvoiceResponse ChangeInvoiceStatus(int id, string newStatus);

        [OperationContract]
        InvoiceResponse SendInvoice(int id);

        [OperationContract]
        BaseResponse DeleteInvoice(int id);
    }
}