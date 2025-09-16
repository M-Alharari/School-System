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
    public partial class frmCompareGradesScores : Form
    {
        public frmCompareGradesScores()
        {
            InitializeComponent();
            LoadGrades();
            LoadTerms();

            // Attach event handlers
            comboBoxGrade.SelectedIndexChanged += ComboBoxes_SelectedIndexChanged;
            comboBoxTerm.SelectedIndexChanged += ComboBoxes_SelectedIndexChanged;

            // Use Shown event to update chart after everything is ready
            this.Shown += FrmCompareGradesScores_Shown;
        }
        private void FrmCompareGradesScores_Shown(object sender, EventArgs e)
        {
            // Update chart manually on first load
            UpdateChartForSelected();
        }
        private void LoadGrades()
        {
            comboBoxGrade.DataSource = clsGrade.GetAllGrades();
            comboBoxGrade.DisplayMember = "GradeName";
            comboBoxGrade.ValueMember = "GradeID";

            if (comboBoxGrade.Items.Count > 0)
                comboBoxGrade.SelectedIndex = 0;
        }

        private void LoadTerms()
        {
            comboBoxTerm.DataSource = clsTerm.GetAll();
            comboBoxTerm.DisplayMember = "TermName";
            comboBoxTerm.ValueMember = "TermID";

            if (comboBoxTerm.Items.Count > 0)
                comboBoxTerm.SelectedIndex = 0;
        }

        private void ComboBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateChart(DataTable dt)
        {
            chartClasses.Series.Clear();
            chartClasses.ChartAreas[0].AxisX.Title = "Classes";
            chartClasses.ChartAreas[0].AxisY.Title = "Average Total Score";

            if (dt.Rows.Count == 0)
                return;

            // Find highest and lowest
            DataRow highest = dt.AsEnumerable().OrderByDescending(r => r.Field<decimal>("AvgTotalScore")).First();
            DataRow lowest = dt.AsEnumerable().OrderBy(r => r.Field<decimal>("AvgTotalScore")).First();

            Series series = new Series("Average Score")
            {
                ChartType = SeriesChartType.Bar,
                IsValueShownAsLabel = true
            };

            foreach (DataRow row in dt.Rows)
            {
                decimal score = row.Field<decimal>("AvgTotalScore");
                string className = row.Field<string>("ClassName");

                DataPoint point = new DataPoint();
                point.AxisLabel = className;
                point.YValues = new double[] { (double)score };
                point.Label = score.ToString("0.##");

                // Highlight highest and lowest with different colors and tooltip
                if (row == highest)
                {
                    point.Color = Color.Green;
                    point.ToolTip = $"Highest: {className} ({score})";
                }
                else if (row == lowest)
                {
                    point.Color = Color.Red;
                    point.ToolTip = $"Lowest: {className} ({score})";
                }
                else
                {
                    point.Color = Color.Blue;
                    point.ToolTip = $"{className} ({score})";
                }

                series.Points.Add(point);
            }

            chartClasses.Series.Add(series);
        }

        private void frmCompareGradesScores_Load(object sender, EventArgs e)
        {
          
        }

        private void comboBoxGrade_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChartForSelected(); // chart still shows selected grade
            if (comboBoxTerm.SelectedValue != null && !(comboBoxTerm.SelectedValue is DataRowView))
            {
                int termID = Convert.ToInt32(comboBoxTerm.SelectedValue);
               
            }
        }


        private void UpdateChartForSelected()
        {
            if (comboBoxGrade.SelectedValue == null || comboBoxTerm.SelectedValue == null)
                return;

            if (comboBoxGrade.SelectedValue is DataRowView || comboBoxTerm.SelectedValue is DataRowView)
                return;

            int gradeID = Convert.ToInt32(comboBoxGrade.SelectedValue);
            int termID = Convert.ToInt32(comboBoxTerm.SelectedValue);

            DataTable dt = clsScores.GetClassAverageScores(gradeID, termID);
            UpdateChart(dt);

            // Update highest/lowest grade labels
            UpdateGradeLabels();
        }


        private void comboBoxTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateChartForSelected();
        }

        private void UpdateGradeLabels()
        {
            if (comboBoxTerm.SelectedValue == null || comboBoxTerm.SelectedValue is DataRowView)
                return;

            int termID = Convert.ToInt32(comboBoxTerm.SelectedValue);
            var (highest, lowest) = clsScores.GetHighestLowestGrade(termID);

            lblHighestGrade.Text = highest != null
                ? $"Highest: {highest["GradeName"]} ({highest["GradeAverage"]:0.##})"
                : "Highest: N/A";

            lblLowestGrade.Text = lowest != null
                ? $"Lowest: {lowest["GradeName"]} ({lowest["GradeAverage"]:0.##})"
                : "Lowest: N/A";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }





        //private void LoadGradeComparison(int termID)
        //{
        //    DataTable dt = clsScores.GetGradesAverageScores(termID);

        //    chart1.Series.Clear();
        //    var series = chart1.Series.Add("Grades Avg Scores");
        //    series.ChartType = SeriesChartType.Column;

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        string gradeName = row["GradeName"].ToString();
        //        double avg = Convert.ToDouble(row["AverageScore"]);
        //        series.Points.AddXY(gradeName, avg);
        //    }
        //}






















    }
}
 

 