 
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
    public partial class frmfinancialdashboard : Form
    {
        private clsFinancialDashboard dashboard;
        public frmfinancialdashboard()
        {
            InitializeComponent(); LoadDashboard();
        }

        private void LoadDashboard()
        {
            dashboard = new clsFinancialDashboard();

            // ===== Labels =====

            // Set fixed width and center text
            CenterLabelInPanel(lblTotalRevenue, panelTotalRevenue, "EGP " + dashboard.TotalRevenue.ToString("N2"));
            CenterLabelInPanel(lblReceivable, panelReceivable, "EGP " + dashboard.Receivable.ToString("N2"));
            CenterLabelInPanel(lblTotalExpenses, panelTotalExpenses, "EGP " + dashboard.TotalPayrollExpenses.ToString("N2"));
            CenterLabelInPanel(lblCollectionRate, panelCollectionRate, dashboard.CollectionRate.ToString("0.00") + "%");
            CenterLabelInPanel(lblProfit, panelProfit, "EGP " + dashboard.Profit.ToString("N2"));
            // ===== Charts =====

            // Chart: Paid vs Unpaid Tuition
            chartPaidVsUnpaid.DataSource = dashboard.PaidVsUnpaidChartData;
            chartPaidVsUnpaid.Series[0].XValueMember = "Status";
            chartPaidVsUnpaid.Series[0].YValueMembers = "Total";
            chartPaidVsUnpaid.DataBind();

            // Chart: Outstanding Payments Over Time
            chartOutstandingPayments.DataSource = dashboard.OutstandingPaymentsOverTime;
            chartOutstandingPayments.Series[0].XValueMember = "DueDate";
            chartOutstandingPayments.Series[0].YValueMembers = "OutstandingAmount";
            chartOutstandingPayments.DataBind();

            // Chart: Monthly Trends (Revenue vs Receivable)
            chartMonthlyTrends.Series.Clear();

            chartMonthlyTrends.Series.Add("Paid");
            chartMonthlyTrends.Series["Paid"].XValueMember = "Month";
            chartMonthlyTrends.Series["Paid"].YValueMembers = "TotalPaid";

            chartMonthlyTrends.Series.Add("Receivable");
            chartMonthlyTrends.Series["Receivable"].XValueMember = "Month";
            chartMonthlyTrends.Series["Receivable"].YValueMembers = "TotalReceivable";

            chartMonthlyTrends.DataSource = dashboard.MonthlyFinancialTrend;
            chartMonthlyTrends.DataBind();
        }

        // Helper method to center label in panel
        private void CenterLabelInPanel(Label lbl, Panel pnl, string text)
        {
            lbl.AutoSize = false;                // Fix label width
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.Text = text;

            // Make label fill the panel horizontally
            lbl.Width = pnl.Width;
            lbl.Left = 0;
        }

    }
}
