using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

using MEI.AbbVie.Infrastructure.Commands.Expenses;
using MEI.Core.Commands;
using MEI.Core.DomainModels.Common;
using MEI.Core.DomainModels.Travel;
using MEI.SPDocuments;
using MEI.SPDocuments.TypeCodes;
using MEI.Travel.Commands;

using Microsoft.Extensions.Logging;

using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.Pdf.Parsing;

namespace MEI.Travel.Services
{
    public class InvoiceService : IInvoiceService
    {
        private const string invoiceTemplateFileName = "Documents\\AdvancedUInvoiceTemplate.pdf";

        private readonly ICommandProcessor _clientCommands;
        private readonly ILogger<InvoiceService> _logger;
        private readonly ISPDocuments _sharePointDocuments;
        private readonly IDocumentFactory _documentFactory;

        public InvoiceService(ILogger<InvoiceService> logger, ICommandProcessor clientCommands, ISPDocuments sharePointDocuments, IDocumentFactory documentFactory)
        {
            _logger = logger;
            _clientCommands = clientCommands;
            _sharePointDocuments = sharePointDocuments;
            _documentFactory = documentFactory;
        }

        public void AddInvoiceToSharePoint(Invoice invoice, Client client)
        {
            // Create invoice document
            var stream = GenerateInvoicePdfStream(invoice);

            // Save invoice document to SharePoint
            UploadInvoicePdfToSharepoint(stream, invoice, client);
        }

        private MemoryStream GenerateInvoicePdfStream(Invoice invoice)
        {
            var stream = InsertInvoiceLineItemsToPdf(invoice);

            return InsertInvoiceDetailsToPdf(invoice, stream);
        }

        private MemoryStream InsertInvoiceDetailsToPdf(Invoice invoice, MemoryStream stream)
        {
            var invoiceId = invoice.Id.ToString().PadLeft(7, '0');

            //Load the PDF document.
            var loadedDocument = new PdfLoadedDocument(stream);

            // Get the loaded form.
            var loadedForm = loadedDocument.Form;

            // Get the loaded form field and modify the properties.
            var invoiceIdTextField = loadedForm.Fields["InvoiceId"] as PdfLoadedTextBoxField;
            var submissionDateTextField = loadedForm.Fields["SubmissionDate"] as PdfLoadedTextBoxField;
            invoiceIdTextField.Text = invoiceId;
            submissionDateTextField.Text = DateTime.Now.ToString("MM/dd/yyyy");

            // Remove editablilty 
            loadedDocument.Form.Flatten = true;

            // Save the document.
            loadedDocument.Save(stream);

            // close the document
            loadedDocument.Close(true);

            return stream;
        }

        private MemoryStream InsertInvoiceLineItemsToPdf(Invoice invoice)
        {
            var location = System.Reflection.Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            string[] paths = { directory, invoiceTemplateFileName };
            var template = Path.Combine(paths);

            // Load the existing PDF template.
            var fileStream = new FileStream(template, FileMode.Open, FileAccess.Read);
            var loadedDocument = new PdfLoadedDocument(fileStream);

            // Create a new PDF document to apply the template to
            var document = new PdfDocument();
            document.ImportPageRange(loadedDocument, 0, loadedDocument.Pages.Count - 1);

            // Add the page
            var page = document.Pages[0];

            // Create the graphics
            var graphics = page.Graphics;

            //Create a DataTable
            var dataTable = new DataTable();

            //Add columns to the DataTable
            dataTable.Columns.Add("DETAILS");
            dataTable.Columns.Add("AMOUNT");

            //Add rows to the DataTable
            foreach (var item in invoice.LineItems)
            {
                dataTable.Rows.Add(string.Format("Agency Service Fee for {0}", item.AgencyService.Name), item.Amount.ToString("C"));
            }

            // Create the grid style
            var gridStyle = new PdfGridStyle();
            gridStyle.TextBrush = new PdfSolidBrush(new PdfColor(Color.FromArgb(255, 96, 90, 33)));
            gridStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 11); 
            gridStyle.CellPadding = new PdfPaddings(0, 3, 10, 3);

