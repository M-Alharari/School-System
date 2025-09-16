using SchoolProject.Assigning_Forms;
using SchoolProject.Guardians;
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

namespace SchoolProject.Teachers
{
    public partial class frmAddUpdateTeacher : Form
    {    // Declare a delegate
        public delegate void DataBackEventHandler(object sender, int PersonID);

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;
        enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;

        clsTeacher _Teacher;
        clsPerson _Person;
        clsEmployee _Employee;
        clsClass _Class;
        clsTeacherClassAssignment _AssignedClass;
        private int _AssignedClassID = -1;
        private int _TeacherID = -1;
        private int _EmployeeID = -1;
        public frmAddUpdateTeacher()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
            ctrlEmployeeCardWithFilter1.EmployeeFound += TeacherFound;
        }

        private void TeacherFound(object sender, int personId)
        {
            gbTeacher.Enabled = false; // نوقف فورم المعلم مؤقتاً

            if (!clsEmployee.DoEmployeeExists(personId))
            {
                MessageBox.Show("This person is not registered as an employee.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (clsTeacher.DoTeacherExists(personId))
            {
                MessageBox.Show("Selected person is already a teacher. Please select another person.", "Duplicate Teacher", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlEmployeeCardWithFilter1.FilterFocus();
                return;
            }

            // كل شيء سليم، شغل الـ groupbox فقط لو في وضع إضافة
            if (Mode == enMode.AddNew)
                gbTeacher.Enabled = true;
            btnSave.Enabled = true;
        }




        public frmAddUpdateTeacher(int TeacherID)
        {
            InitializeComponent();
            _TeacherID = TeacherID;
       
             
            Mode = enMode.Update;
        }



        //private void _FillSubjectsInComoboBox()
        //{
        //    DataTable dtSubjects = clsSubject.GetAllSubjects();

        //    if (dtSubjects == null || dtSubjects.Rows.Count == 0)
        //    {
        //        // Option 1: Just clear combo and exit
        //        cbSubjects.Items.Clear();
        //        cbSubjects.Items.Add("No subjects available");
        //        cbSubjects.SelectedIndex = 0;
        //        return;

        //        // Option 2: throw a message to user
        //        // MessageBox.Show("No subjects found in database.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        // return;
        //    }

        //    cbSubjects.Items.Clear(); // avoid duplicates
        //    foreach (DataRow row in dtSubjects.Rows)
        //    {
        //        cbSubjects.Items.Add(row["SubjectName"]);
        //    }

        //    cbSubjects.SelectedIndex = 0;
        //}

        //private void _FillClassesInComoboBox()
        //{
        //    DataTable dtClasses = clsClass.GetAllClasses();

        //    foreach (DataRow row in dtClasses.Rows)
        //    {
        //        cbClasses.Items.Add(row["ClassName"]);
        //    }
        //    cbClasses.SelectedIndex = 0;
        //}


        private void _ResetDefaultValues()
        {
            //_FillSubjectsInComoboBox();
            //_FillClassesInComoboBox();

            if (Mode == enMode.AddNew)
            {
                _Teacher = new clsTeacher();
                lblTitle.Text = "Add New Teacher";
                gbTeacher.Enabled = false;  // يبقى معطل
                ctrlEmployeeCardWithFilter1.FilterFocus();
                lblTeacherID.Text = "[????]";
                btnSave.Enabled = false;
            }
            else
            {
                lblTitle.Text = "Update Teacher";
                gbTeacher.Enabled = true;  // يفعل دائماً في تحديث
                btnSave.Enabled = true;
            }
        }


        private void _LoadDate()
        {
            _Teacher = clsTeacher.FindByTeacherID(_TeacherID);

            if (_Teacher == null)
            {
                MessageBox.Show("No Teacher with ID " + _TeacherID, "Teacher Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                gbTeacher.Enabled = false;
                this.Close();
                return;
            }

            _EmployeeID = _Teacher.EmployeeID;
            _Employee = clsEmployee.FindByEmployeeID(_EmployeeID);

            ctrlEmployeeCardWithFilter1.LoadEmployeeData(_EmployeeID);
            ctrlEmployeeCardWithFilter1.FilterEnabled = false;
            gbTeacher.Enabled = true;
            btnSave.Enabled = true;
            lblTeacherID.Text = _Teacher.TeacherID.ToString();

            // Refresh checkboxes
            _RefreshAssignedClassesCheckbox();   // already exists
            _RefreshAssignedSubjectsCheckbox();  // << ADD THIS LINE <<
        }






        private void _LoadD4ate()
        {

            _Teacher = clsTeacher.FindByTeacherID(_TeacherID);


            ctrlEmployeeCardWithFilter1.FilterEnabled = false;
            if (_Teacher == null)
            {
                MessageBox.Show("No Teacher with ID" + _Teacher, "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                gbTeacher.Enabled = false;
                return;
            }
            gbTeacher.Enabled = true;
            btnSave.Enabled = true;



            ctrlEmployeeCardWithFilter1.LoadEmployeeData(_EmployeeID);
            lblTeacherID.Text = _TeacherID.ToString();
            lblTeacherID.Text = _Teacher.TeacherID.ToString();

            //cbClasses.SelectedIndex = cbClasses.FindString(_Teacher.ClassInfo.ClassName);
            //cbSubjects.SelectedIndex = cbSubjects.FindString(_Teacher.SubjectInfo.SubjectName);

            //cbSubjects.SelectedIndex = 
            // assign class and subject
            





        }

        private void btnSave_Click(object sender, EventArgs e)
        {


            try
            {
                if (!this.ValidateChildren())
                {
                    MessageBox.Show("Some fields are not valid! Put the mouse over the red icon(s) to see the error.",
                        "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (_Teacher == null)
                {
                    MessageBox.Show("Teacher object is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //var subject = clsSubject.FindSubjectByName(cbSubjects.Text);
                //if (subject == null)
                //{
                //    MessageBox.Show("Selected subject is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                //var @class = clsClass.FindClassByName(cbClasses.Text);
                //if (@class == null)
                //{
                //    MessageBox.Show("Selected class is invalid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                if (ctrlEmployeeCardWithFilter1 == null)
                {
                    MessageBox.Show("Employee control is not initialized.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _Teacher.EmployeeID = ctrlEmployeeCardWithFilter1.EmployeeID;
                //_Teacher.SubjectID = subject.SubjectID;
                //_Teacher.ClassID = @class.ClassID;

                if (_Teacher.Save())
                {
                    lblTeacherID.Text = _Teacher.TeacherID.ToString();

                    Mode = enMode.Update;
                    lblTitle.Text = "Update Employee";
                    this.Text = "Update Employee";

                    MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DataBack?.Invoke(this, _Teacher.EmployeeID);
                }
                else
                {
                    MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show("Null reference detected: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void frmAddUpdateTeacher_Load(object sender, EventArgs e)
        {

            _ResetDefaultValues();

            // Enable group box if the employee control already has a value
            if (ctrlEmployeeCardWithFilter1 != null && ctrlEmployeeCardWithFilter1.EmployeeID > 0)
            {
                gbTeacher.Enabled = true;
                btnSave.Enabled = true;
            }

            if (Mode == enMode.Update)
            {
                _LoadDate();
            }
        }

        private void llAssigntoClasses_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Teacher.TeacherID == -1)
            {
                MessageBox.Show("Save the teacher first before assigning classes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AssignClassesToTeachers frm = new AssignClassesToTeachers(_TeacherID);

            // Subscribe to the event
            frm.ClassesAssigned += FrmAssignClassesToTeacher_ClassesAssigned;

            frm.ShowDialog();

        }

       

         private void FrmAssignClassesToTeacher_ClassesAssigned(object sender, bool hasAssignments)
        {
            // Check the checkbox if the teacher has assigned classes
            chkHasClasses.Checked = hasAssignments;
        }

        private void _RefreshAssignedClassesCheckbox()
        {
            if (_Teacher == null || _Teacher.TeacherID <= 0)
                return;

            // Get assigned classes as DataTable
            DataTable dtAssignments = clsTeacherClassAssignment.GetAssignmentsByTeacher(_Teacher.TeacherID);

            // Check if there are any rows — true if teacher has assignments
            bool hasAssignments = dtAssignments != null && dtAssignments.Rows.Count > 0;

            chkHasClasses.Checked = hasAssignments;
        }





        private void llAssigntoSubjects_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Teacher.TeacherID == -1)
            {
                MessageBox.Show("Save the teacher first before assigning classes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AssignTeachersToSubjecs frm = new AssignTeachersToSubjecs(_TeacherID);

            // Subscribe to the event
            frm.ClassesAssigned += FrmAssignSubjectsToTeacher_SubjectsAssigned;

            frm.ShowDialog();
            
        }

        private void FrmAssignSubjectsToTeacher_SubjectsAssigned(object sender, bool hasAssignments)
        {
            // Check the checkbox if the teacher has assigned classes
            chkHasSubjects.Checked = hasAssignments;
        }

        private void _RefreshAssignedSubjectsCheckbox()
        {
            if (_Teacher == null || _Teacher.TeacherID <= 0)
                return;

            // Get assigned classes as DataTable
            DataTable dtAssignments = clsTeacherSubjectAssignment.GetSubjectsByTeacherID(_Teacher.TeacherID);

            // Check if there are any rows — true if teacher has assignments
            bool hasAssignments = dtAssignments != null && dtAssignments.Rows.Count > 0;

            chkHasSubjects.Checked = hasAssignments;
        }

    }
}
