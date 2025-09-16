using SchoolProject.Global_Classes;
using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Fees_and_Payments
{
    public partial class frmPayInstallment : Form
    {
        private int _InstallmentID = -1;
        private clsInstallment currentInstallment;
        private clsTuitionPayment currentPayment;

        public frmPayInstallment(int installmentID)
        {
            InitializeComponent();
            _InstallmentID = installmentID;

            LoadInstallmentDetails();
            LoadInstallmentDetails();  // هنا نستدعي تحميل البيانات وليس العرض فقط
        }
        private void LoadInstallmentDetails()
        {
            currentInstallment = clsInstallment.Find(_InstallmentID);
            if (currentInstallment == null)
            {
                MessageBox.Show("Installment not found.");
                this.Close();
                return;
            }

            currentPayment = clsTuitionPayment.FindPaymentByTuitionFeeID(currentInstallment.TuitionFeeID);
            if (currentPayment == null)
            {
                MessageBox.Show("Payment info not found for this installment.");
                this.Close();
                return;
            }

            DisplayData();
        }
        private void DisplayData()
        {
            lblFullName.Text = currentPayment.StudentInfo?.PersonInfo?.FullName ?? "Unknown";

            lblInstallmentNumber.Text = currentInstallment.InstallmentNumber.ToString();
            lblAmount.Text = currentInstallment.Amount.ToString("0.00");
            lblDueDate.Text = currentInstallment.DueDate.ToShortDateString();
            lblPaidIn.Text = currentInstallment.PaidDate.HasValue
                ? currentInstallment.PaidDate.Value.ToString("g") // general date/time
                : "Not Paid";

            cbIsPaid.Checked = currentInstallment.IsPaid;

            // --- Calculate and display total paid ---
            var dtInstallments = clsInstallment.GetAllByTuitionFee(currentInstallment.TuitionFeeID);
            decimal totalPaid = 0;
            foreach (DataRow row in dtInstallments.Rows)
            {
                if (row["IsPaid"] != DBNull.Value && (bool)row["IsPaid"])
                    totalPaid += row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0;
            }

            lblTotalPaid.Text = totalPaid.ToString("0.00"); // add a label lblTotalPaid on your form
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            int currentUserID = clsGlobal.CurrentUser.UserID;

            if (currentInstallment == null) return;

            // Mark this installment as paid
            bool success = currentInstallment.MarkAsPaid(currentUserID, DateTime.Now);

            if (!success)
            {
                MessageBox.Show("Failed to mark installment as paid.", "Error");
                return;
            }

            // Refresh installment data
            currentInstallment = clsInstallment.Find(_InstallmentID);

            // Recalculate total paid from all installments
            var dtInstallments = clsInstallment.GetAllByTuitionFee(currentInstallment.TuitionFeeID);
            decimal totalPaid = 0;
            foreach (DataRow row in dtInstallments.Rows)
            {
                if (row["IsPaid"] != DBNull.Value && (bool)row["IsPaid"])
                    totalPaid += row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0;
            }

            // Update main payment record
            currentPayment = clsTuitionPayment.FindPaymentByTuitionFeeID(currentInstallment.TuitionFeeID);
            if (currentPayment != null)
            {
                currentPayment.PaidAmount = totalPaid;

                // Check if fully paid
                if (totalPaid >= currentPayment.TotalFees)
                {
                    currentPayment.IsFullyPaid = true;
                    currentPayment.PaymentDate = DateTime.Now;

                    // Optionally, set FirstPaymentDate if not set
                   
                }
                else
                {
                    currentPayment.IsFullyPaid = false;
                }

                // Save the updated tuition payment record
                currentPayment.Save();
            }

            // Update total paid label in the UI
            lblTotalPaid.Text = totalPaid.ToString("0.00");

            MessageBox.Show("Installment marked as paid successfully!");
        }
    }
    }
 
 
