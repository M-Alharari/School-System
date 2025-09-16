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

namespace SchoolProject.Fees_and_Payments
{
    public partial class frmAddUpdatePaymentTypes : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;

        private int _PaymentTypeID;
        private clsPaymentType _PaymentType;


        public frmAddUpdatePaymentTypes()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }



        public frmAddUpdatePaymentTypes(int PaymentTypeID)
        {
            InitializeComponent();
            _PaymentTypeID = PaymentTypeID;
            Mode = enMode.Update;
        }

        private void _ResetForm()
        {
            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Payment Type";
                _PaymentType = new clsPaymentType();
            }
            else
            {
                lblTitle.Text = "Update Payment Type";
            }

            //lblPaymentTypeID.Text = "[????]";
            txtPaymentTypeName.Text = "";
        }

        private void _LoadData()
        {
            _PaymentType = clsPaymentType.Find(_PaymentTypeID);
            if (_PaymentType == null)
            {
                MessageBox.Show("No Payment Type with ID: " + _PaymentTypeID,
                    "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            txtPaymentTypeName.Text = _PaymentType.PaymentTypeName;
            //lblPaymentTypeID.Text = _PaymentType.PaymentTypeID.ToString();
        }

        private void frmAddUpdatePaymentTypes_Load(object sender, EventArgs e)
        {
            _ResetForm();

            if (Mode == enMode.Update)
                _LoadData();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
            {
                MessageBox.Show("Some fields are invalid. Hover over red icons to see errors.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _PaymentType.PaymentTypeName = txtPaymentTypeName.Text.Trim();

            if (_PaymentType.Save())
            {
                //lblPaymentTypeID.Text = _PaymentType.PaymentTypeID.ToString();
                Mode = enMode.Update;
                lblTitle.Text = "Update Payment Type";

                MessageBox.Show("Payment Type saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error saving Payment Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
