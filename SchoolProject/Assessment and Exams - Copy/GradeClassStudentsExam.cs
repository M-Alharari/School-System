using SchoolProject.Business;
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

namespace SchoolProject.Assessment_and_Exams
{
    public partial class GradeClassStudentsExam : Form
    {
  

        public GradeClassStudentsExam()
        {
            InitializeComponent();
            cbGrades.SelectedIndexChanged += cbGrades_SelectedIndexChanged;
            cbClasses.SelectedIndexChanged += cbClasses_SelectedIndexChanged;

        }

        private void frmStudentMarksEntry_Load(object sender, EventArgs e)
        {
            cbGrades.DataSource = clsGrade.GetAllGrades();
            cbGrades.DisplayMember = "GradeName";
            cbGrades.ValueMember = "GradeID";

            if (cbGrades.Items.Count > 0)
            {
                cbGrades.SelectedIndex = 0;
                cbGrades_SelectedIndexChanged(cbGrades, EventArgs.Empty);
            }
            else
            {
                cbClasses.DataSource = null;
                cbSubjects.DataSource = null;
                dgvStudents.DataSource = null;
                lblTitle.Text = "No grades found";
                lblRecordCount.Text = "0";
            }

            lblTitle.AutoSize = false;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Width = this.ClientSize.Width;
            lblTitle.Location = new Point(0, 10);
        }

        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbGrades.SelectedIndex == -1 || cbGrades.SelectedValue == null)
            {
                cbClasses.DataSource = null;
                cbSubjects.DataSource = null;
                dgvStudents.DataSource = null;
                lblTitle.Text = "No grade selected";
                lblRecordCount.Text = "0";
                return;
            }

            // Get selected grade ID safely
            int selectedGradeID = cbGrades.SelectedValue is DataRowView drvGrade
                ? Convert.ToInt32(drvGrade["GradeID"])
                : Convert.ToInt32(cbGrades.SelectedValue);

            // Load classes for the grade
            DataTable dtClasses = clsClass.GetClassesByGradeID(selectedGradeID);

            if (dtClasses == null || dtClasses.Rows.Count == 0)
            {
                cbClasses.DataSource = null;
                cbSubjects.DataSource = null;
                dgvStudents.DataSource = null;
                lblTitle.Text = $"No classes found for '{cbGrades.Text}'";
                lblRecordCount.Text = "0";
                return;
            }

            cbClasses.DataSource = dtClasses;
            cbClasses.DisplayMember = "ClassName";
            cbClasses.ValueMember = "ClassID";
            cbClasses.SelectedIndex = 0;

            // Trigger class selection changed manually
            cbClasses_SelectedIndexChanged(cbClasses, EventArgs.Empty);
        }

        private void cbClasses_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbClasses.SelectedIndex == -1 || cbClasses.SelectedValue == null)
            {
                cbSubjects.DataSource = null;
                dgvStudents.DataSource = null;
                lblTitle.Text = "No class selected";
                lblRecordCount.Text = "0";
                return;
            }

            // Get grade ID for subjects
            int selectedGradeID = cbGrades.SelectedValue is DataRowView drvGrade
                ? Convert.ToInt32(drvGrade["GradeID"])
                : Convert.ToInt32(cbGrades.SelectedValue);

            // Load subjects for the grade
            DataTable dtSubjects = clsGradeSubject.GetSubjectsByGradeID(selectedGradeID);

            if (dtSubjects == null || dtSubjects.Rows.Count == 0)
            {
                cbSubjects.DataSource = null;
                dgvStudents.DataSource = null;
                lblTitle.Text = $"No subjects found for '{cbGrades.Text}'";
                lblRecordCount.Text = "0";
                return;
            }

            cbSubjects.DataSource = dtSubjects;
            cbSubjects.DisplayMember = "SubjectName";
            cbSubjects.ValueMember = "SubjectID";
            cbSubjects.SelectedIndex = 0;

            // Optionally trigger subject changed to load grid
            cbSubjects_SelectedIndexChanged(cbSubjects, EventArgs.Empty);
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentDetail frm = new frmStudentDetail((int)dgvStudents.CurrentRow.Cells["EnrollmentID"].Value);
            frm.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int studentID = (int)dgvStudents.CurrentRow.Cells["EnrollmentID"].Value;
            int ScoreID = (int)dgvStudents.CurrentRow.Cells["ScoreID"].Value;
            TypeScores frmScores = new TypeScores(studentID );
            frmScores.ShowDialog();

        }

        private void cbSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClasses.SelectedIndex == -1 || cbSubjects.SelectedIndex == -1)
            {
                dgvStudents.DataSource = null;
                lblTitle.Text = "Select class and subject";
                lblRecordCount.Text = "0";
                return;
            }

            int selectedClassID = cbClasses.SelectedValue is DataRowView drvClass
                ? Convert.ToInt32(drvClass["ClassID"])
                : Convert.ToInt32(cbClasses.SelectedValue);

            int selectedSubjectID = cbSubjects.SelectedValue is DataRowView drvSub
                ? Convert.ToInt32(drvSub["SubjectID"])
                : Convert.ToInt32(cbSubjects.SelectedValue);

            DataTable dtGrid = clsScoresSummary.GetClassStudentScores(selectedClassID, selectedSubjectID);

            if (dtGrid.Rows.Count == 0)
            {
                dgvStudents.DataSource = null;
                lblTitle.Text = $"No scores found for {cbClasses.Text} - {cbSubjects.Text}";
                lblRecordCount.Text = "0";
                return;
            }

            if (!dtGrid.Columns.Contains("LetterGrade"))
                dtGrid.Columns.Add("LetterGrade", typeof(string));

            foreach (DataRow row in dtGrid.Rows)
            {
                decimal total = Convert.ToDecimal(row["TotalScore"]);
                row["LetterGrade"] = clsGrade.GetLetterGrade(total);
            }

            dgvStudents.DataSource = dtGrid;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.Columns["ScoreDetailID"].Visible = false;

            dgvStudents.Columns["EnrollmentID"].Visible = false;
            dgvStudents.Columns["StudentName"].DisplayIndex = 0;
            dgvStudents.Columns["TestScore"].DisplayIndex = 1;
            dgvStudents.Columns["ExamScore"].DisplayIndex = 2;
            dgvStudents.Columns["TotalScore"].DisplayIndex = 3;
            dgvStudents.Columns["LetterGrade"].DisplayIndex = 4;
            dgvStudents.Columns["EndDate"].DisplayIndex = 5;

            lblTitle.Text = $"Students & Scores in {cbClasses.Text} - {cbSubjects.Text}";
            lblRecordCount.Text = dgvStudents.Rows.Count.ToString();
        }
    }
}
