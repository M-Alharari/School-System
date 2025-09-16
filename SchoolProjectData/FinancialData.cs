using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsFinancialData
    {
        /// <summary>
        /// Gets the total payroll expenses (sum of NetSalary)
        /// </summary>
        /// <returns>Total NetSalary as decimal</returns>
        public static decimal GetTotalPayrollExpenses()
        {
            decimal total = 0;

            string query = "SELECT ISNULL(SUM(NetSalary), 0) FROM PayrollPayments";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }

        /// <summary>
        /// Optionally, get total expenses for a specific month/year
        /// </summary>
        public static decimal GetTotalPayrollExpenses(int year, int month)
        {
            decimal total = 0;
            string query = "SELECT ISNULL(SUM(NetSalary), 0) FROM PayrollPayments WHERE Year=@Year AND Month=@Month";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    total = Convert.ToDecimal(result);
            }

            return total;
        }

        // 1️⃣ Total Revenue (Paid Tuition)
        // 1️⃣ Total Revenue (Paid Tuition)
        public static decimal GetTotalRevenue()
        {
            string query = @"
        SELECT ISNULL(SUM(PaidAmount), 0) AS TotalPaid
        FROM TuitionPayments;
    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();

                // Safe check against DBNull
                if (result == DBNull.Value || result == null)
                    return 0;

                return Convert.ToDecimal(result);
            }
        }



        // 2️⃣ Total Tuition (All Tuition)
        public static decimal GetTotalTuition()
        {
            string query = "SELECT SUM(TotalFees) FROM TuitionPayments";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                // Safely handle DBNull
                return result == DBNull.Value ? 0m : Convert.ToDecimal(result);
            }
        }


        // 3️⃣ Receivable (Unpaid Tuition)
        public static decimal GetReceivable()
        {
            string query = @"
        SELECT SUM(TotalFees - PaidAmount) AS Receivable
        FROM TuitionPayments
        WHERE IsFullyPaid = 0
    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                object result = cmd.ExecuteScalar();
                // Handle DBNull safely
                decimal receivable = result == DBNull.Value ? 0m : Convert.ToDecimal(result);
                return receivable;
            }
        }



        // 4️⃣ Collection Rate (%)
        public static decimal GetCollectionRate()
        {
            decimal totalRevenue = GetTotalRevenue();
            decimal totalTuition = GetTotalTuition();
            return totalTuition == 0 ? 0 : (totalRevenue / totalTuition) * 100;
        }

        // 5️⃣ Total Expenses (if you have Expenses table)
        public static decimal GetTotalExpenses()
        {
            string query = "SELECT SUM(Amount) FROM Expenses";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar() ?? 0);
            }
        }

        // 6️⃣ Paid vs Unpaid Tuition for chart
        public static DataTable GetPaidVsUnpaid()
        {
            string query = @"
        SELECT Status, SUM(Total) AS Total
FROM (
    -- From TuitionPayments
    SELECT 
        CASE WHEN CreatedDate IS NOT NULL THEN 'Paid' ELSE 'Unpaid' END AS Status,
        PaidAmount AS Total
    FROM TuitionPayments

    UNION ALL

    -- From Installments
    SELECT 
        CASE WHEN IsPaid = 1 THEN 'Paid' ELSE 'Unpaid' END AS Status,
        Amount AS Total
    FROM Installments
) AS Combined
GROUP BY Status
    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // 7️⃣ Outstanding Payments Over Time for chart
        public static DataTable GetOutstandingPaymentsOverTime()
        {
            string query = @"
                SELECT 
    CAST(i.DueDate AS DATE) AS DueDate,
    SUM(i.Amount) AS OutstandingAmount
FROM Installments i
WHERE i.IsPaid = 0
GROUP BY CAST(i.DueDate AS DATE)
ORDER BY CAST(i.DueDate AS DATE);

            ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        // Get Monthly Revenue Trend
        public static DataTable GetMonthlyRevenueTrend()
        {
            string query = @"
            -- Monthly Paid and Receivable Trend (Tuition + Installments)
SELECT 
    FORMAT(PaymentMonth, 'yyyy-MM') AS Month,
    SUM(PaidAmount) AS TotalPaid,
    SUM(ReceivableAmount) AS TotalReceivable
FROM (
    -- TuitionPayments (direct payments)
    SELECT 
        CAST(CreatedDate AS DATE) AS PaymentMonth,
        ISNULL(PaidAmount,0) AS PaidAmount,
        (TotalFees - ISNULL(PaidAmount,0)) AS ReceivableAmount
    FROM TuitionPayments
    WHERE PaidAmount > 0

    UNION ALL

    -- Paid and unpaid Installments
    SELECT 
        CAST(DueDate AS DATE) AS PaymentMonth,
        CASE WHEN IsPaid = 1 THEN Amount ELSE 0 END AS PaidAmount,
        CASE WHEN IsPaid = 0 THEN Amount ELSE 0 END AS ReceivableAmount
    FROM Installments
) AS Combined
GROUP BY FORMAT(PaymentMonth, 'yyyy-MM')
ORDER BY Month;


        ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}