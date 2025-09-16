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
    public partial class frmListFeesTypes : Form
    {
        //  clsTuitionPayment _TuitionPayment;
        //  private static DataTable _dtAllPayment = clsTuitionPayment.GetAllTuition();
        //  private DataTable _dtPayment = _dtAllPayment.DefaultView.ToTable(false,
        //"_TuitionFeeID", "FullName", "TotalFees", "PaymentMode", "InstallmentFrequency", "DueAmount", "IsFullyPaid",
        //"Position", "EmployeeStatus");


        //  private void _RefreshEmployeesList()
        //  {

        //      _dtAllPayment = clsEmployee.GetAllEmployees(); // ✅ correct source
        //      _dtPayment = _dtPayment = _dtAllPayment.DefaultView.ToTable(false,
        //"_TuitionFeeID", "FullName", "TotalFees", "PaymentMode", "InstallmentFrequency", "DueAmount", "IsFullyPaid"
        // );

        //      dgvPayments.DataSource = _dtPayment;
        //      lblRecordCount.Text = dgvPayments.Rows.Count.ToString();


        //  }
        //dgvPayments.DataSource = clsTuitionPayment.GetAllTuition();
        //    dgvPayments.DataSource = _dtPayment;
        //    cbFilterBy.SelectedIndex = 0;
        //    lblRecordCount.Text = dgvPayments.Rows.Count.ToString();

        //    if (dgvPayments.Rows.Count > 0)
        //    {
        //        dgvPayments.Columns[0].HeaderText = "_TuitionFeeID";
        //        dgvPayments.Columns[0].Width = 100;

        //        dgvPayments.Columns[1].HeaderText = "FullName";
        //        dgvPayments.Columns[1].Width = 80;

        //        dgvPayments.Columns[2].HeaderText = "TotalFees";
        //        dgvPayments.Columns[2].Width = 200;



        //        dgvPayments.Columns[3].HeaderText = "PaymentMode";
        //        dgvPayments.Columns[3].Width = 80;


        //        dgvPayments.Columns[4].HeaderText = "InstallmentFrequency";
        //        dgvPayments.Columns[4].Width = 120;



        //        dgvPayments.Columns[5].HeaderText = "DueAmount";
        //        dgvPayments.Columns[5].Width = 150;
        //        dgvPayments.Columns[5].DefaultCellStyle.Format = "D";

        //        dgvPayments.Columns[6].HeaderText = "IsFullyPaid"; // 🟢 Correct column index
        //        dgvPayments.Columns[6].Width = 80;

        //        // 🟢 Format the actual HiredDate column (index 4 or use "HiredDate" if you prefer)


        //        dgvPayments.Columns[7].HeaderText = "Employee Status";
        //        dgvPayments.Columns[7].Width = 100;
        //    }
        public frmListFeesTypes()
        {
            InitializeComponent();
        }
    }
}
