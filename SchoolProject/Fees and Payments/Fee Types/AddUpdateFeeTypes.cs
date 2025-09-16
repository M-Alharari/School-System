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

namespace SchoolProject.Fees_and_Payments.Fee_Types
{
    public partial class AddUpdateFeeTypes : Form
    {
        private clsFeeType _feeType;
        private bool _isUpdateMode;

        public AddUpdateFeeTypes()
        {
            InitializeComponent(); _isUpdateMode = false;
            _feeType = new clsFeeType();
        }

        public AddUpdateFeeTypes(clsFeeType feeType)
        {
            InitializeComponent();
            _isUpdateMode = true;
            _feeType = feeType;

            // Load values into form for editing
            txtFeeTypeName.Text = _feeType.FeeTypeName;
            //chkIsDefault.Checked = _feeType.IsDefault;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFeeTypeName.Text))
            {
                MessageBox.Show("Please enter a fee type name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _feeType.FeeTypeName = txtFeeTypeName.Text.Trim();
            //_feeType.IsDefault = chkIsDefault.Checked;

            bool success;
            int currentuser = clsGlobal.CurrentUser.UserID;
            if (_isUpdateMode)
            {
                success = _feeType.Save(currentuser);
            }
            else
            {
                success = _feeType.Save(currentuser);
            }

            if (success)
            {
                MessageBox.Show("Fee type saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save fee type. Please check your data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
