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
using System.Windows.Forms.DataVisualization.Charting;

namespace SchoolProject.Comparisons
{
    public partial class frmCompareStudentsInAClass : Form
    {
        public frmCompareStudentsInAClass()
        {
            InitializeComponent();
            LoadGrades();
            LoadTerms();

            // load first class for the default grade
            if (comboBoxGrade.SelectedValue != null && !(comboBoxGrade.SelectedValue is DataRowView))
            {
                int gradeID = Convert.ToInt32(comboBoxGrade.SelectedValue);
                LoadClasses(gradeID);
            }

            comboBoxGrade.SelectedIndexChanged += ComboBoxes_SelectedIndexChanged;
            comboBoxClass.SelectedIndexChanged += ComboBoxes_SelectedIndexChanged;
            comboBoxTerm.SelectedIndexChanged += ComboBoxes_SelectedIndexChanged;

            this.Shown += FrmCompareStudentsScores_Shown;
        }


        private void FrmCompareStudentsScores_Shown(object sender, EventArgs e)
        {
            UpdateChartForSelected();
        }
        private void LoadGrades()
        {
            comboBoxGrade.DataSource = clsGrade.GetAllGrades();
            comboBoxGrade.DisplayMember = "GradeName";
            comboBoxGrade.ValueMember = "GradeID";
            if (comboBoxGrade.Items.Count > 0)
                comboBoxGrade.SelectedIndex = 0; // default first grade
        }

        private void LoadClasses(int gradeID)
        {
            comboBoxClass.DataSource = clsClass.GetClassesByGradeID(gradeID);
            comboBoxClass.DisplayMember = "ClassName";
            comboBoxClass.ValueMember = "ClassID";
            if (comboBoxClass.Items.Count > 0)
                comboBoxClass.SelectedIndex = 0; // default first class
        }

        private void LoadTerms()
        {
            comboBoxTerm.DataSource = clsTerm.GetAll();
            comboBoxTerm.DisplayMember = "TermName";
            comboBoxTerm.ValueMember = "TermID";
            if (comboBoxTerm.Items.Count > 0)
                comboBoxTerm.SelectedIndex = 0; // default first term
        }

        private void ComboBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == comboBoxGrade && comboBoxGrade.SelectedValue != null && !(comboBoxGrade.SelectedValue is DataRowView))
            {
                int gradeID = Convert.ToInt32(comboBoxGrade.SelectedValue);
                LoadClasses(gradeID);
            }
            UpdateChartForSelected();
        }

        private void UpdateChart(DataTable dt)
        {
            chartStudents.Series.Clear();
            chartStudents.ChartAreas[0].AxisX.Title = "Students";
            chartStudents.ChartAreas[0].AxisY.Title = "Average Score";

            if (dt.Rows.Count == 0)
                return;

            var highest = dt.AsEnumerable().OrderByDescending(r => r.Field<decimal>("AvgScore")).First();
            var lowest = dt.AsEnumerable().OrderBy(r => r.Field<decimal>("AvgScore")).First();

            Series series = new Series("Student Scores")
            {
                ChartType = SeriesChartType.Column,
                IsValueShownAsLabel = true
            };

            foreach (DataRow row in dt.Rows)
            {
                decimal score = row.Field<decimal>("AvgScore");
                string studentName = row.Field<string>("FullName");

                DataPoint point = new DataPoint();
                point.AxisLabel = studentName;
                point.YValues = new double[] { (double)score };
                point.Label = score.ToString("0.##");

                if (row == highest)
                {
                    point.Color = Color.Green;
                    point.ToolTip = $"Top: {studentName} ({score})";
                }
                else if (row == lowest)
                {
                    point.Color = Color.Red;
                    point.ToolTip = $"Lowest: {studentName} ({score})";
                }
                else
                {
                    point.Color = Color.Blue;
                    point.ToolTip = $"{studentName} ({score})";
                }

                series.Points.Add(point);
            }

            chartStudents.Series.Add(series);
        }

        private void UpdateChartForSelected()
        {
            if (comboBoxGrade.SelectedValue == null || comboBoxClass.SelectedValue == null || comboBoxTerm.SelectedValue == null)
                return;

            if (comboBoxGrade.SelectedValue is DataRowView ||
                comboBoxClass.SelectedValue is DataRowView ||
                comboBoxTerm.SelectedValue is DataRowView)
                return;

            int gradeID = Convert.ToInt32(comboBoxGrade.SelectedValue);
            int classID = Convert.ToInt32(comboBoxClass.SelectedValue);
            int termID = Convert.ToInt32(comboBoxTerm.SelectedValue);

            DataTable dt = clsScores.GetStudentsAverageScores(gradeID, classID, termID);
            UpdateChart(dt);
        }

    }





























}
 
