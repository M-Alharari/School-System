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

namespace SchoolProject.Terms
{
    public partial class frmaddUpdateTerm : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private int _TermID;
        private clsTerm _Term;
        public frmaddUpdateTerm()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }
        public frmaddUpdateTerm(int TermID)
        {
            InitializeComponent();
            _TermID = TermID;
            Mode = enMode.Update;
        }

        private void frmaddUpdateTerm_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (Mode == enMode.Update)
                _LoadData();
        }
        private void _ResetDefaultValues()
        {
            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Term";
                _Term = new clsTerm();
            }
            else
            {
                lblTitle.Text = "Update Term";
            }

            lblTermID.Text = "[????]";
            txtTermName.Text = "";

            // Reset DateTimePickers to today
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today;
            chkFinal.Checked = false;
        }

        private void _LoadData()
        {
            _Term = clsTerm.Find(_TermID);
            if (_Term == null)
            {
                MessageBox.Show("No Term with ID: " + _TermID, "Term Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            txtTermName.Text = _Term.TermName;

            // Load dates into DateTimePickers
            dtpStart.Value = _Term.StartDate;
            dtpEnd.Value = _Term.EndDate;

            lblTermID.Text = _Term.TermID.ToString();
            chkFinal.Checked = _Term.IsFinal;
        }


        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show(
                    "Some fields are not valid! Hover over the red icon(s) to see the error.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error
                );
                return;
            }

            // Assign term name
            _Term.TermName = txtTermName.Text.Trim();
            _Term.IsFinal = chkFinal.Checked;
            // Assign dates directly from DateTimePickers
            _Term.StartDate = dtpStart.Value.Date;
            _Term.EndDate = dtpEnd.Value.Date;

            if (_Term.StartDate > _Term.EndDate)
            {
                MessageBox.Show("Start Date cannot be after End Date.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Save depending on mode
            if (Mode == enMode.AddNew)
            {
                if (_Term.Save())
                {
                    // Make second term final automatically after adding
                    //clsTerm.SetFinalTermAutomatically();

                    lblTermID.Text = _Term.TermID.ToString();
                    Mode = enMode.Update;
                    lblTitle.Text = "Update Term";
                    this.Text = "Update Term";

                    MessageBox.Show("Term Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error: Term not saved.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else // Update mode
            {
                if (_Term.Save())
                {
                    MessageBox.Show("Term Updated Successfully.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error: Term not updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }

}