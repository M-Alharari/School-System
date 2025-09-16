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

namespace SchoolProject.Employees
{
    public partial class ctrlEmployeeCard : UserControl
    {
        private clsEmployee _Employee;
        private clsPerson _Person;
        private int _EmployeeID = -1;
        private int _PersonID = -1;
        public int EmployeeID
        {
            get { return _Employee?.EmployeeID ?? -1; }
        }

        public clsEmployee SelectedEmployeeInto
        {
            get { return _Employee; }
        }
        public ctrlEmployeeCard()
        {
            InitializeComponent();
            llEditEmployeeInfo.Enabled = false;
        }

        public void LoadInfo(int employeeID)
        {
            llEditEmployeeInfo.Enabled = false;
            _Employee = clsEmployee.FindByEmployeeID(employeeID);

            if (_Employee == null)
            {
                _ResetPersonInfo();
                MessageBox.Show($"No employee found with EmployeeID: {employeeID}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Person = clsPerson.Find(_Employee.PersonID);
            _FillUserInfo();
        }

        public void LoadInfoByPersonID(int personID)
        {
            llEditEmployeeInfo.Enabled = false;
            _Employee = clsEmployee.FindByPersonID(personID); // ✅ FIXED: use personID here

            if (_Employee == null)
            {
                _ResetPersonInfo();
                MessageBox.Show($"No employee found with PersonID: {personID}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Person = clsPerson.Find(personID);
            _FillUserInfo();
        }

        private void _FillUserInfo()
        {
            if (_Employee == null || _Person == null)
            {
                _ResetPersonInfo();
                MessageBox.Show("Employee or Person record is missing.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            llEditEmployeeInfo.Enabled = true;
            lblPersonID.Text = _Person.PersonID.ToString();
            lblEmployeeID.Text = _Employee.EmployeeID.ToString();
            lblFullName.Text = _Person.FullName;
            lblGender.Text = _Person.Gender == 0 ? "Male" : "Female";
            lblEmail.Text = _Person.Email;
            lblPhone.Text = _Person.Phone;

            int age = DateTime.Today.Year - _Person.DateOfBirth.Year;
            if (_Person.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
            lblDateOfBirth.Text = $"{age} Years old";

            lblCountry.Text = clsCountry.Find(_Person.NationalityCountryID)?.CountryName ?? "Unknown";
            lblAddress.Text = _Person.Address;

            double monthlySalary = _Employee.MonthlySalary;
            double yearlySalary = monthlySalary * 12;
            lblYearlySalary.Text = yearlySalary.ToString("N2");

            lblPosition.Text = _Employee.JobTitle;
            lblIsActive.Text = _Employee.EmployeeStatus ? "Active" : "Not Active";
            lblHiredDate.Text = _Employee.HiredDate.ToString();

            _LoadPersonImage();
        }

        private void _LoadPersonImage()
        {
            // Default image based on gender
            pbPersonImage.Image = _Person.Gender == 0 ? Resources.Male_512 : Resources.Female_512;

            if (!string.IsNullOrWhiteSpace(_Person.ImagePath) && File.Exists(_Person.ImagePath))
            {
                pbPersonImage.ImageLocation = _Person.ImagePath;
            }
            else if (!string.IsNullOrWhiteSpace(_Person.ImagePath))
            {
                MessageBox.Show("Could not find this image: " + _Person.ImagePath,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //private void _LoadPersonImage()
        //{

        //    if (_Employee.PersonInfo.Gender == 0)
        //        pbPersonImage.Image = Resources.Male_512;
        //    else
        //        pbPersonImage.Image = Resources.Female_512;

        //    string ImagePath = _Employee.PersonInfo.ImagePath;
        //    if (ImagePath != "")
        //        if (File.Exists(ImagePath))
        //            pbPersonImage.ImageLocation = ImagePath;
        //        else
        //            MessageBox.Show("Could not find this image: " + ImagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);



        //}



        //private void _FillUserInfo()
        //{

        //    if (_Employee == null)
        //    {
        //        _ResetPersonInfo();
        //        MessageBox.Show("Employee or PersonInfo is null.", "Error");
        //        return;
        //    }
        //    _Person = clsPerson.Find(_Employee.PersonID);
        //    llEditEmployeeInfo.Enabled = true;
        //    lblPersonID.Text = _Employee.PersonID.ToString();
        //    lblEmployeeID.Text = _Employee.EmployeeID.ToString();
        //    lblFullName.Text = _Person.FullName;
        //    lblGender.Text = _Employee.PersonInfo.Gender == 0 ? "Male" : "Female";
        //    lblEmail.Text = _Employee.PersonInfo.Email;
        //    lblPhone.Text = _Employee.PersonInfo.Phone;
        //    int age = DateTime.Today.Year - _Employee.PersonInfo.DateOfBirth.Year;
        //    if (_Employee.PersonInfo.DateOfBirth > DateTime.Today.AddYears(-age)) age--;
        //    lblDateOfBirth.Text = $"{age} Years old";
        //    lblCountry.Text = clsCountry.Find(_Employee.PersonInfo.NationalityCountryID).CountryName;
        //    lblAddress.Text = _Employee.PersonInfo.Address;

        //    double monthlySalary = double.Parse(_Employee.MonthlySalary.ToString());
        //    double yearlySalary = monthlySalary * 12;
        //    lblYearlySalary.Text = yearlySalary.ToString();

        //    lblPosition.Text = _Employee.JobTitle;
        //    lblIsActive.Text = _Employee.EmployeeStatus ? "Active" : "Not Active";
        //    lblHiredDate.Text = _Employee.HiredDate.ToString();

        //    _LoadPersonImage();


        //}

        private void _ResetPersonInfo()
        {

            llEditEmployeeInfo.Enabled = false;

            lblPersonID.Text = "[???]";
            lblEmployeeID.Text = "[???]";
            lblFullName.Text = "[???]";
            lblGender.Text = "[???]";
            lblEmail.Text = "[???]";
            lblPhone.Text = "[???]";
            lblDateOfBirth.Text = "[???]";
            lblCountry.Text = "[???]";
            lblAddress.Text = "[???]";
            lblYearlySalary.Text = "[???]";
            lblPosition.Text = "[???]";
            lblIsActive.Text = "[???]";
            lblHiredDate.Text = "[???]";

            pbPersonImage.Image = Resources.Male_512;
            pbGender.Image = Resources.Man_32;

        }

        private void llEditEmployeeInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdateEmployee frm = new frmAddUpdateEmployee(_Employee.EmployeeID);
            frm.ShowDialog();
            LoadInfo(_Employee.EmployeeID);
        }
    }
}