            // Create the header styles
            var headerStyle = new PdfGridCellStyle();
            headerStyle.Font = new PdfStandardFont(PdfFontFamily.Helvetica, 12, PdfFontStyle.Bold);
            headerStyle.Borders.All = PdfPens.Transparent;

            // Create a new PdfGrid instance
            var grid = new PdfGrid();

            // Assign data source to grid
            grid.DataSource = dataTable;

            // Assign the grid and header style to the grid
            grid.Style = gridStyle;
            grid.Headers.ApplyStyle(headerStyle);

            // Create the alignment formats
            var columnFormat = new PdfStringFormat();
            columnFormat.Alignment = PdfTextAlignment.Left;
            columnFormat.LineAlignment = PdfVerticalAlignment.Bottom;

            // Assign column width and formats
            grid.Columns[0].Width = 300;
            grid.Columns[0].Format = columnFormat;
            grid.Columns[1].Width = 180;
            grid.Columns[1].Format = columnFormat;

            // Create the default cell styles
            var bottomBorder = new PdfPen(new PdfColor(Color.FromArgb(255, 189, 181, 83)));
            var defaultCellStyle = new PdfGridCellStyle();
            defaultCellStyle.Borders.All = PdfPens.Transparent;
            defaultCellStyle.Borders.Bottom = bottomBorder;

            // Assign default cell style to each cell
            foreach (var row in grid.Rows)
            {
                row.Cells[0].Style = defaultCellStyle;
                row.Cells[1].Style = defaultCellStyle;
            }

            // Add the row for totals
            var totals = new PdfGridRow(grid);
            grid.Rows.Add(totals);

            // Create the styles for the totals row
            var totalsCellStyle = new PdfGridCellStyle();
            totalsCellStyle.Borders.All = PdfPens.Transparent;
            totalsCellStyle.CellPadding = new PdfPaddings(0, 5, 10, 3);

            // Add the Cell values, format and styles
            totals.Cells[0].Value = "TOTAL";
            totals.Cells[0].StringFormat = new PdfStringFormat {Alignment = PdfTextAlignment.Right, LineAlignment = PdfVerticalAlignment.Bottom};
            totals.Cells[0].Style = totalsCellStyle;
            totals.Cells[1].Value = invoice.LineItems.Sum(l => l.Amount).ToString("C");
            totals.Cells[1].StringFormat = new PdfStringFormat {Alignment = PdfTextAlignment.Left, LineAlignment = PdfVerticalAlignment.Bottom};
            totals.Cells[1].Style = totalsCellStyle;

            // Draw the grid
            grid.Draw(page, 64, 300);

            //Save the new document into a memory stream
            var stream = new MemoryStream();
            document.Save(stream);

            //Close the template
            loadedDocument.Close(true);
            document.Close(true);

            return stream;
        }

        private void UploadInvoicePdfToSharepoint(MemoryStream stream, Core.DomainModels.Travel.Invoice invoice, Client client)
        {
            try
            {
                var companyCode = (Company)Enum.Parse(typeof(Company), client.SharePointCompanyCode);
                var properties = new object [] {invoice.Id, invoice.ConsultantId, invoice.EventName, stream.ToArray(), ".pdf", companyCode};
                var document = (SPDocuments.Document.TravelAgencyServiceInvoice)_documentFactory.CreateDocument(SPDocumentType.TravelAgencyServiceInvoice);
                document.Setup(properties);
                _sharePointDocuments.SaveWebDocumentToWebShare(document);

                // write log pertaining to invoice add
                _logger.LogInformation(string.Format("Travel Invoice for {0} - {1} - {2} was uploaded to SharePoint", client.Name, invoice.EventName, invoice.ConsultantName));
            }
            catch (Exception e)
            {
                var message = string.Format("Travel Invoice for {0} - {1} - {2} was uploaded to SharePoint: {3}", client.Name, invoice.EventName, invoice.ConsultantName, e.Message);
                _logger.LogError(e, message);
            }
        }

    }
}