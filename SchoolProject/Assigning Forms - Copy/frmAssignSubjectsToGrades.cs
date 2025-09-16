using SchoolProject.People.Controls;
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

namespace SchoolProject.Assigning_Forms.Assign_Subjects_to_Grades
{
    public partial class frmAssignSubjectsToGrades : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private DataTable _dtGrades;
        private int _GradeID;
        private DataTable _dtSubjects; // كل الموضوعات
        private DataTable _dtAssignedSubjects; // الموضوعات المعينة للدرجة المختارة
        public frmAssignSubjectsToGrades()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }

        public frmAssignSubjectsToGrades(int GradeID)
        {
            InitializeComponent();
            _GradeID = GradeID;
            Mode = enMode.Update;
        }
        private void _LoadGrades()
        {
            cbGrades.Items.Clear();
            _dtGrades = clsGrade.GetAllGrades();

            if (_dtGrades.Rows.Count == 0)
            {
                cbGrades.Items.Add("No items");
                cbGrades.SelectedIndex = 0;
                cbGrades.Enabled = false;

                return;
            }



            cbGrades.SelectedIndex = 0; // This will trigger SelectedIndexChanged and load classes for Grade 0
            cbGrades.Enabled = true;

        }




        private void frmAssignSubjectsToGrades_Load(object sender, EventArgs e)
        {
            LoadGrades();
            LoadAllSubjects();

            if (Mode == enMode.Update)
            {
                if (_GradeID != 0)
                {
                    SelectGradeByID(_GradeID);
                }
            }
            else
            {
                if (_dtGrades.Rows.Count > 0)
                {
                    cbGrades.SelectedIndex = 0; // عرض أول درجة تلقائيًا
                }
            }
        }
        private void LoadGrades()
        {
            _dtGrades = clsGrade.GetAllGrades();
            cbGrades.Items.Clear();

            if (_dtGrades.Rows.Count == 0)
            {
                cbGrades.Items.Add("No grades available");
                cbGrades.Enabled = false;
                return;
            }

            foreach (DataRow row in _dtGrades.Rows)
            {
                cbGrades.Items.Add(row["GradeName"].ToString());
            }

            cbGrades.Enabled = true;
        }

        private void LoadAllSubjects()
        {
            _dtSubjects = clsSubject.GetAllSubjects();
            // _dtSubjects = data table of all subjects with columns like SubjectID, SubjectName
        }
        private void SelectGradeByID(int gradeID)
        {
            for (int i = 0; i < _dtGrades.Rows.Count; i++)
            {
                if ((int)_dtGrades.Rows[i]["GradeID"] == gradeID)
                {
                    cbGrades.SelectedIndex = i;
                    break;
                }
            }
        }

        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGrades.SelectedIndex == -1 || _dtGrades == null || _dtGrades.Rows.Count == 0)
                return;

            _GradeID = (int)_dtGrades.Rows[cbGrades.SelectedIndex]["GradeID"];
            LoadAssignedSubjects(_GradeID);
        }
        private void LoadAssignedSubjects(int gradeID)
        {
            // هنا تجلب المواد المعينة للدرجة من قاعدة البيانات (جدول ربط المواد بالدرجات)
            _dtAssignedSubjects = clsGradeSubject.GetSubjectsByGradeID(gradeID);

            // اربط الداتا جريد بالمادة كاملة مع خاصية تحديد (checkbox) لكل مادة مع العلم أن المربعات التي بالموضوعات المختارة تكون مُحددة
            BindSubjectsGrid();
        }
        private void BindSubjectsGrid()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("SubjectID", typeof(int));
            dtGrid.Columns.Add("SubjectName", typeof(string));
            dtGrid.Columns.Add("Assigned", typeof(bool));

            foreach (DataRow subjectRow in _dtSubjects.Rows)
            {
                int subjectID = (int)subjectRow["SubjectID"];
                string subjectName = subjectRow["SubjectName"].ToString();

                bool assigned = false;
                if (_dtAssignedSubjects != null)
                {
                    foreach (DataRow assignedRow in _dtAssignedSubjects.Rows)
                    {
                        if ((int)assignedRow["SubjectID"] == subjectID)
                        {
                            assigned = true;
                            break;
                        }
                    }
                }

                dtGrid.Rows.Add(subjectID, subjectName, assigned);
            }

            dgvSubjects.DataSource = dtGrid;

            // تأكد أن عمود Assigned هو CheckBox
            if (dgvSubjects.Columns["Assigned"] != null)
            {
                dgvSubjects.Columns["Assigned"].ReadOnly = false;
                dgvSubjects.Columns["Assigned"].HeaderText = "Assigned";
                dgvSubjects.Columns["Assigned"].Width = 80;
            }

            // أخفي عمود SubjectID لو تريد
            dgvSubjects.Columns["SubjectID"].Visible = false;

            dgvSubjects.Columns["SubjectName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvSubjects.AllowUserToAddRows = false; // منع إضافة صفوف جديدة
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_GradeID == 0)
            {
                MessageBox.Show("Please select a grade first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dgvSubjects.EndEdit(); // تأكد من انتهاء التحرير

            var dtGrid = (DataTable)dgvSubjects.DataSource;
            if (dtGrid == null)
                return;

            List<int> selectedSubjectIDs = new List<int>();

            foreach (DataRow row in dtGrid.Rows)
            {
                if (row.Field<bool>("Assigned"))
                {
                    selectedSubjectIDs.Add(row.Field<int>("SubjectID"));
                }
            }

            string errorMessage;
            bool success = clsGradeSubject.AssignSubjectsToGrade(_GradeID, selectedSubjectIDs, out errorMessage);

            if (success)
            {
                MessageBox.Show("Subjects assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Failed to assign subjects:\n" + errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvSubjects.DataSource == null) return;

            dgvSubjects.EndEdit(); // تأكد من إغلاق التحرير الحالي

            DataTable dt = (DataTable)dgvSubjects.DataSource;

            bool chkAll = checkAll.Checked;

            foreach (DataRow row in dt.Rows)
            {
                row["Assigned"] = chkAll;
            }

            dt.AcceptChanges();

            dgvSubjects.Refresh();
        }
    }
}