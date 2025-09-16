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
    public partial class frmFeesReceiptViewer : Form
    {
        private int _paymentID;
        private Panel pnlReceipt;
        private Button btnSaveImage;
        private Button btnPrint;

        public frmFeesReceiptViewer(int paymentID)
        {
            InitializeComponent(); InitializeForm(); _paymentID = paymentID; RenderReceiptInPanel(pnlReceipt, _paymentID); // <-- call it here
        }

        private void InitializeForm()
        {
            this.Text = "Receipt Preview";
            this.Width = 650;  // made it a bit wider
            this.Height = 800;
            this.StartPosition = FormStartPosition.CenterScreen;

            int margin = 10;
            pnlReceipt = new Panel
            {
                Width = this.ClientSize.Width - margin * 2,
                Height = 750,
                Location = new Point(margin, 20),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                BackColor = Color.White
            };

            this.Controls.Add(pnlReceipt);

            // --- Buttons inside the panel ---
            btnSaveImage = new Button
            {
                Text = "Save as Image",
                Width = 120,
                Height = 30,
                Location = new Point(20, pnlReceipt.Height - 50) // 50px from bottom
            };
            btnSaveImage.Click += btnClick_Click; // attach your existing save logic
            pnlReceipt.Controls.Add(btnSaveImage);

            btnPrint = new Button
            {
                Text = "Print",
                Width = 80,
                Height = 30,
                Location = new Point(160, pnlReceipt.Height - 50) // same bottom line
            };
            btnPrint.Click += btnPrintIt_Click; // attach your existing print logic
            pnlReceipt.Controls.Add(btnPrint);
        }

        private void RenderReceiptInPanel(Panel pnl, int tuitionFeeID)
        {
            pnl.Controls.Clear();
            pnl.BackColor = Color.White;

            // Load payment
            clsTuitionPayment payment = clsTuitionPayment.FindPaymentByTuitionFeeID(tuitionFeeID);
            if (payment == null)
            {
                MessageBox.Show("Payment not found!");
                return;
            }
            clsStudent student = payment.StudentInfo;
            clsEnrollment enrollment = clsEnrollment.FindByID(payment.EnrollmentID);
            clsSchoolInfo school = clsSchoolInfo.GetSchoolInfo();

            // Get fee type as DataTable
            //DataTable dtFeeType = clsFeeType.FindByIDAsDataTable(payment.FeeTypeID);

            string feeTypeName = "Tuition Fees";
            //if (dtFeeType != null && dtFeeType.Rows.Count > 0)
            //{
            //    feeTypeName = dtFeeType.Rows[0]["FeeTypeName"].ToString();
            //}

            pnl.Paint += (s, e) =>
            {
                Graphics g = e.Graphics;
                int y = 20;
                int leftMargin = 20;

                Font titleFont = new Font("Arial", 16, FontStyle.Bold);
                Font headerFont = new Font("Arial", 12, FontStyle.Bold);
                Font normalFont = new Font("Arial", 12);

                // Reserve space for buttons at bottom
                int bottomMargin = 70; // enough to leave buttons visible
                int maxY = pnl.Height - bottomMargin;

                // --- Header ---
                g.DrawString("School Fee Receipt", titleFont, Brushes.Black, leftMargin + 100, y);
                y += 40;

                g.DrawString($"Date: {DateTime.Now:dd/MM/yyyy}", normalFont, Brushes.Black, leftMargin, y);
                g.DrawString($"Receipt No: {clsReceipts.GenerateReceiptNumber()}", normalFont, Brushes.Black, leftMargin + 250, y);
                y += 25;

                g.DrawString($"Student Name: {enrollment.StudentFullName}", normalFont, Brushes.Black, leftMargin, y);
                y += 25;
                g.DrawString($"Class: {clsClass.FindClassByID(enrollment?.ClassID ?? -1)?.ClassName}", normalFont, Brushes.Black, leftMargin, y);
                y += 25;
                g.DrawString($"School: {school?.SchoolName ?? "Unknown School"}", normalFont, Brushes.Black, leftMargin, y);
                y += 40;

                // --- Table ---
                int tableX = leftMargin;
                int tableY = y;
                int[] colWidths = { 150, 100, 150, 180 };
                int rowHeight = 25;

                string[] headers = { "Fee Type", "Amount", "Payment Mode", "Date of Payment" };
                Rectangle headerRect = new Rectangle(tableX, tableY, colWidths.Sum(), rowHeight);
                g.FillRectangle(Brushes.SteelBlue, headerRect);

                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawRectangle(Pens.Black, tableX + colWidths.Take(i).Sum(), tableY, colWidths[i], rowHeight);
                    g.DrawString(headers[i], headerFont, Brushes.White, tableX + colWidths.Take(i).Sum() + 5, tableY + 5);
                }

                tableY += rowHeight;

                // --- Payment Row ---
                DrawTableRow(g, tableX, tableY, colWidths,
                    new string[]
                    {
                feeTypeName,
                payment.PaidAmount.ToString("C"),
                payment.PaymentMode.ToString(),
                payment.CreatedDate.ToString("dd/MM/yyyy hh:mm tt")
                    },
                    normalFont
                );

                tableY += rowHeight + 20;

                // --- Summary ---
                decimal remainingBalance = Math.Max(payment.TotalFees - payment.PaidAmount, 0);

                // Ensure we don’t draw over buttons
                if (tableY + 100 > maxY) tableY = maxY - 100;

                g.DrawString($"Paid By: {enrollment.StudentFullName}", normalFont, Brushes.Black, leftMargin, tableY);
                tableY += 25;
                g.DrawString($"Amount Paid: {payment.PaidAmount:C}   Remaining Balance: {remainingBalance:C}", normalFont, Brushes.Black, leftMargin, tableY);
                tableY += 40;

                // --- Footer ---
                g.DrawString("Thank you for your payment. For any queries, please contact the school.", normalFont, Brushes.Black, leftMargin, tableY);
                tableY += 20;
                g.DrawString($"Contact: {school?.Email ?? "???"} | {school?.Phone ?? "???"}", normalFont, Brushes.Black, leftMargin, tableY);
            };

            pnl.Invalidate(); // redraw
        }

        private void DrawTableRow(Graphics g, int x, int y, int[] colWidths, string[] values, Font font)
        {
            for (int i = 0; i < values.Length; i++)
            {
                g.DrawRectangle(Pens.Black, x + colWidths.Take(i).Sum(), y, colWidths[i], 25);
                g.DrawString(values[i], font, Brushes.Black, x + colWidths.Take(i).Sum() + 5, y + 5);
            }
        }


        private void LoadReceiptPreview()
        {
            pnlReceipt.Controls.Clear();

            var schoolInfo = clsSchoolInfo.GetSchoolInfo();
            string schoolName = schoolInfo?.SchoolName ?? "School Name";
            string schoolContact = $"Address: {schoolInfo?.Address}\nPhone: {schoolInfo?.Phone}\nEmail: {schoolInfo?.Email}";

            DataTable dt = clsReceipts.GetReceiptsByTuitionFee(_paymentID);
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Payment not found!");
                return;
            }

            DataRow row = dt.Rows[0];
            string paymentModeText = ((clsTuitionPayment.enPaymentType)Convert.ToInt32(row["PaymentMode"])).ToString();
            decimal totalFees = Convert.ToDecimal(row["TotalFees"]);
            decimal paidAmount = Convert.ToDecimal(row["PaidAmount"]);
            decimal remainingBalance = totalFees - paidAmount;

            int yOffset = 10;

            // School Name
            Label lblSchool = new Label
            {
                Text = schoolName,
                Font = new Font("Arial", 16, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblSchool);
            yOffset += lblSchool.Height + 5;

            // School Contact
            Label lblContact = new Label
            {
                Text = schoolContact,
                Font = new Font("Arial", 10),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblContact);
            yOffset += lblContact.Height + 10;

            // Separator
            pnlReceipt.Controls.Add(CreateSeparator(yOffset));
            yOffset += 10;

            // Student Info
            Label lblStudent = new Label
            {
                Text = $"Student: {row["FullName"]}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblStudent);
            yOffset += lblStudent.Height + 5;

            Label lblTuitionID = new Label
            {
                Text = $"TuitionFeeID: {row["TuitionFeeID"]}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblTuitionID);
            yOffset += lblTuitionID.Height + 5;

            Label lblPaymentDate = new Label
            {
                Text = $"Payment Date: {Convert.ToDateTime(row["CreatedDate"]):dd/MM/yyyy}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblPaymentDate);
            yOffset += lblPaymentDate.Height + 5;

            Label lblPaymentType = new Label
            {
                Text = $"Payment Type: {paymentModeText}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblPaymentType);
            yOffset += lblPaymentType.Height + 5;

            Label lblPaid = new Label
            {
                Text = $"Amount Paid: {paidAmount:0.00}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblPaid);
            yOffset += lblPaid.Height + 5;

            Label lblRemaining = new Label
            {
                Text = $"Remaining Balance: {remainingBalance:0.00}",
                Font = new Font("Arial", 12),
                AutoSize = true,
                Location = new Point(10, yOffset)
            };
            pnlReceipt.Controls.Add(lblRemaining);
        }

        private Control CreateSeparator(int yOffset)
        {
            return new Label
            {
                BorderStyle = BorderStyle.Fixed3D,
                Width = pnlReceipt.Width - 20,
                Height = 2,
                Location = new Point(10, yOffset)
            };
        }

        private void btnClick_Click(object sender, EventArgs e)
        {
            using (Bitmap bmp = new Bitmap(pnlReceipt.Width, pnlReceipt.Height))
            {
                pnlReceipt.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|JPEG Image|*.jpg",
                    FileName = $"Receipt_{_paymentID}.png"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                    bmp.Save(sfd.FileName);
            }
        }

        private void btnPrintIt_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, args) =>
            {
                Bitmap bmp = new Bitmap(pnlReceipt.Width, pnlReceipt.Height);
                pnlReceipt.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
                args.Graphics.DrawImage(bmp, 50, 50);
            };

            PrintPreviewDialog preview = new PrintPreviewDialog { Document = pd };
            preview.ShowDialog();
        }
    }
}
