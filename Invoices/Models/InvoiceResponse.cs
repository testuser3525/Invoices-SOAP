using System.Collections.Generic;

namespace Invoices.Models
{
    public class InvoiceResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public InvoiceDto Invoice { get; set; }
    }

    public class InvoiceListResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<InvoiceDto> Invoices { get; set; }
    }

    public class BaseResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}