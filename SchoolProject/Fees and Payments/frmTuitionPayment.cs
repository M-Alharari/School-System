using SchoolProject.Fees_and_Payments;
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
using System.Globalization;
using System.Text.RegularExpressions;
using SchoolProject.Receipts;
namespace SchoolProject
{
    public partial class frmTuitionPayment : Form
    {
        private System.Windows.Forms.Timer checkInstallmentsTimer;

        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        public int StudentID { get; set; } = -1;
        public clsEnrollment _Enrollment;
        private int _EnrollmentID;
        private clsTuitionPayment currentPayment;
        public clsStudent StudentInfo { get; set; }
        public bool WasInUpdateMode { get; private set; }
        public frmTuitionPayment(int enrollmentID)
        {
            InitializeComponent();
            _EnrollmentID = enrollmentID;

            if (_EnrollmentID == -1)
            {
                WasInUpdateMode = false;
                return;
            }

            _Enrollment = clsEnrollment.Find(_EnrollmentID);
            lblFullName.Text = _Enrollment.StudentFullName;

            currentPayment = clsTuitionPayment.FindPaymentByEnrollmentID(_EnrollmentID);
            if (currentPayment != null)
            {
                Mode = enMode.Update;
                WasInUpdateMode = true;
                FillFormWithPaymentDetails();
            }
            else
            {
                Mode = enMode.AddNew;
                WasInUpdateMode = false;
            }

        }

        private void UpdatePaidLabel()
        {
            if (currentPayment == null) return;

            if (cmbPaymentMode.SelectedItem == null) return;

            var selectedMode = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            if (selectedMode == clsTuitionPayment.enPaymentType.Full)
            {
                // Paid amount equals total fee
                if (decimal.TryParse(txtTotalFees.Text, out decimal total))
                {
                    lblPaidFees.Text = total.ToString("0.00");
                }
                else
                {
                    lblPaidFees.Text = "0.00";
                }
            }
            else if (selectedMode == clsTuitionPayment.enPaymentType.Installment)
            {
                // Show first installment amount if total fee > 0
                if (decimal.TryParse(txtTotalFees.Text, out decimal total))
                {
                    int numberOfInstallments = 1;

                    if (cmbInstallmentFrequencyID.SelectedItem != null)
                    {
                        switch ((clsTuitionPayment.enInstallmentFrequency)cmbInstallmentFrequencyID.SelectedItem)
                        {
                            case clsTuitionPayment.enInstallmentFrequency.Monthly:
                                numberOfInstallments = 12; break;
                            case clsTuitionPayment.enInstallmentFrequency.Quarterly:
                                numberOfInstallments = 4; break;
                            case clsTuitionPayment.enInstallmentFrequency.SemiAnnual:
                                numberOfInstallments = 2; break;
                            case clsTuitionPayment.enInstallmentFrequency.Yearly:
                                numberOfInstallments = 1; break;
                            case clsTuitionPayment.enInstallmentFrequency.Experimental:
                                numberOfInstallments = 6; break;
                        }
                    }

                    decimal firstInstallment = Math.Round(total / numberOfInstallments, 2);
                    lblPaidFees.Text = firstInstallment.ToString("0.00");
                }
                else
                {
                    lblPaidFees.Text = "0.00";
                }
            }
        }

        private void FillFormWithPaymentDetails()
        {
            FillPaymentModeCombo();
            FillInstallmentFrequencyCombo();
            Checks();

            lblFullName.Text = currentPayment.FullName ?? "Unknown";
            txtTotalFees.Text = currentPayment.TotalFees.ToString("F2");

            decimal paidAmount = 0m;

            // Set lblPaidAmount depending on mode
            if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Full)
            {
                paidAmount = currentPayment.TotalFees;
                lblPaidFees.Text = currentPayment.TotalFees.ToString("0.00");
            }
            else if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Installment)
            {
                var installments = clsInstallment.GetAllByTuitionFee(currentPayment.TuitionFeeID);
                if (installments != null && installments.Rows.Count > 0)
                {
                    // Take first installment or sum all paid
                    paidAmount = 0m;
                    foreach (DataRow row in installments.Rows)
                    {
                        if ((bool)row["IsPaid"])
                            paidAmount += Convert.ToDecimal(row["Amount"]);
                    }

                    // If no installment is paid yet, show first installment
                    if (paidAmount == 0m)
                        paidAmount = Convert.ToDecimal(installments.Rows[0]["Amount"]);

                    lblPaidFees.Text = paidAmount.ToString("0.00");
                }
                else
                {
                    lblPaidFees.Text = "0.00";
                }
            }

