using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsReceiptsData
    {
        // Add a new receipt
        public static int AddReceipt(
           string receiptNumber,
           int tuitionFeeID,
           int? installmentID,
           decimal amount,
           DateTime paymentDate,
           string notes,
           int createdByUserID,
           int modifiedByUserID,
           DateTime modifiedDate)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        INSERT INTO Receipts
        (ReceiptNumber, TuitionFeeID, InstallmentID, Amount, PaymentDate, CreatedByUserID, CreatedDate, ModifiedByUserID, ModifiedDate)
        VALUES
        (@ReceiptNumber, @TuitionFeeID, @InstallmentID, @Amount, @PaymentDate,   @CreatedByUserID, GETDATE(), @ModifiedByUserID, @ModifiedDate);
        SELECT SCOPE_IDENTITY();", con))
            {
                cmd.Parameters.AddWithValue("@ReceiptNumber", receiptNumber);
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                cmd.Parameters.AddWithValue("@ModifiedDate", modifiedDate );

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Update a receipt
        public static void UpdateReceipt(
           int receiptID,
           string receiptNumber,
           int tuitionFeeID,
           int installmentID,
           decimal amount,
           DateTime paymentDate,
           string notes,
           int modifiedByUserID,
           DateTime modifiedDate)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE Receipts
        SET ReceiptNumber=@ReceiptNumber,
            TuitionFeeID=@TuitionFeeID,
            InstallmentID=@InstallmentID,
            Amount=@Amount,
            PaymentDate=@PaymentDate,
            Notes=@Notes,
            ModifiedByUserID=@ModifiedByUserID,
            ModifiedDate=@ModifiedDate
        WHERE ReceiptID=@ReceiptID", con))
            {
                cmd.Parameters.AddWithValue("@ReceiptID", receiptID);
                cmd.Parameters.AddWithValue("@ReceiptNumber", receiptNumber);
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                cmd.Parameters.AddWithValue("@InstallmentID", (object)installmentID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@PaymentDate", paymentDate);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                cmd.Parameters.AddWithValue("@ModifiedDate", modifiedDate);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Delete a receipt
        public static void DeleteReceipt(int receiptID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Receipts WHERE ReceiptID=@ReceiptID", con))
            {
                cmd.Parameters.AddWithValue("@ReceiptID", receiptID);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Get a single receipt
        public static DataRow GetReceiptByID(int receiptID)
        {
            using (SqlConnection con  = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Receipts WHERE ReceiptID=@ReceiptID", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@ReceiptID", receiptID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count == 0) return null;
                return dt.Rows[0];
            }
        }

        // Get all receipts
        public static DataTable GetAllReceipts()
        {
            using (SqlConnection con  = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Receipts ORDER BY PaymentDate DESC", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }    // Get the last receipt number for a given year
        public static string GetLastReceiptNumberForYear(int year)
        {
            string prefix = $"RCPT-{year}-";
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 ReceiptNumber
                FROM Receipts
                WHERE ReceiptNumber LIKE @Prefix + '%'
                ORDER BY ReceiptID DESC", con))
            {
                cmd.Parameters.AddWithValue("@Prefix", prefix);
                con.Open();
                object result = cmd.ExecuteScalar();
                return result?.ToString();
            }
        }

        // Other methods: Update, Delete, GetByID, GetAll...
      

        public static DataTable GetReceiptsByTuitionFee(int tuitionFeeID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
    r.*,
    p.TotalFees,
    p.PaidAmount,
    p.PaymentMode,
    -- Build the full name from the People table
    (pe.FirstName + ' ' + pe.SecondName + ' ' + pe.ThirdName + ' ' + pe.LastName) AS FullName
FROM Receipts r
INNER JOIN TuitionPayments p ON r.TuitionFeeID = p.TuitionFeeID
INNER JOIN Students s ON r.StudentID = s.StudentID
INNER JOIN People pe ON s.PersonID = pe.PersonID
WHERE r.TuitionFeeID = @TuitionFeeID


"; // or by date

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
