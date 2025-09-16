 
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
    public partial class frmAttendaceManagement : Form
    {
        private string _personType = "Employees"; // default shown in grid

        private static DataTable _dtAllAttendance = clsEmployeeAttendance.GetAllAttendance();

        private DataTable _dtAttendance = _dtAllAttendance.DefaultView.ToTable(false,
      "AttendanceID", "EmployeeID", "AttendanceDate", "IsPresent", "AbsenceReason");

        private void _RefreshAttendanceList()
        {
            DataTable dtAll = null;

            if (_personType == "Employees")
            {
                dtAll = clsEmployeeAttendance.GetAllAttendance();
                if (dtAll.Columns.Contains("EmployeeID") && !dtAll.Columns.Contains("PersonID"))
                    dtAll.Columns["EmployeeID"].ColumnName = "PersonID";

                lblTitle.Text = "Manage Employees Attendance"; // update title for employees
            }
            else if (_personType == "Students")
            {
                dtAll = clsStudentAttendance.GetAllAttendance();
                if (dtAll.Columns.Contains("StudentID") && !dtAll.Columns.Contains("PersonID"))
                    dtAll.Columns["StudentID"].ColumnName = "PersonID";

                lblTitle.Text = "Manage Students Attendance"; // update title for students
            }

            _dtAttendance = dtAll ?? new DataTable();
            dgvAttendance.DataSource = _dtAttendance;

            lblRecordCount.Text = dgvAttendance.Rows.Count.ToString();

            if (dgvAttendance.Rows.Count > 0)
            {
                dgvAttendance.Columns["AttendanceID"].HeaderText = "Attendance ID";

                if (dgvAttendance.Columns.Contains("FullName"))
                    dgvAttendance.Columns["FullName"].HeaderText = "Full Name";
                else
                    dgvAttendance.Columns["PersonID"].HeaderText = "PersonID";

                dgvAttendance.Columns["AttendanceDate"].HeaderText = "Date";
                dgvAttendance.Columns["IsPresent"].HeaderText = "Present?";
                dgvAttendance.Columns["AbsenceReason"].HeaderText = "Reason";
            }

            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public frmAttendaceManagement()
        {
            InitializeComponent();
        }

        private void frmAttendaceManagement_Load(object sender, EventArgs e)
        {    // Initialize mode ComboBox
            cbMode.Items.Clear();
            cbMode.Items.Add("Employees");
            cbMode.Items.Add("Students");
            cbMode.SelectedIndex = 0; // default Employees

            // Initialize filter ComboBox
            cbFilterBy.SelectedIndex = 0;

            // Load attendance for the default mode
            _RefreshAttendanceList();

            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";

            switch (cbFilterBy.Text)
            {
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
                _dtAttendance.DefaultView.RowFilter = "";
                lblRecordCount.Text = dgvAttendance.Rows.Count.ToString();
                return;
            }

            _dtAttendance.DefaultView.RowFilter = string.Format("[{0}] like '{1}%'", FilterColumn, txtFilterValue.Text.Trim());
            lblRecordCount.Text = dgvAttendance.Rows.Count.ToString();
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
        private void OpenDailyAttendance(string personType)
        {
            frmDailyAttendance frmDailyAttendance = new frmDailyAttendance(personType);
            frmDailyAttendance.ShowDialog();

            // After closing the dialog, refresh the grid in main form
            _personType = personType; // store which type is currently displayed
            _RefreshAttendanceList();
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            // Pass the current grid type to frmDailyAttendance
            frmDailyAttendance frmDailyAttendance = new frmDailyAttendance(_personType);
            frmDailyAttendance.ShowDialog();

            // Refresh grid after closing form
            _RefreshAttendanceList();
        }

        private void cmMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMode.SelectedItem == null) return;

            // Update the current person type
            _personType = cbMode.SelectedItem.ToString();

            // Refresh grid based on new type
            _RefreshAttendanceList();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMode.SelectedItem == null) return;

            // Update the current person type
            _personType = cbMode.SelectedItem.ToString();

            // Refresh grid based on new type
            _RefreshAttendanceList();
        }
    }
}
