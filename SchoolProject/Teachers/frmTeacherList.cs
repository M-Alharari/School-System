using SchoolProject.Attendance;
using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Teachers
{
    public partial class frmTeacherList : Form
    {
        //private static DataTable _dtAllTeachers = clsTeacher.GetAllTeachers();

        private static DataTable _dtAllTeachers;
        private DataTable _AllTeachers;


        private void _RefreshTeachersList()
        {
            _dtAllTeachers = clsTeacher.GetAllTeachers();

            if (_dtAllTeachers == null || _dtAllTeachers.Rows.Count == 0)
            {
                // No data found — clear UI
                _AllTeachers = new DataTable();
                dgvTeachers.DataSource = null;
                lblRecordCount.Text = "0";
                return;
            }

            _AllTeachers = _dtAllTeachers.DefaultView
                .ToTable(false, "TeacherID", "EmployeeID", "FullName");

            dgvTeachers.DataSource = _AllTeachers;
            lblRecordCount.Text = _AllTeachers.Rows.Count.ToString();
        }

        public frmTeacherList()
        {
            InitializeComponent();
        }

        private void frmTeacherList_Load(object sender, EventArgs e)
        {
            cbPeopleFilter.SelectedIndex = 0;

            _dtAllTeachers = clsTeacher.GetAllTeachers() ?? new DataTable();

            if (_dtAllTeachers.Rows.Count > 0)
            {
                _AllTeachers = _dtAllTeachers.DefaultView.ToTable(false, "TeacherID", "EmployeeID", "FullName" );
            }
            else
            {
                _AllTeachers = _dtAllTeachers.Clone(); // creates same columns, but empty table
            }

            dgvTeachers.DataSource = _AllTeachers;
            lblRecordCount.Text = dgvTeachers.Rows.Count.ToString();

            if (dgvTeachers.Rows.Count > 0)
            {
                dgvTeachers.Columns[0].HeaderText = "Teacher ID";
                dgvTeachers.Columns[0].Width = 100;

                dgvTeachers.Columns[1].HeaderText = "EmployeeID";
                dgvTeachers.Columns[1].Width = 100;
                dgvTeachers.Columns[2].HeaderText = "FullName";
                dgvTeachers.Columns[2].Width = 200;
                //dgvTeachers.Columns[3].HeaderText = "Subject Name";
                //dgvTeachers.Columns[3].Width = 100;
                //dgvTeachers.Columns[4].HeaderText = "Class Name";
                //dgvTeachers.Columns[4].Width = 100;
            }


        }

        private void cbPeopleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilter.Visible = (cbPeopleFilter.Text != "None");

            if (txtFilter.Visible)
            {
                txtFilter.Text = "";
                txtFilter.Focus();
            }
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
         
            string FilterColumn = "";

            switch (cbPeopleFilter.Text)
            {
                case "TeacherID":
                    FilterColumn = "TeacherID";
                    break;

                case "Full Name":
                    FilterColumn = "FullName";
                    break;

                case "Class Name":
                    FilterColumn = "ClassName";
                    break;

                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilter.Text.Trim() == "" || FilterColumn == "None")
            {
                _AllTeachers.DefaultView.RowFilter = "";
                lblRecordCount.Text = _AllTeachers.DefaultView.Count.ToString();
                return;
            }

            if (FilterColumn == "TeacherID")
            {
                if (int.TryParse(txtFilter.Text.Trim(), out int filterValue))
                {
                    _AllTeachers.DefaultView.RowFilter = $"[{FilterColumn}] = {filterValue}";
                }
                else
                {
                    _AllTeachers.DefaultView.RowFilter = "";
                }
            }
            else
            {
                _AllTeachers.DefaultView.RowFilter = $"[{FilterColumn}] LIKE '{txtFilter.Text.Trim()}%'";
            }

            lblRecordCount.Text = _AllTeachers.DefaultView.Count.ToString();
        }
        

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTeacherDetails frm = new frmTeacherDetails((int)dgvTeachers.CurrentRow.Cells["TeacherID"].Value);
            frm.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateTeacher frm = new frmAddUpdateTeacher((int)dgvTeachers.CurrentRow.Cells["TeacherID"].Value);
            frm.ShowDialog();
            _RefreshTeachersList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int ID = (int)dgvTeachers.CurrentRow.Cells[0].Value;
            if (MessageBox.Show($"are you sure you want to delete Teacher [{ID}]?", "Confirm Deletion", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                if (clsTeacher.DeleteTeahcer(ID))
                {
                    MessageBox.Show("Teacher has been deleted successfully", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshTeachersList();
                    //frmClassesList_Load(null, null);
                }

                else
                    MessageBox.Show("Teacher is not deleted due to data connected to it.", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void btnAddNewTeacher_Click(object sender, EventArgs e)
        {
            frmAddUpdateTeacher frm = new frmAddUpdateTeacher();
            frm.ShowDialog();
            _RefreshTeachersList();
        }

        private void attendaceCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AttendanceCard attendanceCard = new AttendanceCard((int)dgvTeachers.CurrentRow.Cells["EmployeeID"].Value);
            attendanceCard.ShowDialog();

        }
    }
}
