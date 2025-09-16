using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsTuitionPaymentData
    {
        public static DataTable GetPaymentsByEnrollmentID(int enrollmentID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT 
                TuitionFeeID,
                EnrollmentID,
                PaymentMode,
                InstallmentFrequencyID,
                TotalFees,
                PaidAmount,
                PaymentDate,
                IsFullyPaid,
                CreatedByUserID,
                CreatedDate,
                ModifiedByUserID,
                ModifiedDate
            FROM TuitionPayments
            WHERE EnrollmentID = @EnrollmentID;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error in GetPaymentsByEnrollmentID", ex);
                }
            }

            return dt;
        }


        public static DataRow FindByEnrollmentID2(int enrollmentID)
        {
            string query = @" SELECT TOP (1)
            tp.TuitionFeeID,
            tp.EnrollmentID,
            tp.PaymentMode,
            tp.InstallmentFrequencyID,
            tp.TotalFees,
            tp.PaidAmount,
            tp.IsFullyPaid,
            tp.PaymentDate,
            
            tp.CreatedDate,
           
            tp.ModifiedDate,
            CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName) AS FullName
        FROM TuitionPayments tp
        INNER JOIN Enrollments e ON tp.EnrollmentID = e.EnrollmentID
        INNER JOIN Students s ON e.StudentID = s.StudentID
        INNER JOIN People p ON s.PersonID = p.PersonID
        WHERE tp.EnrollmentID = @EnrollmentID
        ORDER BY tp.CreatedDate DESC;";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                }
            }

            return null;
        }
        public static DataRow FindByEnrollmentID(int enrollmentID)
        {
            string query = @"SELECT TOP 1 * 
                     FROM TuitionPayments 
                     WHERE EnrollmentID = @EnrollmentID 
                     ORDER BY CreatedDate DESC";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                }
            }

            return null;
        }

        public static int AddNew(int EnrollmentID, int paymentMode, int installmentFreq, decimal totalFees,
            decimal paidAmount, int createdByUserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO TuitionPayments
                (EnrollmentID, PaymentMode, InstallmentFrequencyID, TotalFees, PaidAmount,  CreatedByUserID, CreatedDate)
                OUTPUT INSERTED.TuitionFeeID                                               
                VALUES                                                                     
                (@EnrollmentID, @PaymentMode, @InstallmentFreq, @TotalFees, @PaidAmount,  @CreatedByUserID, GETDATE())", con))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", EnrollmentID);
                cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                cmd.Parameters.AddWithValue("@InstallmentFreq", installmentFreq);
                cmd.Parameters.AddWithValue("@TotalFees", totalFees);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public static bool Update(int tuitionFeeID, int paymentMode, int installmentFreq, decimal totalFees,
            decimal paidAmount,  int? modifiedByUserID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE TuitionPayments
                SET PaymentMode=@PaymentMode,
                    InstallmentFrequencyID=@InstallmentFreq,
                    TotalFees=@TotalFees,
                    PaidAmount=@PaidAmount,
                   
                    ModifiedByUserID=@ModifiedByUserID,
                    ModifiedDate=GETDATE()
                WHERE TuitionFeeID=@TuitionFeeID", con))
            {
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                cmd.Parameters.AddWithValue("@PaymentMode", paymentMode);
                cmd.Parameters.AddWithValue("@InstallmentFreq", installmentFreq);
                cmd.Parameters.AddWithValue("@TotalFees", totalFees);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                
                cmd.Parameters.AddWithValue("@ModifiedByUserID", (object)modifiedByUserID ?? DBNull.Value);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static DataRow FindByStudentID(int studentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(@"SELECT TOP 1 * FROM TuitionPayments WHERE StudentID=@StudentID", conn))
            {
                DataTable dt = new DataTable();
                da.SelectCommand.Parameters.AddWithValue("@StudentID", studentID);
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public static DataRow FindByTuitionFeeID(int tuitionFeeID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TuitionPayments WHERE TuitionFeeID=@TuitionFeeID", conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }

        public static bool UpdateTuitionPaymentStatus(int tuitionFeeID, decimal paidAmount, bool isFullyPaid)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE TuitionPayments
                SET PaidAmount=@PaidAmount,
                    IsFullyPaid=@IsFullyPaid,
                    ModifiedAt=GETDATE()
                WHERE TuitionFeeID=@TuitionFeeID", conn))
            {
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                cmd.Parameters.AddWithValue("@PaidAmount", paidAmount);
                cmd.Parameters.AddWithValue("@IsFullyPaid", isFullyPaid ? 1 : 0);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static DataTable GetAllTuitionPayments()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM TuitionPayments", conn))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static (int StudentID, bool Success) GetStudentIDByTuitionFeeID(int tuitionFeeID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT StudentID FROM TuitionFees WHERE TuitionFeeID = @TuitionFeeID";
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                        return (Convert.ToInt32(result), true);
                }
            }
            return (0, false);
        }
        public static DataRow GetStudentByID(int studentID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT s.StudentID, p.FirstName, p.LastName " +
                             "FROM Students s " +
                             "INNER JOIN Persons p ON s.PersonID = p.PersonID " +
                             "WHERE s.StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                            return dt.Rows[0];
                    }
                }
            }
            return null;
        }
        public static DataTable GetPaymentDetailsByPaymentID(int paymentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT p.PaymentID, p.TuitionFeeID, p.PaymentDate, pt.PaymentTypeName AS PaymentType,
                   p.AmountPaid,
                   (t.TotalAmount - ISNULL(SUM(p2.AmountPaid),0)) AS RemainingBalance,
                   pe.FirstName + ' ' + pe.SecondName + ' ' + pe.ThirdName + ' ' + pe.LastName AS FullName
            FROM TuitionPayments p
            INNER JOIN TuitionFees t ON p.TuitionFeeID = t.TuitionFeeID
            INNER JOIN PaymentTypes pt ON p.PaymentTypeID = pt.PaymentTypeID
            INNER JOIN Students s ON t.StudentID = s.StudentID
            INNER JOIN People pe ON s.PersonID = pe.PersonID
            LEFT JOIN TuitionPayments p2 ON p2.TuitionFeeID = t.TuitionFeeID AND p2.PaymentID <= p.PaymentID
            WHERE p.PaymentID = @PaymentID
            GROUP BY p.PaymentID, p.TuitionFeeID, p.PaymentDate, pt.PaymentTypeName, p.AmountPaid, t.TotalAmount,
                     pe.FirstName, pe.SecondName, pe.ThirdName, pe.LastName";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaymentID", paymentID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }
        public static DataTable GetPaymentsByTuitionFeeID(int tuitionFeeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                   SELECT
    tp.TuitionFeeID,
    tp.PaymentMode,
    tp.InstallmentFrequencyID,
    tp.TotalFees,
    tp.PaidAmount,
    tp.FirstPaidAmount,
    tp.LastPaidAmount,
    tp.FirstPaymentDate,
    tp.LastPaymentDate,
    tp.CreatedDate,
    pe.FirstName + ' ' + pe.SecondName + ' ' + pe.ThirdName + ' ' + pe.LastName AS FullName
FROM TuitionPayments tp
INNER JOIN Students s ON tp.StudentID = s.StudentID
INNER JOIN People pe ON s.PersonID = pe.PersonID
WHERE tp.TuitionFeeID = @TuitionFeeID;
";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
            }

            return dt;
        }
    }
}
