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
    public partial class frmPendingPaymentsReport : Form
    {
        private static DataTable _dtAllPaymentTypes = clsPaymentType.GetAllPaymentTypes();

        private DataTable _dtPaymentTypes = _dtAllPaymentTypes.DefaultView.ToTable(false, "PaymentTypeID", "PaymentTypeName");

        public frmPendingPaymentsReport()
        {
            InitializeComponent();
        }

        private void _RefreshPaymentTypeList()
        {
            _dtAllPaymentTypes = clsPaymentType.GetAllPaymentTypes();
            _dtPaymentTypes = _dtAllPaymentTypes.DefaultView.ToTable(false, "PaymentTypeID", "PaymentTypeName");

            dgvPaymentTypes.DataSource = _dtPaymentTypes;
            lblRecordCount.Text = dgvPaymentTypes.Rows.Count.ToString();
        }

        private void frmPendingPaymentsReport_Load(object sender, EventArgs e)
        {
            _RefreshPaymentTypeList();

            //cbFilterBy.SelectedIndex = 0;

            if (dgvPaymentTypes.Rows.Count > 0)
            {
                dgvPaymentTypes.Columns[0].HeaderText = "PaymentTypeID";
                dgvPaymentTypes.Columns[0].Width = 80;

                dgvPaymentTypes.Columns[1].HeaderText = "Payment Type";
                dgvPaymentTypes.Columns[1].Width = 180;
            }
        }

        private void btnAddNewPaymentType_Click(object sender, EventArgs e)
        {
            frmAddUpdatePaymentTypes frm = new frmAddUpdatePaymentTypes();
            frm.ShowDialog();
            _RefreshPaymentTypeList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePaymentTypes frm = new frmAddUpdatePaymentTypes((int)dgvPaymentTypes.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _RefreshPaymentTypeList();
        } 

        // private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    int id = (int)dgvPaymentTypes.CurrentRow.Cells[0].Value;
        //    string name = dgvPaymentTypes.CurrentRow.Cells[1].Value.ToString();

        //    if (MessageBox.Show($"Are you sure you want to delete Payment Type [{name}]?",
        //        "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
        //    {
        //        if (clsPaymentType.DeletePaymentType(id))
        //        {
        //            MessageBox.Show("Deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //            _RefreshPaymentTypeList();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Failed to delete. It may be linked to other records.",
        //                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}
    }
}
