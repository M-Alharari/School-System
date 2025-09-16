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
    public partial class frmDailyAttendance : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private DataTable _dtPeople; // Employees or Students
        private string _personType;  // "Employees" or "Students"
        public frmDailyAttendance(string personType)
        {
            InitializeComponent();
            Mode = enMode.AddNew;

            // store type passed from parent
            _personType = personType;
        }

       
       

        private void frmDailyAttendance_Load(object sender, EventArgs e)
        {
            // Set the form title dynamically
          lblTitle.Text = _personType == "Employees" ? "Employees Daily Attendance" : "Students Daily Attendance";

            LoadPeopleIntoGrid();
            AddCheckboxColumns();
        }


        //private void LoadTeachersIntoGrid()
        //{
        //    _dtTeachers = clsTeacher.GetAllTeachers(); // يجب أن تحتوي على FullName و TeacherID

        //    dgvTeachers.DataSource = _dtTeachers;
        //    dgvTeachers.Columns["TeacherID"].Visible = false;

        //    if (!dgvTeachers.Columns.Contains("IsPresent"))
        //    {
        //        dgvTeachers.Columns.Add(new DataGridViewCheckBoxColumn
        //        {
        //            HeaderText = "Present?",
        //            Name = "IsPresent"
        //        });
        //    }

        //    if (!dgvTeachers.Columns.Contains("Reason"))
        //    {
        //        var reasonCol = new DataGridViewComboBoxColumn()
        //        {
        //            HeaderText = "Reason",
        //            Name = "Reason"
        //        };
        //        reasonCol.Items.AddRange("None", "Sick", "Late", "On Leave", "Excused");
        //        dgvTeachers.Columns.Add(reasonCol);
        //    }

        //    dgvTeachers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        //}
        private void LoadPeopleIntoGrid()
        {
            // Load Employees or Students
            _dtPeople = (_personType == "Employees")
                ? clsEmployee.GetAllEmployees()
                : clsStudent.GetAllStudents();

            // Add placeholder column "Reason" if not exists
            if (!_dtPeople.Columns.Contains("Reason"))
                _dtPeople.Columns.Add("Reason", typeof(string));

            foreach (DataRow row in _dtPeople.Rows)
                row["Reason"] = "None";

            dgvEmployees.DataSource = _dtPeople;

            // Hide ID columns if you want
            if (_personType == "Employees" && _dtPeople.Columns.Contains("EmployeeID"))
                dgvEmployees.Columns["EmployeeID"].Visible = false;
            else if (_personType == "Students" && _dtPeople.Columns.Contains("StudentID"))
                dgvEmployees.Columns["StudentID"].Visible = false;

            dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Add Present? checkbox column ONLY ONCE
            if (dgvEmployees.Columns["IsPresent"] == null)
            {
                DataGridViewCheckBoxColumn chkPresent = new DataGridViewCheckBoxColumn
                {
                    Name = "IsPresent",
                    HeaderText = "Present?"
                };
                dgvEmployees.Columns.Add(chkPresent);
            }

            // Add Reason combo box ONLY ONCE
            if (dgvEmployees.Columns["ReasonColumn"] == null)
            {
                DataGridViewComboBoxColumn cmbReason = new DataGridViewComboBoxColumn
                {
                    Name = "ReasonColumn",
                    HeaderText = "Reason",
                    DataPropertyName = "Reason",
                    DataSource = new string[] { "None", "Sick", "Late", "On Leave", "Excused" }
                };

                // Remove the old "Reason" column from DataGridView if exists
                if (dgvEmployees.Columns.Contains("Reason"))
                    dgvEmployees.Columns.Remove("Reason");

                dgvEmployees.Columns.Add(cmbReason);
            }
        }

        private void AddCheckboxColumns()
        {
            // Add Present? checkbox
            if (!dgvEmployees.Columns.Contains("IsPresent"))
            {
                DataGridViewCheckBoxColumn chkPresent = new DataGridViewCheckBoxColumn
                {
                    Name = "IsPresent",
                    HeaderText = "Present?"
                };
                dgvEmployees.Columns.Add(chkPresent);
            }

            // Add Reason combo box
            if (!dgvEmployees.Columns.Contains("Reason"))
            {
                DataGridViewComboBoxColumn cmbReason = new DataGridViewComboBoxColumn
                {
                    Name = "Reason",
                    HeaderText = "Reason",
                    DataPropertyName = "Reason" // Bind to DataTable column
                };
                cmbReason.Items.AddRange("None", "Sick", "Late", "On Leave", "Excused");
                dgvEmployees.Columns.Add(cmbReason);
            }
        }




        private void AddCheckboxColumn()
        {
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
            chk.HeaderText = "Select";
            chk.Name = "chkSelect";
            dgvEmployees.Columns.Insert(0, chk);
        }

        private void btnMarkAttendance_Click(object sender, EventArgs e)
        {
            bool allSaved = true;
            int currentUserID = 1; // Replace with logged-in user ID

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                if (row.IsNewRow) continue;

                bool isPresent = Convert.ToBoolean(row.Cells["IsPresent"].Value ?? false);
                string reason = row.Cells["ReasonColumn"].Value?.ToString() ?? "None";

                if (_personType == "Employees")
                {
                    int employeeID = Convert.ToInt32(row.Cells["EmployeeID"].Value);

                    // Check FK exists
                    if (!clsEmployee.DoEmployeeExists(employeeID))
                    {
                        MessageBox.Show($"Employee with ID {employeeID} does not exist or is inactive.",
                            "Inactive/Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }


                    // Skip if attendance already exists
                    if (clsEmployeeAttendance.Exists(employeeID, DateTime.Today)) continue;

                    var attendance = new clsEmployeeAttendance
                    {
                        EmployeeID = employeeID,
                        IsPresent = isPresent,
                        AbsenceReason = (!isPresent && reason != "None") ? reason : null,
                        AttendanceDate = DateTime.Today
                    };

                    bool saved = attendance.Save(currentUserID);
                    if (!saved) allSaved = false;
                }
                else
                {
                    int studentID = Convert.ToInt32(row.Cells["StudentID"].Value);
                    if (!clsStudent.DoStudentExists(studentID))
                    {
                        MessageBox.Show("This student is not active.",
                            "Inactive Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }


                   
                    if (clsStudentAttendance.Exists(studentID, DateTime.Today)) continue;

                    var attendance = new clsStudentAttendance
                    {
                        StudentID = studentID,
                        IsPresent = isPresent,
                        AbsenceReason = (!isPresent && reason != "None") ? reason : null,
                        AttendanceDate = DateTime.Today
                    };

                    bool saved = attendance.Save(currentUserID);
                    if (!saved) allSaved = false;
                }
            }

            MessageBox.Show(allSaved
                ? "Attendance recorded successfully."
                : "Some records failed to save.",
                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadPeopleIntoGrid(); // Refresh grid after saving
        }

        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            //bool checkAll = chkSelectAll.Checked;

            foreach (DataGridViewRow row in dgvEmployees.Rows)
            {
                if (row.Cells["IsPresent"] is DataGridViewCheckBoxCell chkCell)
                {
                    chkCell.Value = checkAll;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Pass "Employees" or "Students" depending on which type you want to view
            MonthlyAttendanceView monthlyAttendanceView = new MonthlyAttendanceView(_personType);
            monthlyAttendanceView.ShowDialog();
        }
    }
 }

