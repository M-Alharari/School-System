using SchoolProject.Employees;
using SchoolProject.People;
using SchoolProject.Teachers;
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

namespace SchoolProject.Attendance
{
    public partial class AttendanceCard : Form
    {
        private int _EmployeeID = -1;

       
        public AttendanceCard(int EmployeeID)
        {
            InitializeComponent();  
            _EmployeeID = EmployeeID;
        }

        private void LoadTeacherAttendanceSummary(int EmployeeID, int month, int year)
        {
            if (EmployeeID <= 0)
            {
                ClearLabels();
                MessageBox.Show("Invalid Teacher ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable dt = clsAttendanceRecord.GetSummaryForTeacher(EmployeeID);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                lblFullName.Text = row["FullName"].ToString(); // ← هنا
                lblTotalDays.Text = row["TotalDays"].ToString();
                lblPresentDays.Text = row["DaysPresent"].ToString();
                lblLastDayPresent.Text = row["LastDayPresent"].ToString();
                lblAttendancePercentage.Text = string.Format("{0:0.00} %", Convert.ToDouble(row["AttendancePercentage"]));
                
                
                int totalDays = Convert.ToInt32(row["TotalDays"]);
                int presentDays = Convert.ToInt32(row["DaysPresent"]);

                LoadAttendanceChart(totalDays, presentDays);

            }
            else
            {
                ClearLabels();
                MessageBox.Show("No attendance data found for this teacher.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void LoadAttendanceChart(int totalDays, int daysPresent)
        {
            int daysAbsent = totalDays - daysPresent;
            if (daysAbsent < 0) daysAbsent = 0;

            chartAttendance.Series.Clear();
            chartAttendance.Titles.Clear();

            // Create the series
            Series series = new Series("Attendance");
            series.ChartType = SeriesChartType.Pie;

            // Add data points
            series.Points.AddXY("Present", daysPresent);
            series.Points.AddXY("Absent", daysAbsent);

            // Optional styling
            series.Points[0].Color = Color.FromArgb(76, 175, 80);   // Green
            series.Points[1].Color = Color.FromArgb(244, 67, 54);   // Red

            series.Points[0].Label = $"Present: {daysPresent}";
            series.Points[1].Label = $"Absent: {daysAbsent}";

            chartAttendance.Series.Add(series);

            // Add title (optional)
            chartAttendance.Titles.Add("Attendance Breakdown");
        }


        private void ClearLabels()
        {
            lblFullName.Text = "-";
            lblTotalDays.Text = "-";
            lblPresentDays.Text = "-";
            lblLastDayPresent.Text = "-";
            lblAttendancePercentage.Text = "-";
        }
    
        private void AttendanceCard_Load(object sender, EventArgs e)
        {
            //LoadTeacherInfo(_EmployeeID);
            LoadTeacherAttendanceSummary(_EmployeeID, DateTime.Now.Month, DateTime.Now.Year);

        }

        private void llTeacherInfoCard_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            clsEmployee Emp = clsEmployee.FindByEmployeeID(_EmployeeID);
            if (Emp == null)
            {
                MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmShowEmployee frmTeacherDetails = new frmShowEmployee(_EmployeeID);
            frmTeacherDetails.ShowDialog();


        }
    }
}
