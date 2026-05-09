# Invoice SOAP Web Service

A .NET Framework 4.8 WCF SOAP service for managing invoices. Uses ADO.NET with SQL Server stored procedures, no ORM.

### Screenshot
![postman](https://i.imgur.com/Ao7G09U.png)
![db](https://i.imgur.com/dUj0cji.png)
---

## Tech

- .NET Framework 4.8
- WCF (SOAP, basicHttpBinding)
- ADO.NET (SqlConnection, SqlCommand)
- SQL Server / LocalDB
- Stored procedures only, no EF

---

## How to Run

### 1. Set up the database

Open SSMS, copy the contents of `sql/setup.sql` and run it.

This creates the `InvoiceDB` database, `Invoices` table, all stored procedures, and inserts 5 seed records.

### 2. Update the connection string

Open `Web.config` and update if needed:

```xml
<add name="InvoiceDB"
     connectionString="Server=localhost;Database=InvoiceDB;Trusted_Connection=True;"
     providerName="System.Data.SqlClient" />
```

Not on LocalDB? Find your instance name with `SELECT @@SERVERNAME;` in SSMS and update `Server=` accordingly.

### 3. Run

Press F5 in Visual Studio. Then verify the service is up:

```
http://localhost:{port}/InvoiceWebService.svc?wsdl
```

---

## Testing

Import `SOAP.postman_collection.json` into Postman. All 7 requests are preconfigured with correct headers and body.

All requests are POST to `http://localhost:{port}/InvoiceWebService.svc` with:

```
Content-Type: text/xml
SOAPAction: http://tempuri.org/IInvoiceWebService/{OperationName}
```

Example (Get All Invoices):

```xml
<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                  xmlns:tem="http://tempuri.org/">
   <soapenv:Header/>
   <soapenv:Body>
      <tem:GetAllInvoices/>
   </soapenv:Body>
</soapenv:Envelope>
```

---

## Available Operations

| Operation | Description |
|---|---|
| CreateInvoice | Creates a new invoice with status Draft |
| GetAllInvoices | Returns all non-deleted invoices |
| GetInvoiceById | Returns a single invoice by Id |
| UpdateInvoice | Updates CustomerName and Amount (blocked on Paid/Cancelled) |
| ChangeInvoiceStatus | Moves invoice to a new status (validates transition) |
| SendInvoice | Moves invoice from Issued to Sent and logs the action |
| DeleteInvoice | Soft delete, sets IsDeleted = 1, record stays in DB |

---

## Status Flow

```
Draft -> Issued -> Sent -> Paid
  |         |       |
  +---------+-------+--> Cancelled
```

Paid and Cancelled are terminal, no transitions out of them.

---

## Response Structure

Every response follows the same shape. Always check Success first.

Success:

```xml
<InvoiceResponse>
   <Success>true</Success>
   <ErrorMessage i:nil="true"/>
   <Invoice>
      <Id>1</Id>
      <UniqueCode>INV-20260507-3847</UniqueCode>
      <CustomerName>Alice Johnson</CustomerName>
      <Amount>1500.00</Amount>
      <Status>Draft</Status>
      <CreatedAt>2026-05-07T10:00:00</CreatedAt>
      <UpdatedAt i:nil="true"/>
      <IsDeleted>false</IsDeleted>
   </Invoice>
</InvoiceResponse>
```

Failure:

```xml
<InvoiceResponse>
   <Success>false</Success>
   <ErrorMessage>Invalid status transition: Paid -> Draft is not allowed.</ErrorMessage>
   <Invoice i:nil="true"/>
</InvoiceResponse>
```

---

## Logs

Logs are written to `logs/invoices-{date}.log`. The file is created at runtime so Visual Studio may not show it in Solution Explorer. Check it directly in File Explorer at:

```
{project root}/Invoices/logs/
```
## Notes

- Error handling is done per method in `InvoiceService.cs` rather than via global error handling. This is intentional, the project is simple enough that method-level try/catch keeps things explicit and easy to follow without adding middleware or global exception filters.
- Make sure you have set correct API URL in postaman
- Make sure Web.config has correct db connection string
