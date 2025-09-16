using SchoolProject.Behaviours;
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

namespace SchoolProject.Enrollment_Management
{
    public partial class frmEnrollmentHistory : Form
    {
        private int _StudentID;
        private DataTable _dtEnrollmentHistory;
        public frmEnrollmentHistory(int studentID)
        {
            InitializeComponent(); _StudentID = studentID; ctrlStudentCard1.LoadInfo(studentID);
        }

        private void frmEnrollmentHistorys_Load(object sender, EventArgs e)
        {

            LoadEnrollmentHistory(_StudentID);
            LoadBehaviourHistory(_StudentID); // 👈 new line

        }



        private void LoadEnrollmentHistory(int studentID)
        {
            try
            {
                // Example: replace with your actual method to fetch data
                DataTable _dtEnrollmentHistory = clsEnrollmentHistory.GetStudentHistory(studentID);

                if (_dtEnrollmentHistory == null || _dtEnrollmentHistory.Rows.Count == 0)
                {
                    dgvHistory.DataSource = null;
                    lblRecordCount.Text = "0";
                    return;
                }

                dgvHistory.DataSource = _dtEnrollmentHistory;
                lblRecordCount.Text = _dtEnrollmentHistory.Rows.Count.ToString();

                // Hide IDs
                if (dgvHistory.Columns.Contains("EnrollmentID"))
                    dgvHistory.Columns["EnrollmentID"].Visible = false;

                if (dgvHistory.Columns.Contains("EnrollmentID"))
                    dgvHistory.Columns["EnrollmentID"].Visible = false;

                // Set headers and widths
                if (dgvHistory.Columns.Contains("FullName"))
                {
                    dgvHistory.Columns["FullName"].HeaderText = "Student Name";
                    dgvHistory.Columns["FullName"].Width = 180;
                }

                if (dgvHistory.Columns.Contains("GradeName"))
                {
                    dgvHistory.Columns["GradeName"].HeaderText = "Grade";
                    dgvHistory.Columns["GradeName"].Width = 60;
                }

                if (dgvHistory.Columns.Contains("ClassName"))
                {
                    dgvHistory.Columns["ClassName"].HeaderText = "Class";
                    dgvHistory.Columns["ClassName"].Width = 60;
                }

                if (dgvHistory.Columns.Contains("TermPeriod"))
                {
                    dgvHistory.Columns["TermPeriod"].HeaderText = "Term Period";
                    dgvHistory.Columns["TermPeriod"].Width = 140;
                }

                if (dgvHistory.Columns.Contains("FinalAverage"))
                {
                    dgvHistory.Columns["FinalAverage"].HeaderText = "Final Avg";
                    dgvHistory.Columns["FinalAverage"].Width = 80;
                }

                if (dgvHistory.Columns.Contains("Status"))
                {
                    dgvHistory.Columns["Status"].HeaderText = "Status";
                    dgvHistory.Columns["Status"].Width = 60;
                }

                if (dgvHistory.Columns.Contains("EnrollmentDate"))
                {
                    dgvHistory.Columns["EnrollmentDate"].HeaderText = "Enrolled On";
                    dgvHistory.Columns["EnrollmentDate"].Width = 120;
                    dgvHistory.Columns["EnrollmentDate"].Visible = false;
                }

                if (dgvHistory.Columns.Contains("HistoryGraduated"))
                {
                    dgvHistory.Columns["HistoryGraduated"].HeaderText = "Prev Status";
                    dgvHistory.Columns["HistoryGraduated"].Width = 100;
                    //dgvHistory.Columns["HistoryGraduated"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading enrollment history: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadBehaviourHistory(int studentID)
        {
            try
            {
                DataTable dtBehaviour = clsStudentBehaviour.GetEnrollmentBehaviourSummaryforterms(studentID);

                if (dtBehaviour == null || dtBehaviour.Rows.Count == 0)
                {
                    dgvBehaviour.DataSource = null;
                    lblBehaviourCount.Text = "0";
                    return;
                }

                dgvBehaviour.DataSource = dtBehaviour;
                lblBehaviourCount.Text = dtBehaviour.Rows.Count.ToString();

                // Make columns auto-size to fill the grid
                dgvBehaviour.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                // Optional: customize headers (not widths, since fill will handle them)
                if (dgvBehaviour.Columns.Contains("BehaviourType"))
                    dgvBehaviour.Columns["BehaviourType"].HeaderText = "Type";

                if (dgvBehaviour.Columns.Contains("Severity"))
                    dgvBehaviour.Columns["Severity"].HeaderText = "Severity";

                if (dgvBehaviour.Columns.Contains("ActionTaken"))
                    dgvBehaviour.Columns["ActionTaken"].HeaderText = "Action Taken";

                if (dgvBehaviour.Columns.Contains("Description"))
                    dgvBehaviour.Columns["Description"].HeaderText = "Description";

                if (dgvBehaviour.Columns.Contains("DateRecorded"))
                    dgvBehaviour.Columns["DateRecorded"].HeaderText = "Date";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading behaviour history: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InternationalLicenseHistorytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentBehaviorslist frmStudentBehaviorslist = new frmStudentBehaviorslist(_StudentID);
            frmStudentBehaviorslist.ShowDialog();
        }
    }
}
