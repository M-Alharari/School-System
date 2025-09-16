using SchoolProject.Global_Classes;
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

namespace SchoolProject.Assigning_Forms
{
    public partial class AssignTeachersToSubjecs : Form
    {
        public event EventHandler<bool> ClassesAssigned; // true = has assignments
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private DataTable _dtTeachers;
        private DataTable _dtSubjects;
        private DataTable _dtAssignedSubjects;

        private int _TeacherID;

        public AssignTeachersToSubjecs(int TeacherID)
        {
            InitializeComponent();
            _TeacherID = TeacherID;
            Mode = enMode.AddNew;
        }

        private void AssignTeachersToSubjecs_Load(object sender, EventArgs e)
        {
            LoadAllSubjects();
            LoadTeachers();

            if (_TeacherID > 0)
            {
                // Update mode: select the teacher in combo box
                int index = _dtTeachers.Rows
                             .Cast<DataRow>()
                             .ToList()
                             .FindIndex(r => (int)r["TeacherID"] == _TeacherID);

                if (index >= 0)
                {
                    cbTeachers.SelectedIndex = index;

                    // Manually load subjects for this teacher
                    LoadAssignedSubjects(_TeacherID);
                }
            }
            else
            {
                // Add mode: select first item if available
                if (cbTeachers.Items.Count > 0)
                    cbTeachers.SelectedIndex = 0;
            }
        }


        private void LoadTeachers()
        {
            _dtTeachers = clsTeacher.GetAllTeachers();
            cbTeachers.Items.Clear();

            if (_dtTeachers == null || _dtTeachers.Rows.Count == 0)
            {
                cbTeachers.Items.Add("No teachers available");
                cbTeachers.Enabled = false;
                return;
            }

            foreach (DataRow row in _dtTeachers.Rows)
            {
                cbTeachers.Items.Add(row["FullName"].ToString());
            }

            cbTeachers.Enabled = true;

            // ✅ If we have a teacher ID (update mode), select it and load subjects
            if (_TeacherID > 0)
            {
                int index = _dtTeachers.AsEnumerable()
                                       .ToList()
                                       .FindIndex(r => r.Field<int>("TeacherID") == _TeacherID);

                if (index >= 0)
                {
                    // Temporarily remove event to prevent double-loading
                    cbTeachers.SelectedIndexChanged -= cbTeachers_SelectedIndexChanged;

                    cbTeachers.SelectedIndex = index;

                    cbTeachers.SelectedIndexChanged += cbTeachers_SelectedIndexChanged;

                    // Immediately load assigned subjects for this teacher
                    LoadAssignedSubjects(_TeacherID);
                }
            }
            else
            {
                // Default to first teacher for AddNew mode
                cbTeachers.SelectedIndex = 0; // this will trigger SelectedIndexChanged
            }
        }



        private void LoadAllSubjects()
        {
            _dtSubjects = clsSubject.GetAllSubjects();
            if (_dtSubjects == null)
                _dtSubjects = new DataTable();
            lblRecordCount.Text = _dtSubjects.Rows.Count.ToString();
        }

        private void cbTeachers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTeachers.SelectedIndex == -1 || _dtTeachers == null || _dtTeachers.Rows.Count == 0)
                return;

            _TeacherID = _dtTeachers.Rows[cbTeachers.SelectedIndex].Field<int>("TeacherID");
            LoadAssignedSubjects(_TeacherID);
        }
        private void LoadAssignedSubjects(int teacherID)
        {
            _dtAssignedSubjects = clsTeacherSubjectAssignment.GetSubjectsByTeacherID(teacherID);
            if (_dtAssignedSubjects == null)
                _dtAssignedSubjects = new DataTable();

            BindSubjectsGrid();
        }

        private void BindSubjectsGrid()
        {
            if (_dtSubjects == null)
                _dtSubjects = new DataTable();
            if (_dtAssignedSubjects == null)
                _dtAssignedSubjects = new DataTable();

            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("SubjectID", typeof(int));
            dtGrid.Columns.Add("SubjectName", typeof(string));
            dtGrid.Columns.Add("Assigned", typeof(bool));

            foreach (DataRow subjectRow in _dtSubjects.Rows)
            {
                int subjectID = (int)subjectRow["SubjectID"];
                string subjectName = subjectRow["SubjectName"].ToString();

                bool assigned = _dtAssignedSubjects
                    .AsEnumerable()
                    .Any(r => r.Field<int>("SubjectID") == subjectID);

                dtGrid.Rows.Add(subjectID, subjectName, assigned);
            }

            dgvSubjects.DataSource = dtGrid;
            dgvSubjects.Columns["Assigned"].ReadOnly = false;
            dgvSubjects.Columns["Assigned"].HeaderText = "Assigned";
            dgvSubjects.Columns["SubjectID"].Visible = false;
            dgvSubjects.Columns["SubjectName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvSubjects.AllowUserToAddRows = false;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_TeacherID == -1)
            {
                MessageBox.Show("Please select a teacher first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dgvSubjects.EndEdit();
            var dtGrid = (DataTable)dgvSubjects.DataSource;
            if (dtGrid == null) return;

            List<int> selectedSubjectIDs = new List<int>();

            foreach (DataRow row in dtGrid.Rows)
            {
                if (row.Field<bool>("Assigned"))
                {
                    selectedSubjectIDs.Add(row.Field<int>("SubjectID"));
                }
            }

            int CreatedByUserID = clsGlobal.CurrentUser.UserID;
            bool success = clsTeacherSubjectAssignment.AssignSubjectsToTeacher(_TeacherID, selectedSubjectIDs, CreatedByUserID);

            if (success)
            {
                ClassesAssigned?.Invoke(this, selectedSubjectIDs.Count > 0);
                MessageBox.Show("Subjects assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
               
            else
                MessageBox.Show("Failed to assign subjects:\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvSubjects.DataSource == null) return;

            dgvSubjects.EndEdit();
            DataTable dt = (DataTable)dgvSubjects.DataSource;
            
            dgvSubjects.Refresh();
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            frmAddUpdateTeacher frmAddUpdateTeacher = new frmAddUpdateTeacher();
            frmAddUpdateTeacher.ShowDialog();
        }
    }
}
     
