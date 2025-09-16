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

namespace SchoolProject.Guardians
{
    public partial class frmAddUpdateGuardian : Form
    {
        // Declare a delegate
        public delegate void DataBackEventHandler(object sender, int PersonID);

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;


        public int RecievedGuardianID { get; private set; } // Public property [[3]]
        public int SelectedGuardianID { get; set; }
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            // Logic to add an item (e.g., increment the count)
            RecievedGuardianID = _Guardian.GuardianID;
        }
        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        private int _GuardianID;
        clsGuardian _Guardian;
        clsPerson _Person;

        private DateTime _Date;
        public frmAddUpdateGuardian()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }
        public frmAddUpdateGuardian(int GuardianID)
        {
            InitializeComponent();
            _GuardianID = GuardianID;
          
            Mode = enMode.Update; //❗ هذا مهم، كان AddNew بالخطأ
        }


        private void _ResetDefaultValues()
        {

            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Guardian";
                this.lblTitle.Text = "Add New Guardian";
                _Guardian = new clsGuardian();
                ctGuardianCardWithFilter1.FilterFocus();

                gbGuardian.Enabled = true;
                //lblHiredDate.Text = DateTime.Now.ToString();

            }
            else
            {
                lblTitle.Text = "Update Guardian";
                this.lblTitle.Text = "Update Guardian";
                gbGuardian.Enabled = true;

            }


            lblGuardianID.Text = "[???]";
            




        }




        private void _LoadData()
        {
            _Guardian = clsGuardian.FindGuardianByID(_GuardianID);
            ctGuardianCardWithFilter1.FilterEnabled = false;

            if (_Guardian == null)
            {
                MessageBox.Show("No Guardian with ID: " + _GuardianID, "Guardian Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                gbGuardian.Enabled = false;
                return;
            }

            lblGuardianID.Text = _Guardian.GuardianID.ToString();
            txtRelationship.Text = _Guardian.Relationship; // load relationship
            gbGuardian.Enabled = true;

            ctGuardianCardWithFilter1.LoadPersonInfo(_Guardian.PersonID);
        }


        private void frmAddUpdateGuardian_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (Mode == enMode.Update)
            {
                _LoadData();

            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Hover over the red icon(s) to see the error.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Assign PersonID and Relationship
            _Guardian.PersonID = ctGuardianCardWithFilter1.PersonID;
            _Guardian.Relationship = txtRelationship.Text.Trim();

            // Save with current user for audit
            if (_Guardian.Save(clsGlobal.CurrentUser.UserID))
            {
                lblGuardianID.Text = _Guardian.GuardianID.ToString();
                DataBack?.Invoke(this, _Guardian.GuardianID); // Send data back to the caller form

                // Switch mode to Update
                Mode = enMode.Update;
                lblTitle.Text = "Update Guardian";
                this.Text = "Update Guardian";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error: Data was not saved successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
