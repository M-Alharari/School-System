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

namespace SchoolProject.Positions
{
    public partial class frmAddUpdatePosition : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        private int _PositionID;
        clsPosition _Position;
        public frmAddUpdatePosition()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }  
        public frmAddUpdatePosition(int PositionID)
        {
            InitializeComponent();
            _PositionID = PositionID;
            Mode = enMode.Update;
        }



        private void _ResetDefaultValues()
        {

            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Position";
                this.lblTitle.Text = "Add New Position";
                _Position = new clsPosition();

            }
            else
            {
                lblTitle.Text = "Update Position";
                this.lblTitle.Text = "Update Position";

            }


            lblPositionID.Text = "[????]";
            txtPositionName.Text = "";

        }



        private void _LoadData()
        {
            _Position = clsPosition.Find(_PositionID);
            if (_Position == null)
            {
                MessageBox.Show("No Subject with ID: " + _PositionID, "Position Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            txtPositionName.Text = _Position.PositionName;
            lblPositionID.Text = _Position.PositionID.ToString();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Position.PositionName = txtPositionName.Text.ToString();


            if (_Position.Save())
            {
                lblPositionID.Text = _Position.PositionID.ToString();
                //txtAcademicYear.Text = _Position.AcademicYear.ToString();

                //change form mode to update.
                Mode = enMode.Update;
                lblTitle.Text = "Update Position";
                this.Text = "Update Position";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Trigger the event to send data back to the caller form.
                //DataBack?.Invoke(this, _Employee.EmployeeID);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void frmAddUpdatePosition_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (Mode == enMode.Update)
            {
                _LoadData();

            }
        }
    }
}
