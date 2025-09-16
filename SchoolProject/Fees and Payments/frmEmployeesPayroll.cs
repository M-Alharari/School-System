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

namespace SchoolProject.Attendance
{
    public partial class frmEmployeesPayroll : Form
    {
        public frmEmployeesPayroll()
        {
            InitializeComponent();
        }


        private void LoadPayroll(int year, int month)
        {
            try
            {
                DataTable payrollData = clsPayroll.GetPayroll(year, month);

                if (payrollData.Rows.Count > 0)
                {
                    dgvPayroll.DataSource = payrollData;
                    lblRecordCount.Text = dgvPayroll.RowCount.ToString();
                }
                else
                {
                    dgvPayroll.DataSource = null;
                    MessageBox.Show("No payroll records found for the selected month and year.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading payroll: " + ex.Message);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            int year = dtpPayrollDate.Value.Year;
            int month = dtpPayrollDate.Value.Month;

            LoadPayroll(year, month);
        }

        private void frmEmployeesPayroll_Load(object sender, EventArgs e)
        {
            // set today's date in the DateTimePicker
            dtpPayrollDate.Value = DateTime.Today;

            // auto-load payroll for today's month and year
            int year = DateTime.Today.Year;
            int month = DateTime.Today.Month;
            LoadPayroll(year, month);
        }

        private void button1_Click(object sender, EventArgs e)
        {

            int year = dtpPayrollDate.Value.Year;
            int month = dtpPayrollDate.Value.Month;

            try
            {
                // 1. Validate attendance + deductions first
                if (!clsPayroll.ValidateAttendanceAndDeductions(year, month))
                {
                    MessageBox.Show($"Attendance or deductions are incomplete for this month {month}. Please review before processing payroll.",
                                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // stop payroll run
                }

                // 2. Get payroll data
                DataTable payrollData = clsPayroll.GetPayroll(year, month);

                foreach (DataRow row in payrollData.Rows)
                {
                    int employeeID = Convert.ToInt32(row["EmployeeID"]);
                    decimal gross = Convert.ToDecimal(row["MonthlySalary"]);
                    decimal deductions = Convert.ToDecimal(row["TotalDeductions"]);
                    decimal net = Convert.ToDecimal(row["NetSalary"]);
                    int currentUserID = clsGlobal.CurrentUser.UserID;

                    // Add payroll record if it doesn't exist
                    clsPayroll.AddPayrollPayment(employeeID, year, month, gross, deductions, net, currentUserID);

                    // Mark as paid
                    clsPayroll.MarkPayrollAsPaid(employeeID, year, month);
                }

                // Reload grid after processing
                LoadPayroll(year, month);

                MessageBox.Show("Payroll processed and marked as paid for all employees.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing payroll: " + ex.Message);
            }
        }

    }

}
 
