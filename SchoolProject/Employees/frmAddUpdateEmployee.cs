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

namespace SchoolProject.Employees
{
    public partial class frmAddUpdateEmployee : Form
    { // Declare a delegate
        public delegate void DataBackEventHandler(object sender, int PersonID);

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;


        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        private int _EmployeeID;
        clsEmployee _Employee;
        clsPerson _Person;

        private DateTime _Date;
        public frmAddUpdateEmployee()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
            //ctrPersonCardWithFilter1.PersonFound += MyUserControl_ActionTriggered;
        } 
        public frmAddUpdateEmployee(int EmpolyeeID)
        {
            InitializeComponent();
            _EmployeeID = EmpolyeeID;

            Mode = enMode.Update;
        }
        private void MyUserControl_ActionTriggered(object sender, int EmployeeID)
        {
            if (clsEmployee.DoEmployeeExistsForPersonID(ctrPersonCardWithFilter1.PersonID))
            {

                MessageBox.Show("Selected Person is an employee, choose another Person.", "Select another Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            gbEmployee.Enabled = true;
        }
        //private void _FillSubjectsInComoboBox()
        //{
        //    DataTable dtSubjects = clsEmployee

        //    foreach (DataRow row in dtSubjects.Rows)
        //    {
        //        cbPosition.Items.Add(row["JobTitle"]);
        //    }
        //    cbSubjects.SelectedIndex = 0;
        //}

        private void _FillPositionsInComoboBox()
        {
            DataTable dtPositions = clsPosition.GetAllPositions();

            cbPosition.Items.Clear();

            if (dtPositions == null || dtPositions.Rows.Count == 0)
            {
                cbPosition.Items.Add("No positions");
                cbPosition.SelectedIndex = 0;
                return;
            }

            foreach (DataRow row in dtPositions.Rows)
            {
                cbPosition.Items.Add(row["PositionName"].ToString());
            }

            cbPosition.SelectedIndex = 0;
        }



        private void _ResetDefaultValues()
        {
            _FillPositionsInComoboBox();
            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Employee";
                this.lblTitle.Text = "Add New Employee";
                _Employee = new clsEmployee();
                ctrPersonCardWithFilter1.FilterFocus();

                gbEmployee.Enabled = true;
                lblHiredDate.Text = DateTime.Now.ToString();

            }
            else
            {
                lblTitle.Text = "Update Employee";
                this.lblTitle.Text = "Update Employee";
                gbEmployee.Enabled = true;

            }


            lblEmployeeID.Text = "[???]";
            cbPosition.Text = "";
            txtMonthlySalary.Text = "";
            lblHiredDate.Text = "[???]";
            gbEmployee.Enabled = true;
            cbPosition.SelectedIndex = 0;


        }





        private void _LoadData()
        {
            _Employee = clsEmployee.FindByEmployeeID(_EmployeeID); // ✅ صححنا هنا

            if (_Employee == null)
            {
                MessageBox.Show("No Employee with ID: " + _EmployeeID, "Employee Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                gbEmployee.Enabled = false;
                return;
            }

            _Person = clsPerson.Find(_Employee.PersonID); // ✅ لا تنسى تحمل الشخص
            if (_Person == null)
            {
                MessageBox.Show("No Person with ID: " + _Employee.PersonID, "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            ctrPersonCardWithFilter1.FilterEnabled = false;
            ctrPersonCardWithFilter1.LoadPersonInfo(_Person.PersonID);

            lblEmployeeID.Text = _Employee.EmployeeID.ToString();
            cbPosition.Text = _Employee.JobTitle.ToString();
            lblHiredDate.Text = _Employee.HiredDate;
            txtMonthlySalary.Text = _Employee.MonthlySalary.ToString();
            cbEmployeeStatus.Checked = _Employee.EmployeeStatus;

            gbEmployee.Enabled = true;
        }


        private void frmAddUpdateEmployee_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (Mode == enMode.Update)
            {
                _LoadData();

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ////incase of add new mode.
            //if (ctrlPersonCardWithFilter1.PersonID != -1)
            //{


            //}






            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            _Employee.PersonID = ctrPersonCardWithFilter1.PersonID;
            _Employee.JobTitle = cbPosition.Text.ToString();
            _Employee.MonthlySalary = double.Parse(txtMonthlySalary.Text);
            _Employee.EmployeeStatus = cbEmployeeStatus.Checked;

            _Employee.HiredDate = DateTime.Now.ToString();

            // Assign to the employee object
            ////_/*Employee.HiredDate =*/

            lblHiredDate.Text = _Employee.HiredDate;



            if (_Employee.Save())
            {
                lblEmployeeID.Text = _Employee.EmployeeID.ToString();


                //change form mode to update.
                Mode = enMode.Update;
                lblTitle.Text = "Update Employee";
                this.Text = "Update Employee";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Trigger the event to send data back to the caller form.
                DataBack?.Invoke(this, _Employee.EmployeeID);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void txtMonthlySalary_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtMonthlySalary.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtMonthlySalary, "This field is required!");
                return;
            }
            else
            {
                errorProvider1.SetError(txtMonthlySalary, null);
            }

        }

        private void txtMonthlySalary_KeyPress(object sender, KeyPressEventArgs e)
        {
            //this will allow only digits if person id is selected


            // Allow digits, control keys (e.g., Backspace), and a single '.' 
            // Allow digits, backspace, and decimal point
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
            // Ensure only one decimal point is allowed
            if (e.KeyChar == '.' && txtMonthlySalary.Text.Contains("."))
            {
                e.Handled = true;
            }
        }

      
    }
}
