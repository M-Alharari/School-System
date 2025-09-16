using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SchoolProject
{
    public partial class Form4 : Form
    {
        private string[] Days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
        private string[] TimeSlots = { "08:00 - 09:00", "09:00 - 10:00", "10:00 - 11:00", "11:00 - 12:00", "12:00 - 01:00" };

        private Dictionary<(string day, string time), (string subject, string teacher)> Timetable = new Dictionary<(string, string), (string, string)>();

        // المواد والمعلمين لتعريف التعديلات
        private string[] Subjects = { "Math", "English", "Science", "History", "Art" };
        private string[] Teachers = { "Mr. A", "Ms. B", "Mr. C", "Ms. D", "Mr. E" };

        public Form4()
        {
            InitializeComponent();
            // تأكد من أن DataGridView اسمه dataGridView1 موجود على الفورم


            dataGridView1.Dock = DockStyle.Fill;
            SetupGrid();
            RegenerateTimetable();
            DisplayTimetable();

            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
        }


        private void SetupGrid()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();

            dataGridView1.Columns.Add("Time", "Time");
            foreach (string day in Days)
                dataGridView1.Columns.Add(day, day);

            dataGridView1.Rows.Add(TimeSlots.Length);

            for (int i = 0; i < TimeSlots.Length; i++)
                dataGridView1.Rows[i].Cells[0].Value = TimeSlots[i];

            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.ReadOnly = true;
        }


        private void RegenerateTimetable()
        {
            Random rnd = new Random();
            Timetable.Clear();

            foreach (var time in TimeSlots)
            {
                foreach (var day in Days)
                {
                    int i = rnd.Next(Subjects.Length);
                    Timetable[(day, time)] = (Subjects[i], Teachers[i]);
                }
            }
        }


        private void DisplayTimetable()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string time = row.Cells["Time"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(time)) continue;

                foreach (string day in Days)
                {
                    if (!dataGridView1.Columns.Contains(day)) continue;
                    if (!Timetable.ContainsKey((day, time))) continue;

                    var (subject, teacher) = Timetable[(day, time)];
                    row.Cells[day].Value = subject + "\n" + teacher;
                }
            }
        }


        private void AdjustTimetableLayout()
        {
            // Set size of the DataGridView
            dataGridView1.Width = 894;
            dataGridView1.Height = 596;

            // Set column widths
            dataGridView1.Columns["Time"].Width = 120;

            int dayColumnWidth = (894 - 120) / 5; // Remaining space divided among 5 day columns
            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };

            foreach (string day in days)
            {
                if (dataGridView1.Columns.Contains(day))
                    dataGridView1.Columns[day].Width = dayColumnWidth;
            }

            // Visual settings
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
        }

        private void LoadTimetable()
        {
            DataTable dt = new DataTable();

            // Define columns
            dt.Columns.Add("Time");
            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday" };
            foreach (var day in days)
                dt.Columns.Add(day);

            // Define slots and teachers
            var subjects = new[] { "Math", "English", "Science", "History", "Art" };
            var teachers = new[] { "Mr. Ahmed", "Ms. Sara", "Mr. Khaled", "Ms. Noor", "Mr. Tarek" };
            var timeSlots = new[] { "08:00 - 09:00", "09:00 - 10:00", "10:00 - 11:00", "11:00 - 12:00", "12:00 - 01:00" };

            // Fill data
            for (int i = 0; i < timeSlots.Length; i++)
            {
                DataRow row = dt.NewRow();
                row["Time"] = timeSlots[i];

                for (int j = 0; j < days.Length; j++)
                {
                    int index = (i + j) % subjects.Length;
                    row[days[j]] = subjects[index] + "\n" + teachers[index];
                }

                dt.Rows.Add(row);
            }

            dataGridView1.DataSource = dt;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            AdjustTimetableLayout();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //LoadTimetable();
        }

        private void btnregenerate_Click(object sender, EventArgs e)
        {

            RegenerateTimetable();
            DisplayTimetable();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 1) return; // لا يسمح بتعديل رأس الجدول أو العمود الأول

            string time = dataGridView1.Rows[e.RowIndex].Cells["Time"].Value.ToString();
            string day = dataGridView1.Columns[e.ColumnIndex].Name;

            // المادة والمعلم الحاليين
            var current = Timetable[(day, time)];
            string currentSubject = current.subject;
            string currentTeacher = current.teacher;

            // عرض قائمة لاختيار معلم جديد (هنا بطريقة بسيطة: نستخدم InputBox لاختيار اسم معلم جديد)
            string newTeacher = Microsoft.VisualBasic.Interaction.InputBox(
                $"Current teacher: {currentTeacher}\nEnter new teacher name (choose from: {string.Join(", ", Teachers)}):",
                "Change Teacher",
                currentTeacher);

            if (string.IsNullOrWhiteSpace(newTeacher) || !Teachers.Contains(newTeacher))
            {
                MessageBox.Show("Teacher not changed or invalid teacher.");
                return;
            }

            if (newTeacher == currentTeacher)
            {
                // لا تغيير
                return;
            }

            // إزالة المعلم الجديد من أي خانة أخرى بنفس الوقت (لكي لا يتكرر معلم في نفس التوقيت)
            var keysToRemove = Timetable.Where(kv =>
                kv.Key.time == time && kv.Value.teacher == newTeacher && kv.Key.day != day).Select(kv => kv.Key).ToList();

            foreach (var key in keysToRemove)
            {
                Timetable.Remove(key);
            }

            // تعيين المعلم الجديد في الخانة المختارة مع المادة نفسها
            Timetable[(day, time)] = (currentSubject, newTeacher);

            DisplayTimetable();
        }
    }
}
