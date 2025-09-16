using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.TimeTable
{
    public partial class ViewTimeTable : Form
    {
        private string connectionString = @"Server=YOUR_SERVER;Database=SchoolManagementDB;Trusted_Connection=True;";
        public ViewTimeTable()
        {
            InitializeComponent();
            LoadGrades();
        } 

            private void LoadGrades()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("SELECT GradeID, GradeName FROM Grades ORDER BY GradeID", conn);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                cmbGrades.DataSource = dt;
                cmbGrades.DisplayMember = "GradeName";
                cmbGrades.ValueMember = "GradeID";

                cmbGrades.SelectedIndexChanged += CmbGrades_SelectedIndexChanged;
            }
        }

        private void CmbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGrades.SelectedValue == null) return;

            int gradeId = (int)cmbGrades.SelectedValue;
            LoadClassesForGrade(gradeId);
        }

        private void LoadClassesForGrade(int gradeId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(
                    "SELECT ClassID, ClassName FROM Classes WHERE GradeID = @GradeID ORDER BY ClassName",
                    conn);
                adapter.SelectCommand.Parameters.AddWithValue("@GradeID", gradeId);

                DataTable dt = new DataTable();
                adapter.Fill(dt);

                dgvClasses.DataSource = dt;

                // أخفي عمود الـClassID من العرض لو تحب
                if (dgvClasses.Columns.Contains("ClassID"))
                    dgvClasses.Columns["ClassID"].Visible = false;
            }
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (dgvClasses.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد فصول لتوليد الجداول الزمنية لها.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow row in dgvClasses.Rows)
            {
                if (row.IsNewRow) continue;

                int classId = Convert.ToInt32(row.Cells["ClassID"].Value);
                GenerateTimetableForClass(classId);
            }

            MessageBox.Show("تم توليد الجداول الزمنية لجميع الفصول المحددة بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void GenerateTimetableForClass(int classId)
        {
            // نفذ عملية توليد الجدول الزمني بنفس الطريقة التي تريدها
            // الكود هنا يعتمد على كود التوليد في الـ business layer/data layer

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    DECLARE @Days TABLE (DayOfWeek NVARCHAR(10));
                    INSERT INTO @Days VALUES ('Sunday'), ('Monday'), ('Tuesday'), ('Wednesday'), ('Thursday');

                    INSERT INTO TimetableEntries (DayOfWeek, TimeSlotID, SubjectID, TeacherID, ClassID)
                    SELECT 
                        d.DayOfWeek,
                        ts.TimeSlotID,
                        ta.SubjectID,
                        ta.TeacherID,
                        c.ClassID
                    FROM Classes c
                    JOIN TeacherAssignments ta 
                        ON ta.GradeID = c.GradeID
                    CROSS JOIN @Days d
                    JOIN TimeSlots ts 
                        ON ts.IsBreak = 0
                    WHERE c.ClassID = @classId
                      AND NOT EXISTS (
                        SELECT 1 FROM TimetableEntries t
                        WHERE t.DayOfWeek = d.DayOfWeek
                          AND t.TimeSlotID = ts.TimeSlotID
                          AND (t.TeacherID = ta.TeacherID OR t.ClassID = c.ClassID)
                      )
                    ORDER BY c.ClassID, d.DayOfWeek, ts.TimeSlotID;
                ";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@classId", classId);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
