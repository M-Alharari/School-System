using SchoolProject.Employees;
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

namespace SchoolProject.People.Controls
{
    public partial class ctrPersonCardWithFilter : UserControl
    {   // Define a custom event handler delegate with parameters
        public event Action<int> OnPersonSelected;
        // Create a protected method to raise the event with a parameter
        protected virtual void PersonSelected(int PersonID)
        {
            Action<int> handler = OnPersonSelected;
            if (handler != null)
            {
                handler(PersonID); // Raise the event with the parameter
            }
        }
        public event EventHandler<int> PersonSelectedIsEmp; // int = PersonID

        private void OnPersonIsEmployee(int personID)
        {
            PersonSelectedIsEmp?.Invoke(this, personID);
        }

        // This delegate is optional. The parent form can assign any logic here.
        public Func<int, bool> PersonCannotBeSelectedCheck { get; set; }


        private bool _ShowAddPerson = true;
        public bool ShowAddPerson
        {
            get
            {
                return _ShowAddPerson;
            }
            set
            {
                _ShowAddPerson = value;
                btnFind.Visible = _ShowAddPerson;
            }
        }

        private bool _FilterEnabled = true;
        public bool FilterEnabled
        {
            get
            {
                return _FilterEnabled;
            }
            set
            {
                _FilterEnabled = value;
                gbFilters.Enabled = _FilterEnabled;
            }
        }
        public void FilterFocus()
        {
            txtFilterValue.Focus();
        }
        public ctrPersonCardWithFilter()
        {
            InitializeComponent();
        }


        private int _PersonID = -1;

        public int PersonID
        {
            get { return personcard1.PersonID; }
        }

        public clsPerson SelectedPersonInfo
        {
            get { return personcard1.SelectedPersonInfo; }
        }

        public void LoadPersonInfo(int PersonID)
        {

            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Text = PersonID.ToString();
            FindNow();

        }

        private void FindNow()
        {
            switch (cbFilterBy.Text)
            {
                case "Person ID":
                    personcard1.LoadPersonInfo(int.Parse(txtFilterValue.Text));
                    break;

                case "National No.":
                    personcard1.LoadPersonInfo(txtFilterValue.Text);
                    break;

                default:
                    break;
            }

            int personID = personcard1.PersonID;

            if (OnPersonSelected != null && FilterEnabled)
                OnPersonSelected(personID);

            // ✅ Call parent-provided check
            if (PersonCannotBeSelectedCheck != null && PersonCannotBeSelectedCheck(personID))
            {
                // Fire the warning event
                OnPersonIsEmployee(personID); // or rename event to something generic like PersonInvalidSelection
            }
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Text = "";
            txtFilterValue.Focus();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
           
        }

        private void ctrPersonCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Focus();
        }

        private void txtFilterValue_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtFilterValue.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            }
            else
            {
                //e.Cancel = false;
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        private void btnFind_Click_1(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }
            FindNow();
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.DataBack += DataBackEvent;
            frm.ShowDialog();
        }
        private void DataBackEvent(object sender, int NewPersonID)
        {
            txtFilterValue.Text = NewPersonID.ToString();
            personcard1.LoadPersonInfo(NewPersonID);

            // حدث الاختيار لازم يتفعل بعد تحميل البيانات
            OnPersonSelected?.Invoke(NewPersonID);
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.SelectedItem == null) return; // nothing selected yet

            string selectedType = cbFilterBy.SelectedItem.ToString();
            if (selectedType == "Person ID")
            {
                // Allow digits, control keys (Backspace, Delete, etc.) and space
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                }
            }

            else if (selectedType == "National No.")
            {
                // Allow only letters, space, and control keys
                if (!char.IsControl(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != ' ')
                {
                    e.Handled = true;
                }
            }
        }
    }
}
