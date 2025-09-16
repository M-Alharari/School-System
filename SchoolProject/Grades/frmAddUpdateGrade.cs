 
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

namespace SchoolProject.Grades
{
    public partial class frmAddUpdateGrade : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        private int _GradeID;
        clsGrade _Grade;

        public frmAddUpdateGrade()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }  public frmAddUpdateGrade(int GradeID)
        {
            InitializeComponent();
            _GradeID = GradeID;
            Mode = enMode.Update;
        
        }

        private void _ResetDefaultValues()
        {

            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Grade";
                this.lblTitle.Text = "Add New Grade";
                _Grade = new clsGrade();

            }
            else
            {
                lblTitle.Text = "Update Grade";
                this.lblTitle.Text = "Update Grade";

            }


            lblGradeID.Text = "[????]";
            txtGradeName.Text = "";

        }



        private void _LoadData()
        {
            _Grade =clsGrade.FindGradeByID(_GradeID);
            if (_Grade == null)
            {
                MessageBox.Show("No Subject with ID: " + _GradeID, "Subject Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            txtGradeName.Text = _Grade.GradeName;
            lblGradeID.Text = _Grade.GradeID.ToString();

        }

        private void frmAddUpdateGrade_Load(object sender, EventArgs e)
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
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _Grade.GradeName = txtGradeName.Text;


            if (_Grade.Save())
            {
                lblGradeID.Text = _Grade.GradeID.ToString();
                //txtAcademicYear.Text = _Position.AcademicYear.ToString();

                //change form mode to update.
                Mode = enMode.Update;
                lblTitle.Text = "Update Class";
                this.Text = "Update Class";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Trigger the event to send data back to the caller form.
                //DataBack?.Invoke(this, _Employee.EmployeeID);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
 }

