using Invoices.DataAccess;
using Invoices.Models;
using Invoices.Services.Logging;
using System;

namespace Invoices.Services.Invoice
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly ILoggerService _logger;

        public InvoiceService()
        {
            _repository = new InvoiceRepository();
            _logger = new FileLoggerService();
        }

        public InvoiceResponse CreateInvoice(string customerName, decimal amount)
        {
            try
            {
                var invoice = _repository.Create(customerName, amount);

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice created: {invoice.UniqueCode} for '{invoice.CustomerName}', Amount: {invoice.Amount}"
                );

                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(CreateInvoice), ex);

                return new InvoiceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public InvoiceResponse GetInvoiceById(int id)
        {
            try
            {
                var invoice = _repository.GetById(id);

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice fetched: Id={id}"
                );

                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(GetInvoiceById), ex);

                return new InvoiceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public InvoiceListResponse GetAllInvoices()
        {
            try
            {
                var invoices = _repository.GetAll();

                _logger.Info(
                    nameof(InvoiceService),
                    $"GetAllInvoices returned {invoices.Count} records"
                );

                return new InvoiceListResponse
                {
                    Success = true,
                    Invoices = invoices
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(GetAllInvoices), ex);

                return new InvoiceListResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public InvoiceResponse UpdateInvoice(int id, string customerName, decimal amount)
        {
            try
            {
                var invoice = _repository.Update(id, customerName, amount);

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice updated: Id={id}, CustomerName='{customerName}', Amount={amount}"
                );

                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(UpdateInvoice), ex);

                return new InvoiceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public InvoiceResponse ChangeInvoiceStatus(int id, string newStatus)
        {
            try
            {
                var invoice = _repository.UpdateStatus(id, newStatus);

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice status changed: Id={id}, NewStatus={newStatus}"
                );

                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(ChangeInvoiceStatus), ex);

                return new InvoiceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public InvoiceResponse SendInvoice(int id)
        {
            try
            {
                var invoice = _repository.UpdateStatus(id, "Sent");

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice sent: {invoice.UniqueCode} to customer '{invoice.CustomerName}'"
                );

                return new InvoiceResponse
                {
                    Success = true,
                    Invoice = invoice
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(SendInvoice), ex);

                return new InvoiceResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public BaseResponse DeleteInvoice(int id)
        {
            try
            {
                _repository.SoftDelete(id);

                _logger.Info(
                    nameof(InvoiceService),
                    $"Invoice soft deleted: Id={id}"
                );

                return new BaseResponse
                {
                    Success = true
                };
            }
            catch (Exception ex)
            {
                _logger.Error(nameof(InvoiceService), nameof(DeleteInvoice), ex);

                return new BaseResponse
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}