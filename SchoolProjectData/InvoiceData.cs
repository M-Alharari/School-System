using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsInvoiceData
    {
        public static int AddNew(int enrollmentID, decimal totalAmount, DateTime dueDate, bool isFullPayment, int createdByUserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Invoices (EnrollmentID, InvoiceDate, TotalAmount, Status, DueDate, IsFullPayment, CreatedBy, CreatedDate)
                OUTPUT INSERTED.InvoiceID
                VALUES (@EnrollmentID, GETDATE(), @TotalAmount, 'Pending', @DueDate, @IsFullPayment, @CreatedBy, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@IsFullPayment", isFullPayment);
                cmd.Parameters.AddWithValue("@CreatedBy", createdByUserID);

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public static bool UpdateStatus(int invoiceID, string status)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE Invoices SET Status=@Status, ModifiedDate=GETDATE() WHERE InvoiceID=@InvoiceID", con))
            {
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceID);
                cmd.Parameters.AddWithValue("@Status", status);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static DataRow FindByID(int invoiceID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Invoices WHERE InvoiceID=@InvoiceID", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@InvoiceID", invoiceID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public static DataTable GetAll()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Invoices", con))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
