using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Receipts
{
    public partial class ReceiptDemo : Form
    {
        public ReceiptDemo()
        {
            InitializeComponent(); GenerateReceiptPDF();
        }

        private void GenerateReceiptPDF()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                           $"Receipt_Demo.pdf");

            Document doc = new Document(PageSize.A4, 40, 40, 40, 40);
            PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
            doc.Open();

            // --- Fonts ---
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);

            // --- Header ---
            doc.Add(new Paragraph("School Fee Receipt", titleFont));
            doc.Add(new Paragraph("\n"));

            // --- Details (hardcoded) ---
            doc.Add(new Paragraph("Date: 15/01/2050   Receipt No: 12345", normalFont));
            doc.Add(new Paragraph("Student Name: Cyrus Ortiz", normalFont));
            doc.Add(new Paragraph("Class: 5th Grade", normalFont));
            doc.Add(new Paragraph("School: My School Name", normalFont));
            doc.Add(new Paragraph("\n"));

            // --- Table of fees ---
            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 3, 2, 3, 3 });

            // Headers
            table.AddCell("Fee Type");
            table.AddCell("Amount (USD)");
            table.AddCell("Payment Mode");
            table.AddCell("Date of Payment");

            // Rows (hardcoded)
            table.AddCell("Tuition Fee");
            table.AddCell("$500.00");
            table.AddCell("Bank Transfer");
            table.AddCell("15/01/2050");

            table.AddCell("Activity Fee");
            table.AddCell("$100.00");
            table.AddCell("Bank Transfer");
            table.AddCell("15/01/2050");

            table.AddCell("Library Fee");
            table.AddCell("$50.00");
            table.AddCell("Bank Transfer");
            table.AddCell("15/01/2050");

            table.AddCell("Miscellaneous Fee");
            table.AddCell("$30.00");
            table.AddCell("Bank Transfer");
            table.AddCell("15/01/2050");

            doc.Add(table);
            doc.Add(new Paragraph("\n"));

            // --- Summary ---
            doc.Add(new Paragraph("Paid By: Talia Jacobs", normalFont));
            doc.Add(new Paragraph("Amount Paid: $680.00   Outstanding Balance: $0.00", normalFont));
            doc.Add(new Paragraph("\n"));

            // --- Footer ---
            doc.Add(new Paragraph("Thank you for your payment. Should you have any queries, feel free to reach out.", normalFont));
            doc.Add(new Paragraph("Contact: school@email.com | +1 555-123-4567", normalFont));

            doc.Close();

            MessageBox.Show($"Receipt generated: {filePath}");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                   $"Receipt_Demo.pdf");

            if (File.Exists(filePath))
            {
                System.Diagnostics.Process.Start(filePath);  // Opens in default PDF viewer
            }
            else
            {
                MessageBox.Show("Please generate the receipt first.");
            }

        }
    }
}
