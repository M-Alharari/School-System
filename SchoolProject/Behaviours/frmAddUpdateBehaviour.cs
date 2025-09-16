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

namespace SchoolProject.Behaviours
{
    public partial class frmAddUpdateBehaviour : Form
    {  // Delegate & Event to send data back to parent form
        public delegate void DataBackEventHandler(object sender, int BehaviourID);
        public event DataBackEventHandler DataBack;

        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private int _enrolledID;
        private int _behaviourID; // for update mode
        private clsStudentBehaviour _behaviour;
        public frmAddUpdateBehaviour(int enrolledID)
        {
            InitializeComponent(); _enrolledID = enrolledID;
            Mode = enMode.AddNew;
        }
        public frmAddUpdateBehaviour(int enrolledID, int behaviourID)
        {
            InitializeComponent(); _enrolledID = enrolledID;
            _enrolledID = enrolledID;  // ✅ good
            _behaviourID = behaviourID;
            Mode = enMode.Update;
        }
        private void frmAddUpdateBehaviour_Load(object sender, EventArgs e)
        {
            _behaviour = new clsStudentBehaviour();
            _LoadComboBoxes();

            if (Mode == enMode.Update)
                _LoadData();

            _ResetDefaultValues();
        }

        private void _LoadComboBoxes()
        {
            // Behaviour Types
            DataTable dtTypes = clsStudentBehaviour.GetBehaviourTypes();
            cmbBehaviourType.DisplayMember = "Name";
            cmbBehaviourType.ValueMember = "BehaviourTypeID";
            cmbBehaviourType.DataSource = dtTypes;

            // Categories
            DataTable dtCategories = clsStudentBehaviour.GetAll();
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "CategoryID";
            cmbCategory.DataSource = dtCategories;


            // Severity Levels
            DataTable dtSeverity = clsStudentBehaviour.GetSeverityLevels();
            cmbSeverity.DisplayMember = "Name";
            cmbSeverity.ValueMember = "SeverityLevelID";
            cmbSeverity.DataSource = dtSeverity;

            // Actions
            DataTable dtActions = clsStudentBehaviour.GetActions();
            cmbAction.DisplayMember = "Name";
            cmbAction.ValueMember = "ActionID";
            cmbAction.DataSource = dtActions;
        }


        private void _ResetDefaultValues()
        {
            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Behaviour";

                if (cmbBehaviourType.Items.Count > 0)
                    cmbBehaviourType.SelectedIndex = 0;
                else
                    cmbBehaviourType.SelectedIndex = -1;

                if (cmbSeverity.Items.Count > 0)
                    cmbSeverity.SelectedIndex = 0;
                else
                    cmbSeverity.SelectedIndex = -1;

                if (cmbAction.Items.Count > 0)
                    cmbAction.SelectedIndex = 0;
                else
                    cmbAction.SelectedIndex = -1;

                txtDescription.Text = "";
            }
            else
            {
                lblTitle.Text = "Update Behaviour";
            }
        }


        private void _LoadData()
        {
            DataTable dt = clsStudentBehaviour.GetBehaviourByID(_behaviourID);
            if (dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];
            cmbBehaviourType.SelectedValue = row["BehaviourTypeID"];
            cmbSeverity.SelectedValue = row["SeverityLevelID"];
            cmbAction.SelectedValue = row["ActionTakenID"];
            txtDescription.Text = row["Description"].ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbBehaviourType.SelectedValue == null || cmbSeverity.SelectedValue == null || cmbAction.SelectedValue == null)
            {
                MessageBox.Show("Select Type, Severity, and Action.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _behaviour.EnrolledID = _enrolledID;
            _behaviour.BehaviourTypeID = (int)cmbBehaviourType.SelectedValue;
            _behaviour.SeverityLevelID = (int)cmbSeverity.SelectedValue;
            _behaviour.ActionTakenID = (int)cmbAction.SelectedValue;
            _behaviour.Description = txtDescription.Text.Trim();
            _behaviour.RecordedBy = clsGlobal.CurrentUser.UserID;
            _behaviour.CreatedBy = clsGlobal.CurrentUser.UserID;
            _behaviour.CategoryID = (int)cmbCategory.SelectedValue; // NOT cmbAction.SelectedValue
          _behaviour.CreatedDate = DateTime.Now;

            if (Mode == enMode.AddNew)
            {
                int newID = _behaviour.Add();
                MessageBox.Show("Behaviour added successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataBack?.Invoke(this, newID);
            }
            else
            {
                _behaviour.BehaviourID = _behaviourID;
                _behaviour.Update();
                MessageBox.Show("Behaviour updated successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DataBack?.Invoke(this, _behaviourID);
            }

          
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmAddUpdateBehaviour_Load_1(object sender, EventArgs e)
        {
            _behaviour = new clsStudentBehaviour();
            _LoadComboBoxes();

            if (Mode == enMode.Update)
                _LoadData();
            else
                _ResetDefaultValues();
        }
    }
}
