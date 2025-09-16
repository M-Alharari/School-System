using SchoolProject.Fees_and_Payments;
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
using static SchoolProject.Fees_and_Payments.frmInstallmnetRecord;

namespace SchoolProject.Fees_and_Payments
{
    public partial class frmInstallmnetRecord : Form
    {
        private Timer notificationTimer;
        private int _TuitionFeeID;
        clsInstallment _Installment;
        private void RefreshInstallments()
        {
            // Reload installments from DB
            LoadInstallments();

            // Update Pay button states (Paid/Pay)
            UpdatePayButtons();
        }

        private void LoadInstallments()
        {
            var installments = clsInstallment.GetInstallmentSummaryByTuitionFeeID(_TuitionFeeID);
            string Name = clsTuitionPayment.FindStudentFullName(_TuitionFeeID);
            lblFullName.Text = Name;    
            dgvInstallments.DataSource = installments;
            dgvInstallments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // إضافة زر Pay مرة واحدة فقط
            if (!dgvInstallments.Columns.Contains("PayButton"))
            {
                DataGridViewButtonColumn payButton = new DataGridViewButtonColumn();
                payButton.Name = "PayButton";
                payButton.HeaderText = "Action";
                payButton.Text = "Pay";
                payButton.UseColumnTextForButtonValue = true;
                dgvInstallments.Columns.Add(payButton);
            }
        }


        public frmInstallmnetRecord(int tuitionFeeID)
        {
            InitializeComponent(); _TuitionFeeID = tuitionFeeID;
            dgvInstallments.CellContentClick += dgvInstallments_CellContentClick;
            // Setup timer for notifications
            notificationTimer = new Timer();
            notificationTimer.Interval = 10000; // 10 seconds
            //notificationTimer.Tick += NotificationTimer_Tick;
        }


        private void dgvInstallments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (dgvInstallments.Columns[e.ColumnIndex].Name != "PayButton") return;

            int installmentID = Convert.ToInt32(dgvInstallments.Rows[e.RowIndex].Cells["InstallmentID"].Value);

            using (var payForm = new frmPayInstallment(installmentID))
            {
                payForm.ShowDialog();
                RefreshInstallments();
                //if (payForm.ShowDialog() == DialogResult.OK)
                //{
                //    LoadInstallments();
                //    UpdatePayButtons();
                //}
            }
        }
        private void UpdatePayButtons()
        {
            foreach (DataGridViewRow row in dgvInstallments.Rows)
            {
                bool isPaid = Convert.ToBoolean(row.Cells["IsPaid"].Value);
                DataGridViewButtonCell buttonCell = (DataGridViewButtonCell)row.Cells["PayButton"];
                if (isPaid)
                {
                    buttonCell.Value = "Paid";
                    buttonCell.ReadOnly = true;
                    buttonCell.Style.ForeColor = Color.Gray;
                }
                else
                {
                    buttonCell.Value = "Pay";
                    buttonCell.ReadOnly = false;
                    buttonCell.Style.ForeColor = Color.Black;
                }
            }
        }
        // Notification timer event
        //private void NotificationTimer_Tick(object sender, EventArgs e)
        //{
        //    DataTable allInstallments = clsInstallment.GetAllInstallments();

        //    var dueTuitionFeeIDs = new HashSet<int>();

        //    foreach (DataRow row in allInstallments.Rows)
        //    {
        //        DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
        //        bool isPaid = Convert.ToBoolean(row["IsPaid"]);
        //        int tuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]);

        //        if (!isPaid && dueDate <= DateTime.Today)
        //        {
        //            dueTuitionFeeIDs.Add(tuitionFeeID);
        //        }
        //    }

        //    if (dueTuitionFeeIDs.Count > 0)
        //    {
        //        frmNotification frm = new frmNotification(dueTuitionFeeIDs.ToList());
        //        frm.Show();
        //    }
        //}


        // حدث التايمر: تحقق الأقساط المستحقة وغير المدفوعة

        // كلاس الأقساط
        public class Installment
        {
            public int InstallmentNumber { get; set; }
            public DateTime DueDate { get; set; }
            public decimal Amount { get; set; }
            public bool IsPaid { get; set; }
        }

       



        private void frmPaymentHistory_Load(object sender, EventArgs e)
        {
            LoadInstallments();
            UpdatePayButtons();
            notificationTimer.Start(); // شغّل التايمر عند تحميل الفورم
        }
    }
}
// Notification form that internally loads due installments and displays them
// نموذج التنبيه الذي يعرض الأقساط المستحقة لكل TuitionFeeID
//public class frmNotification : Form
//{
//    private FlowLayoutPanel panel;
//    private List<int> _tuitionFeeIDs;

//    public frmNotification(List<int> tuitionFeeIDs)
//    {
//        _tuitionFeeIDs = tuitionFeeIDs;

//        Text = "Installments Due Notification";
//        Size = new Size(600, 400);
//        StartPosition = FormStartPosition.CenterScreen;
//        FormBorderStyle = FormBorderStyle.FixedToolWindow;

//        panel = new FlowLayoutPanel
//        {
//            Dock = DockStyle.Fill,
//            AutoScroll = true,
//            Padding = new Padding(10)
//        };

//        LoadDueInstallments();

//        Controls.Add(panel);
//    }

//    private void LoadDueInstallments()
//    {
//        foreach (int tuitionFeeID in _tuitionFeeIDs)
//        {
//            DataTable dt = clsInstallment.GetInstallmentSummaryByTuitionFeeID(tuitionFeeID);

//            var dueRows = dt.Select($"DueDate <= #{DateTime.Today:MM/dd/yyyy}# AND IsPaid = false");

//            foreach (DataRow row in dueRows)
//            {
//                string Name = row["FullName"].ToString();
//                int installmentID = Convert.ToInt32(row["InstallmentID"]);
//                int installmentNumber = Convert.ToInt32(row["InstallmentNumber"]);
//                DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
//                decimal amount = Convert.ToDecimal(row["Amount"]);

//                Label lbl = new Label
//                {
//                    Text = $"Name: {Name} - TuitionFeeID: {tuitionFeeID} - Installment #{installmentNumber} - Due: {dueDate:dd/MM/yyyy} - Amount: {amount:0.00}",
//                    AutoSize = true,
//                    Padding = new Padding(5),
//                    Margin = new Padding(3)
//                };

//                Button btnPay = new Button
//                {
//                    Text = "Show Install",
//                    Tag = installmentID,
//                    AutoSize = true,
//                    Margin = new Padding(3)
//                };
//                btnPay.Click += BtnPay_Click;

//                panel.Controls.Add(lbl);
//                panel.Controls.Add(btnPay);
//            }
//        }
//    }

//    private void BtnPay_Click(object sender, EventArgs e)
//    {
//        Button btn = sender as Button;
//        if (btn == null) return;

//        int installmentID = (int)btn.Tag;
//        var payForm = new frmPayInstallment(installmentID);
//        var result = payForm.ShowDialog();

//        if (result == DialogResult.OK)
//        {
//            MessageBox.Show("Payment completed successfully.");
//            Close(); // يغلق نافذة التنبيه بعد الدفع
//        }
//    }
//}

 