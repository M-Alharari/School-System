using SchoolProject.Assessment_and_Exams;
using SchoolProject.Behaviours;
using SchoolProject.Enrollment_Management;
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

namespace SchoolProject.Students
{
    public partial class frmStudentList : Form
    {

        private DataTable _dtAllStudents;
        private DataTable _dtStudents;

        private void _RefreshStudentList()
        {
            _dtAllStudents = clsStudent.GetAllStudents() ?? new DataTable();

            // Only create _dtStudents if required columns exist
            if (_dtAllStudents.Columns.Contains("StudentID") &&
                _dtAllStudents.Columns.Contains("FullName"))
            {
                _dtStudents = _dtAllStudents.DefaultView.ToTable(false,
                   "EnrollmentID", "StudentID", "FullName", "GenderCaption", "CountryName", "GradeName", "ClassName");
            }
            else
            {
                _dtStudents = new DataTable();
            }

            dgvStudents.DataSource = _dtStudents;
            lblRecordCount.Text = _dtStudents.Rows.Count.ToString();
        }

        private void FormatGridColumns()
        {
            dgvStudents.Columns[0].HeaderText = "EnrollmentID";
            dgvStudents.Columns[0].Width = 80;

            dgvStudents.Columns[1].HeaderText = "StudentID";  // ✅ new column
            dgvStudents.Columns[1].Width = 80;

            dgvStudents.Columns[2].HeaderText = "Full Name";
            dgvStudents.Columns[2].Width = 120;

            dgvStudents.Columns[3].HeaderText = "Gender";
            dgvStudents.Columns[3].Width = 90;

            dgvStudents.Columns[4].HeaderText = "Country";
            dgvStudents.Columns[4].Width = 90;

            dgvStudents.Columns[5].HeaderText = "Grade";
            dgvStudents.Columns[5].Width = 90;

            dgvStudents.Columns[6].HeaderText = "Class";
            dgvStudents.Columns[6].Width = 90;

        }

        public frmStudentList()
        {
            InitializeComponent();
        }

        private void frmStudentList_Load(object sender, EventArgs e)
        {
            _RefreshStudentList();

            cbFilterBy.SelectedIndex = 0;

            dgvStudents.DataSource = _dtStudents;
            lblRecordCount.Text = _dtStudents.Rows.Count.ToString();

            if (_dtStudents.Rows.Count > 0)
                FormatGridColumns();
        }

        private void btnAddNewStudent_Click(object sender, EventArgs e)
        {
            frmAddUpdateStudent frm = new frmAddUpdateStudent();
            frm.ShowDialog();
            _RefreshStudentList(); 

        }

        private void showDeatilsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentDetail frm = new frmStudentDetail((int)dgvStudents.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateStudent frm = new frmAddUpdateStudent();
            frm.ShowDialog();
            _RefreshStudentList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateStudent frm = new frmAddUpdateStudent((int)dgvStudents.CurrentRow.Cells[1].Value);
            frm.ShowDialog();
            _RefreshStudentList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
        "Are you sure you want to delete Student [" + dgvStudents.CurrentRow.Cells[0].Value + "]",
        "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                int currentUser = clsGlobal.CurrentUser.UserID;

                // Perform Delete and refresh
                if (clsEnrollment.DeactivateEnrollment((int)dgvStudents.CurrentRow.Cells[0].Value, currentUser))
                {
                    MessageBox.Show("Student Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshStudentList();
                }
                else
                {
                    MessageBox.Show("Person was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmEnrollmentHistory frmEnrollmentHistorys = new frmEnrollmentHistory((int)dgvStudents.CurrentRow.Cells[1].Value);
            frmEnrollmentHistorys.ShowDialog();
        }

        private void callPhoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnterScores frmEnrollmentHistorys = new EnterScores((int)dgvStudents.CurrentRow.Cells[0].Value);
            frmEnrollmentHistorys.ShowDialog();
        }

        private void behavioursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentBehaviorslist frmStudentBehaviorslist = new frmStudentBehaviorslist((int)dgvStudents.CurrentRow.Cells[0].Value);
            frmStudentBehaviorslist.ShowDialog();
        }

        private void behavioursLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentBehaviorslist frmStudentBehaviorslist = new frmStudentBehaviorslist((int)dgvStudents.CurrentRow.Cells[0].Value);
            frmStudentBehaviorslist.ShowDialog();

        }
    }
}
