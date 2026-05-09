using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Invoices.Models;

namespace Invoices.DataAccess
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly string _connectionString;

        public InvoiceRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["InvoiceDB"].ConnectionString;
        }

        public InvoiceDto Create(string customerName, decimal amount)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_create", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerName", customerName);
                cmd.Parameters.AddWithValue("@Amount", amount);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);
                }
            }

            return null;
        }

        public InvoiceDto GetById(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_get_by_id", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);
                }
            }

            return null;
        }

        public List<InvoiceDto> GetAll()
        {
            var invoices = new List<InvoiceDto>();

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_get_all", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        invoices.Add(MapRow(reader));
                }
            }

            return invoices;
        }

        public InvoiceDto Update(int id, string customerName, decimal amount)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_update", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@CustomerName", customerName);
                cmd.Parameters.AddWithValue("@Amount", amount);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);
                }
            }

            return null;
        }

        public InvoiceDto UpdateStatus(int id, string newStatus)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_update_status", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceId", id);
                cmd.Parameters.AddWithValue("@NewStatus", newStatus);

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapRow(reader);
                }
            }

            return null;
        }

        public void SoftDelete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand("sp_invoice_soft_delete", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private static InvoiceDto MapRow(SqlDataReader reader)
        {
            return new InvoiceDto
            {
                Id = (int)reader["Id"],
                UniqueCode = reader["UniqueCode"].ToString(),
                CustomerName = reader["CustomerName"].ToString(),
                Amount = (decimal)reader["Amount"],
                Status = reader["Status"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = reader["UpdatedAt"] == DBNull.Value
                                   ? (DateTime?)null
                                   : (DateTime)reader["UpdatedAt"],
                IsDeleted = (bool)reader["IsDeleted"]
            };
        }
    }
}