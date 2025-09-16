using SchoolProject.Students;
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
    public partial class frmPaymentManage : Form
    {
        clsTuitionPayment _TuitionPayment;
        private static DataTable _dtAllPayment = clsTuitionPayment.GetAllTuition();
      


      

        public frmPaymentManage()
        {
            InitializeComponent();
        }

        private void frmPaymentManagement_Load(object sender, EventArgs e)
        {

          
            dgvPayments.DataSource = _dtAllPayment;
            cbFilterBy.SelectedIndex = 0;
            lblRecordCount.Text = dgvPayments.Rows.Count.ToString();

            

           
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {

        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentDetail frm = new frmStudentDetail((int)dgvPayments.CurrentRow.Cells[0].Value);

            frm.ShowDialog();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.Text == "Is Paid")
            {
                txtFilterValue.Visible = false;
                cbIsPaid.Visible = true;
                cbIsPaid.SelectedIndex = 0;
                cbIsPaid.Focus();
            }
            else
            {
                txtFilterValue.Visible = (cbFilterBy.Text != "None");
                cbIsPaid.Visible = false;

                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }

        private void cbIsPaid_SelectedIndexChanged(object sender, EventArgs e)
        {

            string FilterColumn = "IsFullyPaid";
            string FilterValue = cbIsPaid.Text;
            switch (FilterValue)
            {
                case "All":
                    break;
                case "Yes":
                    FilterValue = "1";
                    break;
                case "No":
                    FilterValue = "0";
                    break;
            }

            if (FilterValue == "All")
                _dtAllPayment.DefaultView.RowFilter = "";
            else
                _dtAllPayment.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);
           lblRecordCount.Text = dgvPayments.Rows.Count.ToString();
        }



        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {

            string filterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "Student ID":
                    filterColumn = "EnrollmentID";
                    break;
                case "Full Name":
                    filterColumn = "FullName";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            if (string.IsNullOrWhiteSpace(txtFilterValue.Text) || filterColumn == "None")
            {
                _dtAllPayment.DefaultView.RowFilter = "";
            }
            else if (filterColumn == "EnrollmentID")
            {
                if (int.TryParse(txtFilterValue.Text.Trim(), out int studentId))
                {
                    _dtAllPayment.DefaultView.RowFilter = $"[{filterColumn}] = {studentId}";
                }
                else
                {
                    _dtAllPayment.DefaultView.RowFilter = "1 = 0"; // لا يعرض أي صفوف إذا الإدخال غير صحيح
                }
            }
            else
            {
                string escapedText = txtFilterValue.Text.Trim().Replace("'", "''");
                _dtAllPayment.DefaultView.RowFilter = $"[{filterColumn}] LIKE '{escapedText}%'";
            }

            lblRecordCount.Text = dgvPayments.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow).ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmInstallmnetRecord frmInstallmnetRecord = new frmInstallmnetRecord((int)dgvPayments.CurrentRow.Cells[1].Value);
            frmInstallmnetRecord.ShowDialog();
        }
    }
 }

