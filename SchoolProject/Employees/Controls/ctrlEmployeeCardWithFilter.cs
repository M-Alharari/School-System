using SchoolProject.People;
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
    public partial class ctrlEmployeeCardWithFilter : UserControl
    { // define a custom event handler delegate with parameters 
        public event Action<int> OnEmployeeSelected;
        // create a protected method to raise the event with a prarameters
        protected virtual void EmployeeSelected(int EmployeeID)
        {
            Action<int> handler = OnEmployeeSelected;
            if (handler != null)
            {
                handler(EmployeeID);
            }

        }
        public event EventHandler<int> EmployeeFound;

        private bool _ShowAddEmployee = true;

        public bool ShowAddEmploye
        {
            get { return _ShowAddEmployee; }
            set { _ShowAddEmployee = value; btnAddNewEmployee.Visible = _ShowAddEmployee; }
        }

        private bool _FilterEnabled = true;
        public bool FilterEnabled
        {
            get { return _FilterEnabled; }
            set { _FilterEnabled = value; gbFilters.Enabled = _FilterEnabled; }

        }
        public ctrlEmployeeCardWithFilter()
        {
            InitializeComponent();
        }
        private int _EmployeeID = -1;
        public int EmployeeID
        {
            get { return ctrlEmployeeCard1.EmployeeID; }
        }

        public clsEmployee SelectedEmployee
        {
            get { return ctrlEmployeeCard1.SelectedEmployeeInto; }

        }

        public void LoadEmployeeData(int EmployeeID)
        {
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Text = EmployeeID.ToString();
            FindNow();
        }

        private void FindNow()
        {
            switch (cbFilterBy.Text)
            {
                case "Employee ID":
                    ctrlEmployeeCard1.LoadInfo(int.Parse(txtFilterValue.Text));
                    break;

                case "PersonID":
                    ctrlEmployeeCard1.LoadInfoByPersonID(int.Parse(txtFilterValue.Text));
                    break;
            }

            if (ctrlEmployeeCard1.EmployeeID > 0)
            {
                OnEmployeeSelected?.Invoke(ctrlEmployeeCard1.EmployeeID);
                EmployeeFound?.Invoke(this, ctrlEmployeeCard1.EmployeeID);
            }
            else
            {
                OnEmployeeSelected?.Invoke(-1);
                EmployeeFound?.Invoke(this, -1);
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Focus();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }


            FindNow();
        }

        private void ctrlEmployeeCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Focus();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {
                // Do not cancel the event; just set the error message
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            }
            else
            {
                // Clear any existing error message
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check if the pressed key is Enter (character code 13)
            if (e.KeyChar == (char)13)
            {

                btnFind.PerformClick();
            }

            //this will allow only digits if person id is selected
            if (cbFilterBy.Text == "Employee ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);

        }

        private void btnAddNewEmployee_Click(object sender, EventArgs e)
        {
            frmAddUpdateEmployee frm = new frmAddUpdateEmployee();
            frm.DataBack += DataBackEvent;
            frm.ShowDialog();
        }

        private void DataBackEvent(object sender, int EmployeeID)
        {
            // Handle the data received

            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Text = EmployeeID.ToString();
            ctrlEmployeeCard1.LoadInfo(EmployeeID);
        }
        public void FilterFocus()
        {
            txtFilterValue.Focus();
        }







    }
}
