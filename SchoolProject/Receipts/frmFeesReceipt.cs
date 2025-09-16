using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Receipts
{
    public partial class frmFeesReceipt : Form
    {
        private int _paymentID;
        public frmFeesReceipt(int paymentID)
        {
            InitializeComponent(); _paymentID = paymentID;
            LoadReceipt();
        }

        private void LoadReceipt()
        {
            // 1. Get school info
            var schoolInfo = clsSchoolInfo.GetSchoolInfo();

            string schoolName = schoolInfo?.SchoolName ?? "School Name";
            string schoolContact = $"Address: {schoolInfo?.Address}\nPhone: {schoolInfo?.Phone}\nEmail: {schoolInfo?.Email}";

            // 2. Get payment info
            DataTable dt = clsReceipts.GetReceiptsByTuitionFee(_paymentID);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Payment not found!");
                return;
            }

            DataRow row = dt.Rows[0];

            // Convert enum PaymentMode to string
            string paymentModeText = ((clsTuitionPayment.enPaymentType)Convert.ToInt32(row["PaymentMode"])).ToString();

            // Calculate remaining balance
            decimal totalFees = Convert.ToDecimal(row["TotalFees"]);
            decimal paidAmount = Convert.ToDecimal(row["PaidAmount"]);
            decimal remainingBalance = totalFees - paidAmount;

            // 3. Build receipt text
            StringBuilder receiptText = new StringBuilder();
            receiptText.AppendLine($"--- {schoolName} ---");
            receiptText.AppendLine(schoolContact);
            receiptText.AppendLine("----------------------");
            receiptText.AppendLine($"Student: {row["FullName"]}");
            receiptText.AppendLine($"TuitionFeeID: {row["TuitionFeeID"]}");
            receiptText.AppendLine($"Payment Date: {Convert.ToDateTime(row["CreatedDate"]):dd/MM/yyyy}");
            receiptText.AppendLine($"Payment Type: {paymentModeText}");
            receiptText.AppendLine($"Amount Paid: {paidAmount:0.00}");
            receiptText.AppendLine($"Remaining Balance: {remainingBalance:0.00}");
            receiptText.AppendLine("----------------------");

            // 4. Show in a TextBox (multiline) or Label
            txtReceipt.Text = receiptText.ToString();
        }



        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, args) =>
            {
                args.Graphics.DrawString(txtReceipt.Text, new Font("Arial", 12), Brushes.Black, new PointF(50, 50));
            };
            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = pd;
            preview.ShowDialog();
        }
    }
}
