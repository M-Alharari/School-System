using SchoolProject.Global_Classes;
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

namespace SchoolProject.Attendance
{
    public partial class tryout : Form
    {
        private DataTable _dtTeachersAttendance;

        public tryout()
        {
            InitializeComponent();
        }

        private void tryout_Load(object sender, EventArgs e)
        {
            LoadMonths();
            string currentMonth = DateTime.Now.ToString("yyyy-MM");
            LoadAttendanceByMonth(currentMonth);

            AddCheckboxColumns();
        }

        private void AddCheckboxColumns()
        {
            // تأكد من عدم تكرار العمود
            if (!dgvAttendance.Columns.Contains("IsPresent"))
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.Name = "IsPresent";
                chk.HeaderText = "Present?";
                chk.DataPropertyName = "IsPresent"; // للربط مع الداتا
                dgvAttendance.Columns.Insert(0, chk);
            }

            if (!dgvAttendance.Columns.Contains("Reason"))
            {
                DataGridViewComboBoxColumn reasonColumn = new DataGridViewComboBoxColumn();
                reasonColumn.Name = "Reason";
                reasonColumn.HeaderText = "Absence Reason";
                reasonColumn.DataPropertyName = "AbsenceReason";

                // قيم الغياب
                reasonColumn.Items.AddRange("Sick", "Vacation", "Late", "Permission", "None");
                dgvAttendance.Columns.Insert(1, reasonColumn);
            }
        }

        // تحميل بيانات الحضور من قاعدة البيانات وعرضها
        private void LoadAttendanceGrid()
        {
            _dtTeachersAttendance = clsEmployeeAttendance.GetAllAttendance();
            dgvAttendance.DataSource = _dtTeachersAttendance;

            // إخفاء أعمدة غير مهمة للعرض
            if (dgvAttendance.Columns.Contains("AttendanceID"))
                dgvAttendance.Columns["AttendanceID"].Visible = false;
            if (dgvAttendance.Columns.Contains("EmpoyeeID"))
                dgvAttendance.Columns["EmpoyeeID"].Visible = false;

            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        // مثلاً ComboBox خاص بالشهور بالشكل "2025-07", "2025-08", ...
        private void LoadMonths()
        {
            // يمكن جلب الشهور من قاعدة البيانات أو توليدها برمجياً
            // هنا توليد 12 شهر قبل وبعد التاريخ الحالي كمثال
            List<string> months = new List<string>();
            DateTime now = DateTime.Now;
            for (int i = -6; i <= 6; i++)
            {
                DateTime m = now.AddMonths(i);
                months.Add(m.ToString("yyyy-MM"));
            }

            comboBoxMonths.DataSource = months;
            comboBoxMonths.SelectedItem = now.ToString("yyyy-MM");
        }
        // إضافة أعمدة للتحكم في تسجيل الحضور (مربع اختيار وسبب الغياب)
        private void AddAttendanceColumns()
        {
            if (!dgvAttendance.Columns.Contains("IsPresentCheck"))
            {
                var chk = new DataGridViewCheckBoxColumn
                {
                    Name = "IsPresentCheck",
                    HeaderText = "Present?",
                    Width = 60
                };
                dgvAttendance.Columns.Add(chk);
            }

            if (!dgvAttendance.Columns.Contains("AbsenceReasonEdit"))
            {
                var cmb = new DataGridViewComboBoxColumn
                {
                    Name = "AbsenceReasonEdit",
                    HeaderText = "Absence Reason",
                    Width = 120
                };
                cmb.Items.AddRange("None", "Sick", "Late", "On Leave", "Excused");
                dgvAttendance.Columns.Add(cmb);
            }

            // تهيئة قيم الأعمدة الجديدة بناء على البيانات الحالية
            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.IsNewRow) continue;

                bool isPresent = row.Cells["IsPresent"].Value != DBNull.Value && Convert.ToBoolean(row.Cells["IsPresent"].Value);
                row.Cells["IsPresentCheck"].Value = isPresent;

                string reason = row.Cells["AbsenceReason"].Value?.ToString();
                row.Cells["AbsenceReasonEdit"].Value = string.IsNullOrEmpty(reason) ? "None" : reason;
            }
        }

        private void btnMarkAttendance_Click(object sender, EventArgs e)
        {
            bool allSaved = true;

            foreach (DataGridViewRow row in dgvAttendance.Rows)
            {
                if (row.IsNewRow) continue;

                int attendanceID = row.Cells["AttendanceID"].Value != DBNull.Value ? Convert.ToInt32(row.Cells["AttendanceID"].Value) : -1;
                int teacherID = Convert.ToInt32(row.Cells["EmpoyeeID"].Value);
                bool isPresent = Convert.ToBoolean(row.Cells["IsPresentCheck"].Value ?? false);
                string absenceReason = row.Cells["AbsenceReasonEdit"].Value?.ToString();

                if (absenceReason == "None") absenceReason = null;

                DateTime attendanceDate = DateTime.Now.Date; // تحفظ على تاريخ اليوم

                clsEmployeeAttendance attendance;
                if (attendanceID == -1)
                {
                    // سجل جديد
                    attendance = new clsEmployeeAttendance()
                    {
                        EmployeeID = teacherID,
                         IsPresent= isPresent,
                        AbsenceReason = absenceReason,
                        AttendanceDate = attendanceDate,
                        Mode = clsEmployeeAttendance.enMode.AddNew
                    };
                }
                else
                {
                    // سجل موجود
                    attendance = clsEmployeeAttendance.FindByID(attendanceID);
                    if (attendance == null)
                    {
                        allSaved = false;
                        continue;
                    }
                    attendance.IsPresent = isPresent;
                    attendance.AbsenceReason = absenceReason;
                    attendance.AttendanceDate = attendanceDate;
                    attendance.Mode = clsEmployeeAttendance.enMode.Update;
                }
                int currentUser = clsGlobal.CurrentUser.UserID;
                bool saved = attendance.Save(currentUser);
                if (!saved)
                {
                    allSaved = false;
                    // يمكن هنا تسجيل الخطأ أو إخطار المستخدم
                }
                else
                {
                    // تحديث جدول البيانات برقم الـ AttendanceID الجديد إن تم الإضافة
                    if (attendanceID == -1)
                    {
                        row.Cells["AttendanceID"].Value = attendance.AttendanceID;
                    }
                }
            }

            if (allSaved)
                MessageBox.Show("Attendance saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Some records failed to save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // إعادة تحميل البيانات لتحديث الأعمدة المحسوبة مثل DayOfMonth, MonthName, YearMonth
            LoadAttendanceGrid();
        }

        private void comboBoxMonths_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMonth = comboBoxMonths.SelectedItem.ToString();
            LoadAttendanceByMonth(selectedMonth);
        }

        private void LoadAttendanceByMonth(string yearMonth)
        {
            DataTable dt = clsEmployeeAttendance.GetAttendanceByMonth(yearMonth);
            dgvAttendance.DataSource = dt;

            // تأكد من إخفاء أعمدة غير ضرورية وإظهار الأعمدة المهمة
            dgvAttendance.Columns["EmployeeID"].Visible = false;
            dgvAttendance.Columns["AttendanceID"].Visible = false;
            dgvAttendance.Columns["YearMonth"].Visible = false;

            // عرض يوم الأسبوع
            dgvAttendance.Columns["WeekDayName"].HeaderText = "Day of Week";

            dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
    }
    }

