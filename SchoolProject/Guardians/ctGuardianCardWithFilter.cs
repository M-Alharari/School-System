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

namespace SchoolProject.Guardians
{
    public partial class ctGuardianCardWithFilter : UserControl
    {
        // Define a custom event handler delegate with parameters
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

       
        public ctGuardianCardWithFilter()
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

                case "Guardian ID": // ✅ New case
                    int guardianID = int.Parse(txtFilterValue.Text);
                    // Assuming you have a method to map guardian → person
                    int? personID = clsGuardian.GetPersonIDByGuardianID(guardianID);

                    if (personID.HasValue)
                        personcard1.LoadPersonInfo(personID.Value);
                    else
                        MessageBox.Show("No person found for this Guardian ID.",
                                        "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                default:
                    break;
            }

            int personIDCheck = personcard1.PersonID;

            if (OnPersonSelected != null && FilterEnabled)
                OnPersonSelected(personIDCheck);

            if (PersonCannotBeSelectedCheck != null && PersonCannotBeSelectedCheck(personIDCheck))
            {
                OnPersonIsEmployee(personIDCheck);
            }
        }


        private void btnFind_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are invalid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            FindNow();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Text = "";
            txtFilterValue.Focus();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFilterValue.Text))
            {
                //e.Cancel = true; // Prevent focus from leaving the control
                errorProvider1.SetError(txtFilterValue, "This field is required!");
            }
            else
            {
                //e.Cancel = false; // Allow focus to leave the control
                errorProvider1.SetError(txtFilterValue, null);
            }
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.DataBack += (s, newPersonID) =>
            {
                txtFilterValue.Text = newPersonID.ToString();
                personcard1.LoadPersonInfo(newPersonID);
                OnPersonSelected?.Invoke(newPersonID);
            };
            frm.ShowDialog();
        }

        private void ctGuardianCardWithFilter_Load(object sender, EventArgs e)
        {
            cbFilterBy.Items.Clear();
            cbFilterBy.Items.Add("Person ID");
            cbFilterBy.Items.Add("National No.");
            cbFilterBy.Items.Add("Guardian ID"); // ✅ New option
            cbFilterBy.SelectedIndex = 0;
            txtFilterValue.Focus();
        }
    }
}
