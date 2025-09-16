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

namespace SchoolProject.Enrollment_Management
{
    public partial class EnrollmnetsDashboard : Form
    {
        public EnrollmnetsDashboard()
        {
            InitializeComponent();
            LoadDashboard();
        }

        private void LoadDashboard()
        {
            LoadKPIs();
            LoadChartEnrollmentByGender();
            LoadChartYearlyEnrollmentTrend();
            LoadChartGradeDistribution();
        }

        #region KPIs
        private void LoadKPIs()
        {
            int TotalEnrollments = clsEnrollment.GetTotalEnrollments();
            int totalTeachers = clsTeacher.GetTotalTeachers();
            int totalStudents = clsStudent.GetTotalStudents();
            int paidStudents = clsEnrollment.GetConvertedEnrollments();

            lblTotalEnrollments.Text = TotalEnrollments.ToString();
            label12.Text = totalStudents.ToString();
            label10.Text = totalTeachers.ToString();

            lblConversionRate.Text = totalStudents == 0 ? "0%" : $"{((decimal)paidStudents / totalStudents) * 100:0.0}%";
        }
        #endregion

        #region Charts
        private void LoadChartEnrollmentByGender()
        {
            DataTable dt = clsEnrollment.GetEnrollmentByGender();

            chartGender.Series.Clear();
            chartGender.Titles.Clear();
            chartGender.Titles.Add("Enrollment by Gender");

            Series series = new Series
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true // show numbers on chart
            };

            foreach (DataRow row in dt.Rows)
            {
                string gender = row["Gender"].ToString();
                int total = Convert.ToInt32(row["Total"]);

                series.Points.AddXY(gender, total);
            }

            chartGender.Series.Add(series);
        }


        private void LoadChartYearlyEnrollmentTrend()
        {
            DataTable dt = clsEnrollment.GetYearlyEnrollmentTrend();

            chartYearly.Series.Clear();
            chartYearly.Titles.Clear();
            chartYearly.Titles.Add("Yearly Enrollment Trend");

            Series series = new Series
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 3,
                IsValueShownAsLabel = true
            };

            foreach (DataRow row in dt.Rows)
            {
                string year = row["Year"].ToString();
                int total = Convert.ToInt32(row["Total"]);

                series.Points.AddXY(year, total);
            }

            chartYearly.Series.Add(series);
        }

        private void LoadChartGradeDistribution()
        {
            DataTable dt = clsEnrollment.GetGradeDistributionWithRepeaters();

            chartGrade.Series.Clear();
            chartGrade.Titles.Clear();
            chartGrade.Titles.Add("Grade/Class Distribution");

            Series newStudents = new Series("New/Promoted")
            {
                ChartType = SeriesChartType.Bar
            };

            Series repeaters = new Series("Repeaters")
            {
                ChartType = SeriesChartType.Bar
            };

            chartGrade.ChartAreas[0].AxisX.Interval = 1;

            foreach (DataRow row in dt.Rows)
            {
                string grade = row["Grade"].ToString();
                int total = Convert.ToInt32(row["Total"]);
                int repeat = Convert.ToInt32(row["Repeaters"]);
                int fresh = total - repeat;

                newStudents.Points.AddXY(grade, fresh);
                repeaters.Points.AddXY(grade, repeat);
            }

            chartGrade.Series.Add(newStudents);
            chartGrade.Series.Add(repeaters);

            chartGrade.Legends[0].Docking = Docking.Bottom;
        }
        #endregion
    }


































}
 
