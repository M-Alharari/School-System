using SchoolProject.Properties;
using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Teachers
{
    public partial class ctrlTeacherCard : UserControl
    {
        private clsTeacher _Teacher;
        private clsPerson _Person;
        private int _TeacherID = -1;
        clsEmployee _Employee;
        public int TeacherID => _Teacher?.TeacherID ?? -1;
        public clsTeacher SelectedTeacherInfo => _Teacher;
        public ctrlTeacherCard()
        {
            InitializeComponent();
            llEditTeacherInfo.Enabled = false;
        }



        public void LoadInfo(int teacherID)
        {
            llEditTeacherInfo.Enabled = false;
            _Teacher = clsTeacher.FindByTeacherID(teacherID);
            _Employee = clsEmployee.FindByEmployeeID(_Teacher.EmployeeID);
            if (_Teacher == null)
            {
                _ResetTeacherInfo();
                MessageBox.Show($"No Teacher with ID: {teacherID}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillTeacherInfo();
        }
        private void _FillTeacherInfo()
        {
            if (_Teacher == null)
            {
                _ResetTeacherInfo();
                return;
            }

            llEditTeacherInfo.Enabled = true;

            lblTeacherID.Text = _Teacher.TeacherID.ToString();

            // Get Person through Employee
            if (_Employee.PersonInfo != null)
            {
                lblFullName.Text = _Employee.PersonInfo.FullName;
          
               
            }
            else
            {
                lblFullName.Text = "[No Person Assigned]";
            }

            _LoadAssignedClasses();
            _LoadAssignedSubjects();
        }



        private void _LoadAssignedSubjects()
        {
            // Get subjects assigned to this teacher
            var dtSubjects = clsTeacherSubjectAssignment.GetSubjectsByTeacherID(_Teacher.TeacherID);

            if (dtSubjects == null || dtSubjects.Rows.Count == 0)
            {
                lblSubjetNames.Text = "[No Subjects Assigned]";
                return;
            }

            // Combine subject names into a display string
            var subjectNames = dtSubjects.AsEnumerable()
                                         .Select(r => r.Field<string>("SubjectName"))
                                         .ToList();

            lblSubjetNames.Text = string.Join(", ", subjectNames);
        }


        private void _LoadAssignedClasses()
        {
            // Get all class assignments for this teacher
            var dtAssignments = clsTeacherClassAssignment.GetAssignmentsByTeacher(_Teacher.TeacherID);

            // Group classes by grade
            var gradeGroups = dtAssignments.AsEnumerable()
                .GroupBy(r => r.Field<string>("GradeName"))
                .ToDictionary(g => g.Key, g => g.Select(x => x.Field<string>("ClassName")).ToList());

            // Build display string
            string display = "";
            foreach (var grade in gradeGroups)
            {
                display += grade.Key + " → " + string.Join(", ", grade.Value) + Environment.NewLine;
            }

            lblClasses.Text = string.IsNullOrWhiteSpace(display) ? "[No Classes Assigned]" : display;
        }

        private void _ResetTeacherInfo()
        {
            llEditTeacherInfo.Enabled = false;

            lblTeacherID.Text = "[???]";
            lblFullName.Text = "[???]";
            
         
           
          
             

         
            lblClasses.Text = "[???]";
        }

        private void llEditTeacherInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdateTeacher frmAddUpdateTeacher = new frmAddUpdateTeacher(_Teacher.TeacherID);
            frmAddUpdateTeacher.ShowDialog();
        }
    }
}
