namespace Invoices.Models
{
    public enum InvoiceStatus
    {
        Draft = 0,
        Issued = 1,
        Sent = 2,
        Paid = 3,
        Cancelled = 4
    }
}