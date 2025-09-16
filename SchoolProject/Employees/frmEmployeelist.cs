using SchoolProject.Attendance;
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

namespace SchoolProject.Employees
{
    public partial class frmEmployeelist : Form
    {
        clsEmployee employee;
        private static DataTable _dtAllEmployees = clsEmployee.GetAllEmployees();
        private DataTable _dtEmployees = _dtAllEmployees.DefaultView.ToTable(false, "EmployeeID",
      "PersonID", "FullName", "GenderCaption", "MonthlySalary", "HiredDate",
      "Position", "EmployeeStatus");



        private void _RefreshEmployeesList()
        {

            _dtAllEmployees = clsEmployee.GetAllEmployees(); // ✅ correct source
            _dtEmployees = _dtAllEmployees.DefaultView.ToTable(false, "EmployeeID",
                                                             "PersonID", "FullName", "GenderCaption", "MonthlySalary", "HiredDate",
                                                             "Position", "EmployeeStatus");

            dgvEmployees.DataSource = _dtEmployees;
            lblRecordCount.Text = dgvEmployees.Rows.Count.ToString();


        }

        public frmEmployeelist()
        {
            InitializeComponent();
        }

        private void frmEmployeelist_Load(object sender, EventArgs e)
        {
            dgvEmployees.DataSource = _dtEmployees;
            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            cbFilterBy.SelectedIndex = 0;
            lblRecordCount.Text = dgvEmployees.Rows.Count.ToString();

            if (dgvEmployees.Rows.Count > 0)
            {
                // Hide EmployeeID column
                dgvEmployees.Columns["EmployeeID"].Visible = false;
                dgvEmployees.Columns["PersonID"].Visible = false;
               

                dgvEmployees.Columns[2].HeaderText = "Full Name";
                dgvEmployees.Columns[2].Width = 200;

                dgvEmployees.Columns[3].HeaderText = "Gender";
                dgvEmployees.Columns[3].Width = 80;

                dgvEmployees.Columns[4].HeaderText = "Monthly Salary";
                dgvEmployees.Columns[4].Width = 120;

                dgvEmployees.Columns[5].HeaderText = "Hired Date";
                dgvEmployees.Columns[5].Width = 150;
                dgvEmployees.Columns[5].DefaultCellStyle.Format = "D";

                dgvEmployees.Columns[6].HeaderText = "Position";
                dgvEmployees.Columns[6].Width = 80;

                dgvEmployees.Columns[7].HeaderText = "Employee Status";
                dgvEmployees.Columns[7].Width = 100;
            }

        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "Employee ID":
                    FilterColumn = "EmployeeID";
                    break;
                case "Person ID":
                    FilterColumn = "PersonID";
                    break;

                case "Full Name":
                    FilterColumn = "FullName";
                    break;
                case "Gender":
                    FilterColumn = "GenderCaption";
                    break;

                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtEmployees.DefaultView.RowFilter = "";
                lblRecordCount.Text = dgvEmployees.Rows.Count.ToString();
                return;
            }

            if (FilterColumn == "PersonID" || FilterColumn == "EmployeeID")
            {
                _dtEmployees.DefaultView.RowFilter = string.Format("[{0}]={1}", FilterColumn, txtFilterValue.Text.Trim());
            }
            else
                _dtEmployees.DefaultView.RowFilter = string.Format("[{0}]like'{1}%'", FilterColumn, txtFilterValue.Text.Trim());
            lblRecordCount.Text = dgvEmployees.Rows.Count.ToString();

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowEmployee frm = new frmShowEmployee((int)dgvEmployees.CurrentRow.Cells["EmployeeID"].Value);
            frm.ShowDialog();
            _RefreshEmployeesList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateEmployee frm = new frmAddUpdateEmployee((int)dgvEmployees.CurrentRow.Cells["EmployeeID"].Value);
            frm.ShowDialog();
            _RefreshEmployeesList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ID = (int)dgvEmployees.CurrentRow.Cells[0].Value;
            if (MessageBox.Show($"are you sure you want to delete Employee [{ID}]?", "Confirm Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (clsEmployee.DeleteEmployee(ID))
                {
                    MessageBox.Show("Employee has been deleted successfully", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshEmployeesList();
                }

                else
                    MessageBox.Show("Employee is not deleted due to data connected to it.", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            frmAddUpdateEmployee frm = new frmAddUpdateEmployee();
                    frm.ShowDialog(); _RefreshEmployeesList();
        }

        private void attendanceCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttendanceCard attendanceCard = new AttendanceCard((int)dgvEmployees.CurrentRow.Cells["EmployeeID"].Value);
            attendanceCard.ShowDialog();
        }
    }
}