            // Determine if fully paid dynamically
            lblIsFullyPaid.Text = paidAmount >= currentPayment.TotalFees ? "Paid in Full" : "Not Fully Paid";

            // Display First Payment Date
            lblFirstPaymentDate.Text = currentPayment.CreatedDate.ToString();


            cmbPaymentMode.SelectedItem = currentPayment.PaymentMode;
            cmbInstallmentFrequencyID.SelectedItem = currentPayment.InstallmentFrequencyID;

            if (Mode == enMode.Update)
            {
                txtTotalFees.Enabled = false;
                cmbInstallmentFrequencyID.Enabled = false;
                cmbPaymentMode.Enabled = false;
                btnSave.Enabled = paidAmount < currentPayment.TotalFees;
            }
        }




        private void ApplyPaymentModeSettings(clsTuitionPayment.enPaymentType paymentMode)
        {
            if (paymentMode == clsTuitionPayment.enPaymentType.Full)
            {
                cmbInstallmentFrequencyID.Enabled = false;

                // Optional: reset selection to None
                cmbInstallmentFrequencyID.SelectedItem = clsTuitionPayment.enInstallmentFrequency.None;
            }
            else if (paymentMode == clsTuitionPayment.enPaymentType.Installment)
            {
                cmbInstallmentFrequencyID.Enabled = true;

               
                    var enumValues = (clsTuitionPayment.enInstallmentFrequency[])cmbInstallmentFrequencyID.DataSource;
                  
                
            }
        }



        private void _ResetDefaultValues()
        {
            FillPaymentModeCombo();
            FillInstallmentFrequencyCombo();

            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Pay School Fees";
                currentPayment = new clsTuitionPayment();

                // Set default payment mode and apply settings
                cmbPaymentMode.SelectedItem = clsTuitionPayment.enPaymentType.Full;
                //ApplyPaymentModeSettings(clsTuitionPayment.enPaymentType.Full);
            }
            else
            {
                _LoadData();
                txtTotalFees.Enabled = false;
            }
        }


        private void _LoadData()
        {
            currentPayment = clsTuitionPayment.FindPaymentByEnrollmentID(_EnrollmentID);
            if (currentPayment == null)
            {
                MessageBox.Show($"No Student with ID: {_EnrollmentID}", "Student Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }
            FillFormWithPaymentDetails();
            CheckAndEnableInstallmentGroupBox();
        }

        private void LoadDataFromControls()
        {
            if (cmbPaymentMode.SelectedItem != null)
                currentPayment.PaymentMode = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            if (decimal.TryParse(txtTotalFees.Text, out decimal total))
                currentPayment.TotalFees = total;
            lblFullName.Text= currentPayment.StudentInfo.PersonInfo.FullName;
            // Determine amount to pay based on mode
            if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Full)
            {
                currentPayment.PaidAmount = currentPayment.TotalFees;
                
                lblPaidFees.Text = currentPayment.TotalFees.ToString("0.00");
            }
            else if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Installment)
            {
                currentPayment.InstallmentFrequencyID = cmbInstallmentFrequencyID.SelectedItem != null
                    ? (clsTuitionPayment.enInstallmentFrequency)cmbInstallmentFrequencyID.SelectedItem
                    : clsTuitionPayment.enInstallmentFrequency.None;

                // Generate installments if not already done
                GenerateInstallments();

                // Set PaidAmount to first unpaid installment
                var installments = clsInstallment.GetAllByTuitionFee(currentPayment.TuitionFeeID);
                if (installments != null && installments.Rows.Count > 0)
                {
                    foreach (DataRow row in installments.Rows)
                    {
                        if (!(bool)row["IsPaid"])
                        {
                            currentPayment.PaidAmount = Convert.ToDecimal(row["Amount"]);
                            break;
                        }
                    }
                }
                lblFullName.Text = currentPayment.StudentInfo.PersonInfo.FullName;
                lblPaidFees.Text = currentPayment.PaidAmount.ToString("0.00");
            }
        }

        private void Checks()
        {
            if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Full)
            {
                cmbInstallmentFrequencyID.Enabled = false;
                cmbPaymentMode.Enabled = false;
                btnSave.Enabled = !currentPayment.IsFullyPaid;
            }
            else if (currentPayment.PaymentMode == clsTuitionPayment.enPaymentType.Installment)
            {
                cmbPaymentMode.Enabled = false;
                cmbInstallmentFrequencyID.Enabled = false;

                var installments = clsInstallment.GetAllByTuitionFee(currentPayment.TuitionFeeID);
                bool anyUnpaid = installments.AsEnumerable().Any(r => !(bool)r["IsPaid"]);
                btnSave.Enabled = anyUnpaid;
            }
        }


        private void FillPaymentModeCombo()
        {
            cmbPaymentMode.DataSource = Enum.GetValues(typeof(clsTuitionPayment.enPaymentType));

            // Select current payment mode if available
            if (currentPayment != null)
            {
                cmbPaymentMode.SelectedItem = currentPayment.PaymentMode;
            }
            else
            {
                cmbPaymentMode.SelectedIndex = 0; // default
            }
        }

        private void FillInstallmentFrequencyCombo()
        {
            cmbInstallmentFrequencyID.DataSource = Enum.GetValues(typeof(clsTuitionPayment.enInstallmentFrequency));

            
        }
        private bool TryParseDecimalFlexible(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input)) return false;

            if (decimal.TryParse(input, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out value))
                return true;

            string cleaned = Regex.Replace(input, @"[^\d\.,\-]", "");
            cleaned = cleaned.Replace(',', '.');

            return decimal.TryParse(cleaned, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
        }


        private void btnSave_Click(object sender, EventArgs e)
        { 
            // 1️⃣ Validation
            if (cmbPaymentMode.SelectedItem == null)
            {
                MessageBox.Show("Select payment mode.");
                return;
            }

            var paymentMode = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            if (paymentMode == clsTuitionPayment.enPaymentType.Installment &&
                (cmbInstallmentFrequencyID.SelectedItem == null ||
                 (clsTuitionPayment.enInstallmentFrequency)cmbInstallmentFrequencyID.SelectedItem == clsTuitionPayment.enInstallmentFrequency.None))
            {
                MessageBox.Show("Select valid installment frequency.");
                return;
            }

            if (!decimal.TryParse(txtTotalFees.Text, out decimal totalFees))
            {
                MessageBox.Show("Enter total fees correctly.");
                return;
            }

            // 2️⃣ Load common data into payment object
            currentPayment.TotalFees = totalFees;
            currentPayment.PaymentMode = paymentMode;
            currentPayment.EnrollmentID = _EnrollmentID;
            currentPayment.PaymentDate = DateTime.Now;

            if (Mode == enMode.AddNew)
                currentPayment.CreatedByUserID = clsGlobal.CurrentUser.UserID;

            if (!currentPayment.Save())
            {
                MessageBox.Show("Payment not saved.");
                return;
            }

            // 3️⃣ Handle Full Payment
            if (paymentMode == clsTuitionPayment.enPaymentType.Full)
            {
                currentPayment.PaidAmount = totalFees;
                currentPayment.IsFullyPaid = true;
                currentPayment.InstallmentFrequencyID = clsTuitionPayment.enInstallmentFrequency.None;
                currentPayment.Save();

                // Generate receipt WITHOUT InstallmentID
                clsReceipts receipt = new clsReceipts
                {
                    ReceiptNumber = clsReceipts.GenerateReceiptNumber(),
                    TuitionFeeID = currentPayment.TuitionFeeID,
                    Amount = totalFees,
                    PaymentDate = DateTime.Now,
                    CreatedByUserID = clsGlobal.CurrentUser.UserID,
                    ModifiedByUserID = clsGlobal.CurrentUser.UserID,
                    ModifiedDate = DateTime.Now,
                    InstallmentID = null // ✅ No FK issue
                };
                receipt.Save();
            }

            // 4️⃣ Handle Installment Payment
            // 4️⃣ Handle Installment Payment
            else if (paymentMode == clsTuitionPayment.enPaymentType.Installment)
            {
                // Set installment frequency
                currentPayment.InstallmentFrequencyID =
                    (clsTuitionPayment.enInstallmentFrequency)cmbInstallmentFrequencyID.SelectedItem;
                currentPayment.Save();

                // Ensure installments exist
                GenerateInstallments();

                // Find first unpaid installment
                int firstUnpaidInstallmentID = clsInstallment.GetFirstUnpaidInstallmentID(currentPayment.TuitionFeeID);
                if (firstUnpaidInstallmentID <= 0)
                {
                    MessageBox.Show("No unpaid installment found.");
                    return;
                }

                clsInstallment installment = clsInstallment.Find(firstUnpaidInstallmentID);
                if (installment == null)
                {
                    MessageBox.Show("Installment not found.");
                    return;
                }

                // Mark installment as paid
                if (!installment.MarkAsPaid(clsGlobal.CurrentUser.UserID, DateTime.Now))
                {
                    MessageBox.Show("Failed to mark installment as paid.");
                    return;
                }

                // ✅ Generate receipt for EXACTLY this installment
                clsReceipts receipt = new clsReceipts
                {
                    ReceiptNumber = clsReceipts.GenerateReceiptNumber(),
                    TuitionFeeID = currentPayment.TuitionFeeID,
                    Amount = installment.Amount,    // ✅ Paid amount = installment amount
                    PaymentDate = DateTime.Now,
                    CreatedByUserID = clsGlobal.CurrentUser.UserID,
                    ModifiedByUserID = clsGlobal.CurrentUser.UserID,
                    ModifiedDate = DateTime.Now,
                    InstallmentID = installment.InstallmentID
                };

                if (!receipt.Save())
                {
                    MessageBox.Show("Failed to save receipt for installment.");
                    return;
                }

                // Update total paid
                var dtInstallments = clsInstallment.GetInstallmentSummaryByTuitionFeeID(currentPayment.TuitionFeeID);
                decimal totalPaid = 0;
                foreach (DataRow row in dtInstallments.Rows)
                {
                    if (row["IsPaid"] != DBNull.Value && (bool)row["IsPaid"])
                        totalPaid += Convert.ToDecimal(row["Amount"]);
                }

                currentPayment.PaidAmount = totalPaid;
                currentPayment.IsFullyPaid = totalPaid >= currentPayment.TotalFees;
                currentPayment.Save();

                lblPaidFees.Text = installment.Amount.ToString("0.00"); // ✅ show only what was just paid
            }


            // 5️⃣ Refresh UI
            lblIsFullyPaid.Text = currentPayment.IsFullyPaid ? "Paid" : "Not Fully Paid";
            Mode = enMode.Update;
            cmbPaymentMode.Enabled = false;
            Checks();

            MessageBox.Show("Payment saved successfully!");
            frmFeesReceiptViewer receiptPreview = new frmFeesReceiptViewer(currentPayment.TuitionFeeID);
            receiptPreview.ShowDialog();
        }

        private void FormTitle()
        {
            string paymentType = currentPayment.PaymentMode.ToString();
            lblTitle.Text = $"{paymentType} Payment Details Finished";
        }



        private void Form3_Load(object sender, EventArgs e)
        {
            if (currentPayment != null) // Update mode
            {
                Mode = enMode.Update;
                FillFormWithPaymentDetails();
            }
            else // Add mode
            {
                Mode = enMode.AddNew;
                currentPayment = new clsTuitionPayment();
                _ResetDefaultValues();
            }

            // Trigger the SelectedIndexChanged manually to set combo enabled state
            cmbPaymentMode.SelectedIndexChanged -= cmbPaymentMode_SelectedIndexChanged; // detach to avoid double call
            cmbPaymentMode.SelectedIndex = 0; // or the appropriate default
            cmbPaymentMode.SelectedIndexChanged += cmbPaymentMode_SelectedIndexChanged;
            cmbPaymentMode_SelectedIndexChanged(cmbPaymentMode, EventArgs.Empty);

            // Optional: focus after load
            Timer timer = new Timer();
            timer.Interval = 100;
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                cmbPaymentMode.Focus();
            };
            timer.Start();

        }
        
 
        private void CheckInstallmentsTimer_Tick(object sender, EventArgs e)
        {
            CheckAndEnableInstallmentGroupBox();
        }


        private void cmbPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPaymentMode.SelectedItem == null) return;

            var selectedPaymentType = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            cmbInstallmentFrequencyID.Enabled = selectedPaymentType == clsTuitionPayment.enPaymentType.Installment;

            UpdatePaidLabel();
            //if (cmbPaymentMode.SelectedItem == null) return;

            //var selectedPaymentType = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            //if (selectedPaymentType == clsTuitionPayment.enPaymentType.Full)
            //{
            //    cmbInstallmentFrequencyID.Enabled = false;
            //    if (cmbInstallmentFrequencyID.Items.Count > 0)
            //        cmbInstallmentFrequencyID.SelectedIndex = 0;
            //}
            //else if (selectedPaymentType == clsTuitionPayment.enPaymentType.Installment)
            //{
            //    cmbInstallmentFrequencyID.Enabled = true;
            //}

            //UpdatePaidLabel();

        }




      

        private void GenerateInstallments()
        {
            var existing = clsInstallment.GetAllByTuitionFee(currentPayment.TuitionFeeID);
            if (existing != null && existing.Rows.Count > 0) return;

            int numberOfInstallments;
            DateTime dueDate;

            switch (currentPayment.InstallmentFrequencyID)
            {
                case clsTuitionPayment.enInstallmentFrequency.Monthly:
                    numberOfInstallments = 12;
                    dueDate = DateTime.Today.AddMonths(1);
                    break;
                case clsTuitionPayment.enInstallmentFrequency.Quarterly:
                    numberOfInstallments = 4;
                    dueDate = DateTime.Today.AddMonths(3);
                    break;
                case clsTuitionPayment.enInstallmentFrequency.SemiAnnual:
                    numberOfInstallments = 2;
                    dueDate = DateTime.Today.AddMonths(6);
                    break;
                case clsTuitionPayment.enInstallmentFrequency.Yearly:
                    numberOfInstallments = 1;
                    dueDate = DateTime.Today.AddYears(1);
                    break;
                case clsTuitionPayment.enInstallmentFrequency.Experimental:
                    numberOfInstallments = 6;
                    dueDate = DateTime.Now.AddSeconds(10);
                    break;
                default:
                    MessageBox.Show("Please select a valid installment frequency.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
            }

            decimal installmentAmount = Math.Round(currentPayment.TotalFees / numberOfInstallments, 2);

            for (int i = 1; i <= numberOfInstallments; i++)
            {
                clsInstallment installment = new clsInstallment
                {
                    TuitionFeeID = currentPayment.TuitionFeeID,
                    InstallmentNumber = i,
                    DueDate = dueDate,
                    Amount = installmentAmount,

                    CreatedByUserID = clsGlobal.CurrentUser.UserID,
                    CreatedAt = DateTime.Now,
                    ModifiedByUserID = clsGlobal.CurrentUser.UserID,
                    ModifiedAt = DateTime.Now
                };
                installment.Save();

                dueDate = currentPayment.InstallmentFrequencyID == clsTuitionPayment.enInstallmentFrequency.Experimental
                    ? dueDate.AddSeconds(10)
                    : dueDate.AddMonths(1);
            }
        }

        private void CheckAndEnableInstallmentGroupBox()
        {
            if (currentPayment == null || currentPayment.PaymentMode != clsTuitionPayment.enPaymentType.Installment)
                return;

            var installments = clsInstallment.GetAllByTuitionFee(currentPayment.TuitionFeeID);
            if (installments == null || installments.Rows.Count == 0)
                return;

            foreach (DataRow row in installments.Rows)
            {
                DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
                bool isPaid = Convert.ToBoolean(row["IsPaid"]);
                if (!isPaid && dueDate <= DateTime.Now)
                {
                    //gbMonthlyPayments.Enabled = true; // enable if any unpaid installment is due
                    return;
                }
            }

            //gbMonthlyPayments.Enabled = false; // disable if all installments are paid or not yet due
        }



        private void _LoadDataFromControlsToPayment()
        {
            if (currentPayment == null) return;

            // Payment Mode
            if (cmbPaymentMode.SelectedItem != null)
                currentPayment.PaymentMode = (clsTuitionPayment.enPaymentType)cmbPaymentMode.SelectedItem;

            // Total Fees
            if (decimal.TryParse(txtTotalFees.Text, out decimal total))
                currentPayment.TotalFees = total;
        }




        private void btnManage_Click(object sender, EventArgs e)
        {
            if (currentPayment == null)
            {
                MessageBox.Show("لم يتم تحميل بيانات الدفع بعد.");
                return;
            }

            if (currentPayment.PaymentMode != clsTuitionPayment.enPaymentType.Installment)
            {
                MessageBox.Show("نوع الدفع ليس تقسيط.");
                return;
            }

            var frm = new frmInstallmnetRecord(currentPayment.TuitionFeeID);
            frm.ShowDialog();
        }

        private void cmbInstallmentFrequencyID_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePaidLabel();
        }

        private void txtTotalFees_TextChanged(object sender, EventArgs e)
        {
            UpdatePaidLabel();
        }

        private void btnrecept_Click(object sender, EventArgs e)
        {
            // Open the receipt preview
            frmFeesReceiptViewer receiptPreview = new frmFeesReceiptViewer(currentPayment.TuitionFeeID);
            receiptPreview.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmFeesReceipt frmFeesReceipt   = new frmFeesReceipt(currentPayment.TuitionFeeID);
            frmFeesReceipt.ShowDialog();
        }
    }
}