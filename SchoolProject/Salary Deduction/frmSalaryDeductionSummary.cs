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
namespace SchoolProject.Salary_Deduction
{
    public partial class frmSalaryDeductionSummary : Form
    {
        public frmSalaryDeductionSummary()
        {
            InitializeComponent();
        }

        private void frmSalaryDeductionSummary_Load(object sender, EventArgs e)
        {
            // Fill month combo
            cmbMonth.DataSource = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                .Where(m => !string.IsNullOrEmpty(m)).ToList();
            cmbMonth.SelectedIndex = DateTime.Now.Month - 1;

            int currentYear = DateTime.Now.Year;

            // Ensure limits
            nudYear.Minimum = 1900;
            nudYear.Maximum = 2100;

            // Safe assignment
            if (currentYear >= nudYear.Minimum && currentYear <= nudYear.Maximum)
            {
                nudYear.Value = currentYear;
            }
            else
            {
                nudYear.Value = nudYear.Minimum; // fallback
            }

            // Fill deduction per day combo
            cmbDeductionPerDay.Items.Clear();
            cmbDeductionPerDay.Items.AddRange(new object[] { 0.5m, 1m, 1.5m, 2m }); // Add more if needed
            cmbDeductionPerDay.SelectedIndex = 0; // default 0.5

            LoadSummary();
        }
        private void LoadSummary()
        {
            int month = cmbMonth.SelectedIndex + 1;
            int year = (int)nudYear.Value;

            // Get user-selected deduction per day
            decimal deductionPerDay = cmbDeductionPerDay.SelectedItem != null
                ? Convert.ToDecimal(cmbDeductionPerDay.SelectedItem)
                : 0.5m; // default if null

            // Get raw summary data
            DataTable dt = clsSalaryDeductionSummary.GetMonthlySummary(month, year);

            // Calculate deductions
            foreach (DataRow row in dt.Rows)
            {
                int absenceDays = Convert.ToInt32(row["TotalAbsenceDays"]);
                decimal totalDeduction = deductionPerDay * absenceDays;
                decimal baseSalary = Convert.ToDecimal(row["BaseSalary"]);
                decimal netSalary = baseSalary - totalDeduction;

                row["TotalDeduction"] = totalDeduction;
                row["TotalDeduction"] = netSalary;
            }

            // Bind to DataGridView
            dgvSummary.DataSource = dt;

            var egpCulture = new CultureInfo("en-US");

            egpCulture.NumberFormat.CurrencySymbol = "EGP";

            // Format columns
            dgvSummary.Columns["EmployeeID"].Visible = false;

            if (dgvSummary.Columns.Contains("FullName"))
                dgvSummary.Columns["FullName"].HeaderText = "Employee";

            dgvSummary.Columns["TotalAbsenceDays"].HeaderText = "Total Absence (Days)";

            if (dgvSummary.Columns.Contains("MonthlySalary"))
            {
                dgvSummary.Columns["BaseSalary"].HeaderText = "Base Salary";
                dgvSummary.Columns["BaseSalary"].DefaultCellStyle.Format = "C2";
                dgvSummary.Columns["BaseSalary"].DefaultCellStyle.FormatProvider = egpCulture;

            }

            dgvSummary.Columns["TotalDeduction"].HeaderText = "Total Deduction";
            dgvSummary.Columns["TotalDeduction"].DefaultCellStyle.Format = "C2";
            dgvSummary.Columns["TotalDeduction"].DefaultCellStyle.FormatProvider = egpCulture;

            // If you are calculating NetSalary on the fly
            if (dgvSummary.Columns.Contains("NetSalary"))
            {
                dgvSummary.Columns["NetSalary"].HeaderText = "Net Salary";
                dgvSummary.Columns["NetSalary"].DefaultCellStyle.Format = "C2";
                dgvSummary.Columns["NetSalary"].DefaultCellStyle.FormatProvider = egpCulture;

            }




        }


        private void btnGenerateSummary_Click(object sender, EventArgs e)
        {
            int month = cmbMonth.SelectedIndex + 1; // ComboBox is 0-indexed
            int year = (int)nudYear.Value;

            // Generate monthly summary
            bool success = clsSalaryDeductionSummary.GenerateMonthlySummary(month, year);
            if (success)
                MessageBox.Show("Monthly salary deduction summary generated successfully.");

            LoadSummary();
        }

        private void cmbDeductionPerDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSummary();
        }
    }
}
