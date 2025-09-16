using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject
{
    public partial class TimeTableGenerator : Form
    {
        public TimeTableGenerator()
        {
            InitializeComponent();
        }

        private void TimeTableGenerator_Load(object sender, EventArgs e)
        {


            LoadGrades();

            // Automatically select the first grade
            if (cbGrades.Items.Count > 0)
            {
                cbGrades.SelectedIndex = 0;

                // Load classes for this grade
                int gradeID = Convert.ToInt32(cbGrades.SelectedValue);
                LoadClasses(gradeID);

                // Automatically show timetable for first class if available
                if (dgvClasses.Rows.Count > 0)
                {
                    DataGridViewRow firstRow = dgvClasses.Rows[0];
                    int classID = Convert.ToInt32(firstRow.Cells["ClassID"].Value);
                    string className = firstRow.Cells["ClassName"].Value.ToString();
                    string gradeName = cbGrades.Text;

                    //ShowTimeTable(gradeID, classID, className, gradeName);
                }
            }
        }



        private void LoadGrades()
        {
            cbGrades.Items.Clear();
            DataTable dtGrades = clsGrade.GetAllGrades();
            cbGrades.DataSource = dtGrades;
            cbGrades.DisplayMember = "GradeName";
            cbGrades.ValueMember = "GradeID";
            if (cbGrades.Items.Count > 0)
                cbGrades.SelectedIndex = 0;
        }

        private void LoadClasses(int gradeID)
        {
            dgvClasses.Columns.Clear();
            DataTable dtClasses = clsClass.GetClassesByGradeID(gradeID);
            dgvClasses.DataSource = dtClasses;

            if (dgvClasses.Columns.Contains("ClassID"))
                dgvClasses.Columns["ClassID"].Visible = false;

            // Add a button column for viewing timetable
            DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn
            {
                Text = "View Timetable",
                UseColumnTextForButtonValue = true,
                Name = "btnViewTimetable",
                DefaultCellStyle = { ForeColor = Color.Blue, Font = new Font("Segoe UI", 9, FontStyle.Underline) }
            };
            dgvClasses.Columns.Add(btnCol);
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGrades.SelectedValue != null && int.TryParse(cbGrades.SelectedValue.ToString(), out int gradeID))
            {
                LoadClasses(gradeID);
            }
        }
       

        private void cbClasses_SelectedIndexChanged(object sender, EventArgs e)
        { 

        }
        private void DisplayTimeTable(int gradeID, int classID)
        {
            dgvClasses.Columns.Clear();
            dgvClasses.Rows.Clear();

            // 1. Get raw timetable data
            DataTable dtRaw = clsTimeTableGenerator.GetTimeTable(gradeID, classID);

            if (dtRaw.Rows.Count == 0)
                return;

            // 2. Determine periods dynamically
            var periods = dtRaw.AsEnumerable()
                .Select(r => r.Field<int>("Period"))
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            // 3. Add columns for each period
            dgvClasses.Columns.Add("Day", "Day");
            foreach (var period in periods)
                dgvClasses.Columns.Add($"Period{period}", $"Period {period}");

            // 4. Get all distinct weekdays in order
            var days = Enum.GetValues(typeof(DayOfWeek))
                           .Cast<DayOfWeek>()
                           .Where(d => d != DayOfWeek.Saturday && d != DayOfWeek.Sunday) // Optional: exclude weekends
                           .ToList();

            // 5. Fill rows for each day
            foreach (var day in days)
            {
                DataGridViewRow dr = new DataGridViewRow();
                dr.CreateCells(dgvClasses);
                dr.Cells[0].Value = day.ToString();

                for (int i = 0; i < periods.Count; i++)
                {
                    int period = periods[i];

                    // Get all slots for this day and period
                    var slots = dtRaw.AsEnumerable()
                        .Where(r => r.Field<int>("Period") == period
                                    && r.Field<int>("DayOfWeek") == (int)day)
                        .ToList();

                    if (slots.Count > 0)
                    {
                        string combined = string.Join(Environment.NewLine, slots.Select(r =>
                            r.Field<string>("TeacherName") != null
                                ? $"{r.Field<string>("TeacherName")} - {r.Field<string>("SubjectName")}"
                                : "Break"
                        ));
                        dr.Cells[i + 1].Value = combined;

                        // Gray out breaks
                        if (slots.All(r => r.IsNull("TeacherName")))
                            dr.Cells[i + 1].Style.BackColor = Color.LightGray;
                    }
                    else
                    {
                        dr.Cells[i + 1].Value = "";
                    }
                }

                dgvClasses.Rows.Add(dr);
            }

            // 6. Make cells multiline
            dgvClasses.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvClasses.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void btnGenerate_Click(object sender, EventArgs e)
        {

            try
            {
                // 1. Define custom periods (or skip to use defaults)
                var customPeriods = new List<clsTimeTableGenerator.SchoolPeriod>
        {
            new clsTimeTableGenerator.SchoolPeriod { Period = 1, StartTime = TimeSpan.FromHours(8), EndTime = TimeSpan.FromHours(8.45) },
            new clsTimeTableGenerator.SchoolPeriod { Period = 2, StartTime = TimeSpan.FromHours(8.50), EndTime = TimeSpan.FromHours(9.35) },
            new clsTimeTableGenerator.SchoolPeriod { Period = 3, StartTime = TimeSpan.FromHours(9.40), EndTime = TimeSpan.FromHours(10.25) },
            new clsTimeTableGenerator.SchoolPeriod { Period = 4, StartTime = TimeSpan.FromHours(10.25), EndTime = TimeSpan.FromHours(10.40), IsBreak = true },
            new clsTimeTableGenerator.SchoolPeriod { Period = 5, StartTime = TimeSpan.FromHours(10.40), EndTime = TimeSpan.FromHours(11.25) },
        };

                // 2. Create generator
                clsTimeTableGenerator generator = new clsTimeTableGenerator(customPeriods);

                // 3. Generate weekly timetable
                List<clsTimeTableGenerator.TimeSlot> timetable = generator.GenerateWeeklyTimeTable();

                // 4. Save timetable to database
                bool saved = generator.SaveWeeklyTimeTable(timetable);

                if (saved)
                    MessageBox.Show("Timetable generated and saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Some slots could not be saved. Check logs.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // 5. Display timetable for all classes in the selected grade
                if (cbGrades.SelectedIndex >= 0)
                {
                    int gradeID = Convert.ToInt32(cbGrades.SelectedValue);

                    // Get all classes for this grade
                    DataTable dtClasses = clsClass.GetClassesByGradeID(gradeID);

                    // Clear grid and add a row per class
                    dgvClasses.Columns.Clear();
                    dgvClasses.Rows.Clear();

                    dgvClasses.Columns.Add("ClassName", "Class Name");
                    dgvClasses.Columns.Add("ViewTimetable", "View Timetable");

                    foreach (DataRow row in dtClasses.Rows)
                    {
                        int classID = Convert.ToInt32(row["ClassID"]);
                        string className = row["ClassName"].ToString();

                        int rowIndex = dgvClasses.Rows.Add();
                        dgvClasses.Rows[rowIndex].Cells["ClassName"].Value = className;
                        dgvClasses.Rows[rowIndex].Cells["ViewTimetable"].Value = "View";
                        dgvClasses.Rows[rowIndex].Tag = new { GradeID = gradeID, ClassID = classID }; // store IDs for click
                    }

                    // Style button column
                    dgvClasses.Columns["ViewTimetable"].DefaultCellStyle.ForeColor = Color.Blue;
                    dgvClasses.Columns["ViewTimetable"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Underline);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error generating timetable:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
              
        }

        private void dgvClasses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvClasses.Columns[e.ColumnIndex].Name == "btnViewTimetable")
            {
                int classID = Convert.ToInt32(dgvClasses.Rows[e.RowIndex].Cells["ClassID"].Value);
                string className = dgvClasses.Rows[e.RowIndex].Cells["ClassName"].Value.ToString();
                string gradeName = cbGrades.Text;
                int gradeID = Convert.ToInt32(cbGrades.SelectedValue); // <-- add this

                ShowTimeTable(gradeID, classID, className, gradeName); // <-- pass gradeID
            }
        }

        // Show timetable for selected class
        private void ShowTimeTable(int gradeID, int classID, string className, string gradeName)
        {
            Form timetableForm = new Form
            {
                Text = $"Timetable for {className} - {gradeName}",
                Size = new Size(900, 500),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                BackColor = Color.White
            };

            // --- Title Label ---
            Label lblTitle = new Label
            {
                Text = $"  {className} - {gradeName}",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            timetableForm.Controls.Add(lblTitle);

            // --- Print Button ---
            Button btnPrint = new Button
            {
                Text = "Print Timetable",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };
            timetableForm.Controls.Add(btnPrint);

            // --- Panel to hold DataGridView ---
            Panel pnlGrid = new Panel
            {
                Dock = DockStyle.Fill, // fills remaining space between title and button
                Padding = new Padding(0)
            };
            timetableForm.Controls.Add(pnlGrid);

            // --- DataGridView ---
            // Instead of DockStyle.Fill directly
            DataGridView dgvTimetable = new DataGridView
            {
                Location = new Point(0, lblTitle.Bottom + 5), // lower grid a little below title
                Size = new Size(timetableForm.ClientSize.Width, timetableForm.ClientSize.Height - lblTitle.Height - btnPrint.Height - 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 60 },
                BackgroundColor = Color.WhiteSmoke,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Segoe UI", 10),
                    BackColor = Color.WhiteSmoke
                },
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    BackColor = Color.LightGray
                }
            };
            timetableForm.Controls.Add(dgvTimetable);

            pnlGrid.Controls.Add(dgvTimetable); // add inside panel

            // --- Build timetable table ---
            DataTable table = new DataTable();
            table.Columns.Add("Day");
            for (int i = 1; i <= 5; i++)
                table.Columns.Add($"Period {i}");

            string[] weekDays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            foreach (var day in weekDays)
                table.Rows.Add(day);

            DataTable dtTimeTable = clsTimeTableGenerator.GetTimeTable(gradeID, classID);
            foreach (DataRow row in dtTimeTable.Rows)
            {
                int dayNum = Convert.ToInt32(row["DayOfWeek"]);
                int period = Convert.ToInt32(row["Period"]);
                if (dayNum < 1 || dayNum > 5 || period < 1 || period > 5) continue;

                string subject = row["SubjectName"].ToString();
                string teacher = row["TeacherName"].ToString();
                table.Rows[dayNum - 1][$"Period {period}"] = $"{subject}\n{teacher}";
            }

            dgvTimetable.DataSource = table;

            // --- Print Logic ---
            btnPrint.Click += (s, e) =>
            {
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += (sender, args) =>
                {
                    Graphics g = args.Graphics;
                    int margin = 50;

                    // --- Title for Print ---
                    string title = $"Timetable for {className} - {gradeName}";
                    Font titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
                    SizeF titleSize = g.MeasureString(title, titleFont);
                    g.DrawString(title, titleFont, Brushes.Black,
                        args.PageBounds.Width / 2 - titleSize.Width / 2, margin);

                    int tableX = margin;
                    int tableY = margin + (int)titleSize.Height + 20; // below title
                    int rowHeight = 60;
                    int[] colWidths = { 100, 120, 120, 120, 120, 120 };
                    Font headerFont = new Font("Segoe UI", 10, FontStyle.Bold);
                    Font cellFont = new Font("Segoe UI", 10);

                    // --- Draw header row ---
                    for (int col = 0; col < dgvTimetable.Columns.Count; col++)
                    {
                        Rectangle rect = new Rectangle(tableX + colWidths.Take(col).Sum(), tableY, colWidths[col], rowHeight);
                        g.FillRectangle(Brushes.LightGray, rect);
                        g.DrawRectangle(Pens.Black, rect);
                        string headerText = dgvTimetable.Columns[col].HeaderText;
                        g.DrawString(headerText, headerFont, Brushes.Black, rect, new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                        });
                    }

                    tableY += rowHeight;

                    // --- Draw table cells ---
                    for (int row = 0; row < dgvTimetable.Rows.Count; row++)
                    {
                        for (int col = 0; col < dgvTimetable.Columns.Count; col++)
                        {
                            Rectangle rect = new Rectangle(tableX + colWidths.Take(col).Sum(), tableY, colWidths[col], rowHeight);
                            g.FillRectangle(Brushes.White, rect);
                            g.DrawRectangle(Pens.Black, rect);
                            string text = dgvTimetable.Rows[row].Cells[col].Value?.ToString() ?? "";
                            g.DrawString(text, cellFont, Brushes.Black, rect, new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            });
                        }
                        tableY += rowHeight;
                    }
                };

                PrintPreviewDialog preview = new PrintPreviewDialog
                {
                    Document = pd,
                    Width = 800,
                    Height = 600
                };
                preview.ShowDialog();
            };

            timetableForm.ShowDialog();
        }

    }
}
