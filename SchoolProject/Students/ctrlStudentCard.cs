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

namespace SchoolProject.Students
{
    public partial class ctrlStudentCard : UserControl
    {
        private clsStudent _Student;
        private clsPerson _Person;
        private clsEnrollment _Enrollment;
        private int _StudentID = -1;
        private int _PersonID = -1;
        public int StudentID
        {
            get { return _Student.StudentID; }
        }

        public clsStudent SelectedStudentInfo
        {
            get { return _Student; }
        }
        public ctrlStudentCard()
        {
            InitializeComponent();
            llEditStudentInfo.Enabled = false;
        }

        public void LoadInfo(int StudentID)
        {
            llEditStudentInfo.Enabled = false;
            _Student = clsStudent.FindStudentByID(StudentID);
            if (_Student == null)
            {
                _ResetPersonInfo();
                MessageBox.Show($"No Student with Student ID: {StudentID}", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

                return;
            }
            _FillUserInfo();
        }

        public void LoadInfoByPersonID(int PersonID)
        {
            _Student = clsStudent.FindStudentByID(PersonID);
            if (_Student == null)
            {
                _ResetPersonInfo();
                MessageBox.Show($"No Student with ID: {_Student}", "Error", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }
            _FillUserInfo();
        }

        private void _LoadPersonImage()
        {

            if (_Student.PersonInfo.Gender == 0)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            string ImagePath = _Student.PersonInfo.ImagePath;
            if (ImagePath != "")
                if (File.Exists(ImagePath))
                    pbPersonImage.ImageLocation = ImagePath;
                else
                    MessageBox.Show("Could not find this image: " + ImagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);



        }



        private void _FillUserInfo()
        {
            _Student = clsStudent.FindStudentByID(_Student.StudentID);
            if (_Student == null)
            {
                _ResetPersonInfo();
                MessageBox.Show("Student is null.", "Error");
                return;
            }
            _Person = clsPerson.Find(_Student.PersonID);
            llEditStudentInfo.Enabled = true;

            
            lblStudentID.Text = _Student.StudentID.ToString();
            lblFullName.Text = _Person.FullName;

            lblGender.Text = _Person.Gender == 0 ? "Male" : "Female";
            //lblEmail.Text = _Enrollment.PersonInfo.Email;
            
            int age = DateTime.Today.Year - _Person.DateOfBirth.Year;

            if (_Person.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
            lblDateOfBirth.Text = $"{age} Years old";
            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID).CountryName;
             

            _Enrollment = clsEnrollment.FindByStudentID(_Student.StudentID);
            lblClass.Text = _Enrollment.ClassInfo.ClassName;
            lblGradeName.Text = _Enrollment.GradeInfo.GradeName;
            lblStatus.Text = _Enrollment.IsActive ? "Active" : "Not Active";
            lblRegistredDate.Text = _Enrollment.EnrollmentDate.ToString();

            _LoadPersonImage();


        }

        private void _ResetPersonInfo()
        {

            llEditStudentInfo.Enabled = false;

          
            lblStudentID.Text = "[???]";
            lblFullName.Text = "[???]";
            lblGender.Text = "[???]";
            //lblEmail.Text = "[???]";
          
            lblDateOfBirth.Text = "[???]";
            lblCountry.Text = "[???]";
          
            
            lblGradeName.Text = "[???]";
            lblStatus.Text = "[???]";
            lblRegistredDate.Text = "[???]";

            pbPersonImage.Image = Resources.Male_512;
            pbGender.Image = Resources.Man_32;

        }

        private void llEditStudentInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdateStudent frm = new frmAddUpdateStudent(_Student.StudentID);
            frm.ShowDialog();
        }
    }
}
