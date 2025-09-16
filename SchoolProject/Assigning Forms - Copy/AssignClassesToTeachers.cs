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

namespace SchoolProject.Assigning_Forms
{
    public partial class AssignClassesToTeachers : Form
    {
        public event EventHandler<bool> ClassesAssigned; // true = has assignments

        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private DataTable _dtTeachers;
        private int _TeacherID;
        private DataTable _dtClasses;          // كل المواد
        private DataTable _dtAssignedClasses;  // المواد المعينة للمعلم المختار

        public AssignClassesToTeachers()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }
        public AssignClassesToTeachers(int teacherID)
        {
            InitializeComponent();
            _TeacherID = teacherID;
            Mode = enMode.Update;
        }

        private void AssignClassesToTeachers_Load(object sender, EventArgs e)
        {
            LoadTeachers();
            LoadAllClasses();

            if (Mode == enMode.Update && _TeacherID != 0)
                SelectTeacherByID(_TeacherID);
            else if (_dtTeachers.Rows.Count > 0)
                cbTeachers.SelectedIndex = 0;
        }


        private void LoadTeachers()
        {
            _dtTeachers = clsTeacher.GetAllTeachers();
            cbTeachers.Items.Clear();

            if (_dtTeachers.Rows.Count == 0)
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
        }

        private void LoadAllClasses()
        {
            _dtClasses = clsClass.GetAllClasses(); // Must return ClassID, ClassName
        }

        private void SelectTeacherByID(int teacherID)
        {
            for (int i = 0; i < _dtTeachers.Rows.Count; i++)
            {
                if ((int)_dtTeachers.Rows[i]["TeacherID"] == teacherID)
                {
                    cbTeachers.SelectedIndex = i;
                    break;
                }
            }
        }

        private void cbTeachers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTeachers.SelectedIndex == -1 || _dtTeachers == null || _dtTeachers.Rows.Count == 0)
                return;

            _TeacherID = (int)_dtTeachers.Rows[cbTeachers.SelectedIndex]["TeacherID"];
            LoadAssignedClasses(_TeacherID);
        }
        private void LoadAssignedClasses(int teacherID)
        {
            _dtAssignedClasses = clsTeacherClassAssignment.GetAssignmentsByTeacher(teacherID);
            BindClassesGrid();
        }

        private void BindClassesGrid()
        {
            DataTable dtGrid = new DataTable();
            dtGrid.Columns.Add("ClassID", typeof(int));
            dtGrid.Columns.Add("ClassName", typeof(string));
            dtGrid.Columns.Add("Assigned", typeof(bool));

            foreach (DataRow classRow in _dtClasses.Rows)
            {
                int classID = (int)classRow["ClassID"];
                string className = classRow["ClassName"].ToString();

                bool assigned = false;
                if (_dtAssignedClasses != null)
                {
                    foreach (DataRow assignedRow in _dtAssignedClasses.Rows)
                    {
                        if ((int)assignedRow["ClassID"] == classID)
                        {
                            assigned = true;
                            break;
                        }
                    }
                }

                dtGrid.Rows.Add(classID, className, assigned);
            }

            dgvClasses.DataSource = dtGrid;

            if (dgvClasses.Columns["Assigned"] != null)
            {
                dgvClasses.Columns["Assigned"].ReadOnly = false;
                dgvClasses.Columns["Assigned"].HeaderText = "Assigned";
                dgvClasses.Columns["Assigned"].Width = 80;
            }

            dgvClasses.Columns["ClassID"].Visible = false;
            dgvClasses.Columns["ClassName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvClasses.AllowUserToAddRows = false;
        }
        //public static void AssignClasses(int teacherID, List<int> classIDs)
        //{
        //    // First delete old assignments
        //    clsTeacherClassAssignment.DeleteAssignment(teacherID);

        //    foreach (int classID in classIDs)
        //    {
        //        clsTeacherClassAssignment.(teacherID, classID);
        //    }
        //}
        //private List<int> GetSelectedClassIDs()
        //{
        //    List<int> selectedClassIDs = new List<int>();

        //    if (dgvClasses.DataSource == null)
        //        return selectedClassIDs; // empty list if no data

        //    DataTable dt = (DataTable)dgvClasses.DataSource;
        //    dgvClasses.EndEdit(); // make sure current edits are applied

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        if (row.Field<bool>("Assigned")) // assuming "Assigned" is the checkbox column
        //        {
        //            selectedClassIDs.Add(row.Field<int>("ClassID")); // assuming ClassID column exists
        //        }
        //    }

        //    return selectedClassIDs;
        ////}

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_TeacherID == 0)
            {
                MessageBox.Show("Please select a teacher first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            dgvClasses.EndEdit();
            var dtGrid = (DataTable)dgvClasses.DataSource;
            if (dtGrid == null) return;

            List<int> selectedClassIDs = new List<int>();
            foreach (DataRow row in dtGrid.Rows)
            {
                if (row.Field<bool>("Assigned"))
                {
                    selectedClassIDs.Add(row.Field<int>("ClassID"));
                }
            }

            int createdby = clsGlobal.CurrentUser.UserID;
            bool success = clsTeacherClassAssignment.AssignClasses(_TeacherID, selectedClassIDs, createdby);

            if (success)
            // Fire the event so the parent can check the checkbox
            {
                ClassesAssigned?.Invoke(this, selectedClassIDs.Count > 0);
            MessageBox.Show("Classes assigned successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
          
            else
                MessageBox.Show("Failed to assign classes:\n" , "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

      
    }

}
    
 
