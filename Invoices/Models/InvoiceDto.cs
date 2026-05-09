using System;

namespace Invoices.Models
{
    public class InvoiceDto
    {
        public int Id { get; set; }
        public string UniqueCode { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}