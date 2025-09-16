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

namespace SchoolProject.Comparisons.Trends
{
    public partial class frmStudentPerformanceTrends : Form
    {
        public frmStudentPerformanceTrends()
        {
            InitializeComponent();
        }

        private void frmStudentPerformanceTrends_Load(object sender, EventArgs e)
        {

            // Load grades
            LoadGrades();

            if (cbGrades.Items.Count > 0)
            {
                cbGrades.SelectedIndex = 0; // select first grade

                // Explicitly load classes and students for the first grade
                LoadClassesAndStudents();
            }
        }
        private void LoadClassesAndStudents()
        {
            if (cbGrades.SelectedValue == null || cbGrades.SelectedValue is DataRowView) return;

            int gradeID = Convert.ToInt32(cbGrades.SelectedValue);

            // Load classes for selected grade
            DataTable dtClasses = clsClass.GetClassesByGradeID(gradeID);
            cbClasses.DataSource = dtClasses;
            cbClasses.DisplayMember = "ClassName";
            cbClasses.ValueMember = "ClassID";

            if (cbClasses.Items.Count > 0)
            {
                cbClasses.SelectedIndex = 0; // triggers student load below

                // Load students for first class
                if (cbClasses.SelectedValue != null)
                {
                    int classID = Convert.ToInt32(cbClasses.SelectedValue);
                    DataTable dtStudents = clsStudent.GetStudentsByClassID(classID);
                    cbStudents.DataSource = dtStudents;
                    cbStudents.DisplayMember = "FullName";
                    cbStudents.ValueMember = "StudentID";

                    if (cbStudents.Items.Count > 0)
                    {
                        cbStudents.SelectedIndex = 0;

                        int studentID = Convert.ToInt32(cbStudents.SelectedValue);
                        LoadStudentInfo(studentID);
                        LoadStudentChart(studentID);
                    }
                }
            }
            else
            {
                cbStudents.DataSource = null;
                chartTrends.Series.Clear();
            }
        }



        // Load grades
        private void LoadGrades()
        {
            DataTable dt = clsGrade.GetAllGrades();
            cbGrades.DataSource = dt;
            cbGrades.DisplayMember = "GradeName";
            cbGrades.ValueMember = "GradeID";

            // After setting DataSource, automatically trigger cbGrades_SelectedIndexChanged
            if (cbGrades.Items.Count > 0)
            {
                cbGrades.SelectedIndex = 0;
            }
        }


        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)

        {
            // When user changes grade manually, reload classes and students
            LoadClassesAndStudents();
        }

        private void LoadStudentInfo(int studentID)
        {
            // Get student info including full name and tuition status
            DataRow studentRow = clsStudent.GetStudentInfoWithTuition(studentID);
            if (studentRow != null)
            {
                lblFullName.Text = studentRow["FullName"].ToString();
                lblTuitionStatus.Text = studentRow["TuitionStatus"].ToString();
            }

            // Get average grade
            double avgGrade = clsTrends.GetStudentAverageGrade(studentID);
            lblAvgGrade.Text = avgGrade.ToString("F2");

            // Get average attendance
            double avgAttendance = clsTrends.GetStudentAverageAttendance(studentID);
            lblAvgAttendance.Text = avgAttendance.ToString("F2") + "%";
        }

        private void cbClasses_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbClasses.SelectedValue == null || cbClasses.SelectedValue is DataRowView) return;

            int classID = Convert.ToInt32(cbClasses.SelectedValue);

            // Load students for the selected class
            DataTable dtStudents = clsStudent.GetStudentsByClassID(classID);
            cbStudents.DataSource = dtStudents;
            cbStudents.DisplayMember = "FullName";
            cbStudents.ValueMember = "StudentID";

            if (cbStudents.Items.Count > 0)
            {
                cbStudents.SelectedIndex = 0; // triggers student info load
                int studentID = Convert.ToInt32(cbStudents.SelectedValue);

                // Load chart and student info
                LoadStudentInfo(studentID);
                LoadStudentChart(studentID);
            }
            else
            {
                chartTrends.Series.Clear();
            }

        }
        private void LoadStudentChart(int studentID, int termID = 0)
        {
            // Clear existing series
            chartTrends.Series.Clear();
            chartTrends.ChartAreas[0].AxisX.Title = "Term";
            chartTrends.ChartAreas[0].AxisY.Title = "Average Score";
            chartTrends.ChartAreas[0].AxisY.Minimum = 0;
            chartTrends.ChartAreas[0].AxisY.Maximum = 100;

            // Get student scores from business layer
            DataTable dtScores = (termID > 0)  ? clsTrends.GetStudentScoresByTerms(studentID)
                /* ? clsTrends.GetStudentScoresByTerm(studentID, termID) */// Returns scores for specific term
                : clsTrends.GetStudentScoresByTerms(studentID);       // Returns scores for all terms

            if (dtScores == null || dtScores.Rows.Count == 0)
                return; // Nothing to plot

            // Create series
            var series = chartTrends.Series.Add("Average Score");
            series.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            series.Color = Color.SteelBlue;  // Customize color
            series.BorderWidth = 2;

            foreach (DataRow row in dtScores.Rows)
            {
                string termName = row["TermName"].ToString();
                double score = Convert.ToDouble(row["AvgScore"]);
                series.Points.AddXY(termName, score);
            }

            // Customize chart area
            chartTrends.ChartAreas[0].AxisX.Title = "Term";
            chartTrends.ChartAreas[0].AxisY.Title = "Average Score";
            chartTrends.ChartAreas[0].AxisY.Minimum = 0;
            chartTrends.ChartAreas[0].AxisY.Maximum = 100;
            chartTrends.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartTrends.ChartAreas[0].AxisX.Interval = 1;



            foreach (DataRow row in dtScores.Rows)
            {
                string termName = row["TermName"].ToString();
                double score = Convert.ToDouble(row["AvgScore"]);
                series.Points.AddXY(termName, score);
            }

            // Optional: improve X-axis labels
            chartTrends.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chartTrends.ChartAreas[0].AxisX.Interval = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           
        }
        

        private void cbTerms_SelectedIndexChanged(object sender, EventArgs e)
        {

             
        }
    }

}