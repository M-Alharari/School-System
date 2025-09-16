using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsFinancialDashboard
    {
        // Total revenue (paid tuition)
        public decimal TotalRevenue => clsFinancialData.GetTotalRevenue();

        // Total tuition (all payments, paid + unpaid)
        public decimal TotalTuition => clsFinancialData.GetTotalTuition();

        // Total receivable (unpaid tuition)
        public decimal Receivable => clsFinancialData.GetReceivable();

        // Tuition collection rate (%)
        public decimal CollectionRate => clsFinancialData.GetCollectionRate();

        // Total expenses (if Expenses table exists)
        //public decimal TotalExpenses => clsFinancialData.GetTotalExpenses();

        // Paid vs Unpaid data for chart
        public DataTable PaidVsUnpaidChartData => clsFinancialData.GetPaidVsUnpaid();

        // Outstanding payments over time for chart
        public DataTable OutstandingPaymentsOverTime => clsFinancialData.GetOutstandingPaymentsOverTime();

        // Total payroll expenses (sum of NetSalary)
        // Total payroll expenses
        public decimal TotalPayrollExpenses => clsFinancialData.GetTotalPayrollExpenses();

        // Total payroll expenses for a specific month/year
        // Total payroll expenses
        //public static decimal TotalPayrollExpenses => clsPayroll.GetTotalPayrollExpenses();
        // Optional: Profit calculation
        public decimal Profit => TotalRevenue - TotalPayrollExpenses;
        // ===== NEW: Monthly Revenue & Receivable Trend =====
        public DataTable MonthlyFinancialTrend => clsFinancialData.GetMonthlyRevenueTrend();

        // Optional: Method to refresh all metrics (if you want to cache them)
        public void RefreshMetrics()
        {
            // Accessing properties will call data layer methods again
            decimal rev = TotalRevenue;
            decimal exp = TotalPayrollExpenses;
            decimal receiv = Receivable;
            decimal rate = CollectionRate;
        }
    }
}
