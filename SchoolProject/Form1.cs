using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject
{
    public partial class Form1 : Form
    {
        private PrintDocument printDocument1;
        public Form1()
        {
            InitializeComponent();

            // Initialize PrintDocument and attach the event
            printDocument1 = new PrintDocument();
            printDocument1.PrintPage += printDocument1_PrintPage;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;

            // Use Microsoft Print to PDF
            printDialog.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument1.PrinterSettings = printDialog.PrinterSettings;
                printDocument1.Print();
            }

        }












        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Font headerFont = new Font("Arial", 14, FontStyle.Bold);
            Font bodyFont = new Font("Arial", 12);
            int leftMargin = e.MarginBounds.Left;
            int rightMargin = e.MarginBounds.Right;
            int topMargin = e.MarginBounds.Top;
            int y = topMargin;

            Graphics g = e.Graphics;

            // ===== Invoice Title =====
            g.DrawString("INVOICE", headerFont, Brushes.Black, leftMargin, y);
            y += 30;

            // ===== First Line after Title =====
            g.DrawLine(Pens.Black, leftMargin, y, rightMargin, y);
            y += 10;

            // ===== Invoice Details =====
            g.DrawString("Invoice #1001", bodyFont, Brushes.Black, leftMargin, y);
            y += 25;
            g.DrawString("Date: " + DateTime.Now.ToShortDateString(), bodyFont, Brushes.Black, leftMargin, y);
            y += 30;

            g.DrawString("Student Name: Ali Ahmad", bodyFont, Brushes.Black, leftMargin, y);
            y += 25;
            g.DrawString("Grade: 6", bodyFont, Brushes.Black, leftMargin, y);
            y += 30;

            // ===== Items Header =====
            g.DrawString("Item                  Qty     Price", bodyFont, Brushes.Black, leftMargin, y);
            y += 25;

            // ===== Invoice Items =====
            g.DrawString("Tuition Fee           1       1000 EGP", bodyFont, Brushes.Black, leftMargin, y);
            y += 25;
            g.DrawString("Bus Fee               1       300 EGP", bodyFont, Brushes.Black, leftMargin, y);
            y += 30;

            // ===== Total =====
            g.DrawString("Total: 1300 EGP", new Font("Arial", 12, FontStyle.Bold), Brushes.Black, leftMargin, y);
            y += 40;

            // ===== Last Line after everything =====
            g.DrawLine(Pens.Black, leftMargin, y, rightMargin, y);
        }

    






    private void SaveInvoiceToFile()
        {
            string invoiceText = "";
            invoiceText += "INVOICE\n";
            invoiceText += "----------------------------\n";
            invoiceText += "Invoice #1001\n";
            invoiceText += "Date: " + DateTime.Now.ToShortDateString() + "\n";
            invoiceText += "Student Name: Ali Ahmad\n";
            invoiceText += "Grade: 6\n";
            invoiceText += "\n";
            invoiceText += "Item                  Qty     Price\n";
            invoiceText += "Tuition Fee           1       1000 EGP\n";
            invoiceText += "Bus Fee               1       300 EGP\n";
            invoiceText += "\n";
            invoiceText += "Total: 1300 EGP\n";
            invoiceText += "----------------------------\n";

            // Get the user's Documents folder
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // Create "School Invoices" folder inside Documents
            string folderPath = Path.Combine(documentsPath, "School Invoices");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Create file name with invoice number and date
            string fileName = "Invoice_1001_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";
            string fullPath = Path.Combine(folderPath, fileName);

            // Save to file
            File.WriteAllText(fullPath, invoiceText);

            // Notify user
            MessageBox.Show("Invoice saved to: " + fullPath, "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }









    }

    }

