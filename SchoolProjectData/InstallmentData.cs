using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsInstallmentData
    {
        public static int GetFirstUnpaidInstallmentID(int tuitionFeeID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1 InstallmentID 
        FROM Installments
        WHERE TuitionFeeID = @TuitionFeeID AND IsPaid = 0
        ORDER BY InstallmentNumber ASC", conn))
            {
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
        public static decimal GetInstallmentAmount(int installmentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT Amount FROM Installments WHERE InstallmentID = @InstallmentID", conn))
            {
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }

        public static bool GetInstallmentByID(int installmentID,
            ref int tuitionFeeID, ref int installmentNumber, ref decimal amount,
            ref DateTime dueDate, ref bool isPaid, ref DateTime? paidDate,
            ref int createdByUserID, ref DateTime createdAt,
            ref int? modifiedByUserID, ref DateTime? modifiedAt)
        {
            bool isFound = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Installments WHERE InstallmentID = @InstallmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    tuitionFeeID = (int)reader["TuitionFeeID"];
                    installmentNumber = (int)reader["InstallmentNumber"];
                    amount = (decimal)reader["Amount"];
                    dueDate = (DateTime)reader["DueDate"];
                    isPaid = (bool)reader["IsPaid"];
                    paidDate = reader["PaidDate"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["PaidDate"];
                    createdByUserID = (int)reader["CreatedByUserID"];
                    createdAt = (DateTime)reader["CreatedAt"];
                    modifiedByUserID = reader["ModifiedByUserID"] == DBNull.Value ? (int?)null : (int)reader["ModifiedByUserID"];
                    modifiedAt = reader["ModifiedAt"] == DBNull.Value ? (DateTime?)null : (DateTime)reader["ModifiedAt"];
                }

                reader.Close();
            }
            return isFound;
        }

        public static int AddNewInstallment(int tuitionFeeID, int installmentNumber, decimal amount,
            DateTime dueDate, int createdByUserID)
        {
            int newID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Installments
                                (TuitionFeeID, InstallmentNumber, Amount, DueDate, IsPaid, CreatedByUserID, CreatedAt)
                                VALUES (@TuitionFeeID, @InstallmentNumber, @Amount, @DueDate, 0, @CreatedByUserID, GETDATE());
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);
                cmd.Parameters.AddWithValue("@InstallmentNumber", installmentNumber);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null) newID = Convert.ToInt32(result);
            }

            return newID;
        }

        public static bool UpdateInstallment(int installmentID, decimal amount, DateTime dueDate,
            bool isPaid, DateTime? paidDate, int modifiedByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Installments
                                SET Amount=@Amount,
                                    DueDate=@DueDate,
                                    IsPaid=@IsPaid,
                                    PaidDate=@PaidDate,
                                    ModifiedByUserID=@ModifiedByUserID,
                                    ModifiedAt=GETDATE()
                                WHERE InstallmentID=@InstallmentID";

                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@DueDate", dueDate);
                cmd.Parameters.AddWithValue("@IsPaid", isPaid);
                cmd.Parameters.AddWithValue("@PaidDate", (object)paidDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            return rowsAffected > 0;
        }

        public static bool DeleteInstallment(int installmentID)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Installments WHERE InstallmentID=@InstallmentID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }

        public static DataTable GetAllInstallmentsByTuitionFee(int tuitionFeeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Installments WHERE TuitionFeeID=@TuitionFeeID ORDER BY InstallmentNumber";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                dt.Load(reader);
            }
            return dt;
        }
        public static DataTable GetInstallmentsWithStudentNamesByTuitionFee(int tuitionFeeID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
      SELECT 
    i.InstallmentID,
    i.InstallmentNumber,
    i.DueDate,
    i.Amount,
    i.PaidDate,
    i.IsPaid,
    tp.EnrollmentID,
    pe.FirstName,
    pe.SecondName,
    pe.ThirdName,
    pe.LastName,
    (pe.FirstName + ' ' + pe.SecondName + ' ' + pe.ThirdName + ' ' + pe.LastName) AS FullName
FROM Installments i
INNER JOIN TuitionPayments tp ON i.TuitionFeeID = tp.TuitionFeeID
INNER JOIN Enrollments e ON tp.EnrollmentID = e.EnrollmentID
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People pe ON s.PersonID = pe.PersonID
WHERE i.TuitionFeeID = @TuitionFeeID
ORDER BY i.InstallmentNumber;








        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }
        public static void MarkInstallmentAsPaid(int installmentID, int userID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
            UPDATE Installments
            SET IsPaid = 1,
                PaidDate = GETDATE(),
                ModifiedByUserID = @UserID,
                ModifiedAt = GETDATE()
            WHERE InstallmentID = @InstallmentID", conn))
            {
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public static bool MarkInstallmentAsPaid(int installmentID, DateTime paidDate, int modifiedByUserID)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Installments
                                SET IsPaid=1, PaidDate=@PaidDate, 
                                    ModifiedByUserID=@ModifiedByUserID, ModifiedAt=GETDATE()
                                WHERE InstallmentID=@InstallmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PaidDate", paidDate);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }
        public static bool UpdatePaidDateAndIsPaid(
    int installmentID,
    DateTime? paidDate,
    bool isPaid,
    int? modifiedByUserID,
    DateTime? modifiedAt)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"UPDATE Installments 
          SET PaidDate = @PaidDate, IsPaid = @IsPaid, ModifiedByUserID = @ModifiedByUserID, ModifiedAt = @ModifiedAt 
          WHERE InstallmentID = @InstallmentID", conn))
            {
                cmd.Parameters.AddWithValue("@PaidDate", (object)paidDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPaid", isPaid);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", (object)modifiedByUserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedAt", (object)modifiedAt ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@InstallmentID", installmentID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static DataTable GetAllInstallments(int? tuitionFeeID = null)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
            SELECT 
                InstallmentID,
                TuitionFeeID,
                InstallmentNumber,
                Amount,
                DueDate,
                PaidDate,
                CreatedByUserID,
                CreatedAt,
                ModifiedByUserID,
                ModifiedAt
            FROM Installments";

                if (tuitionFeeID.HasValue)
                {
                    sql += " WHERE TuitionFeeID = @TuitionFeeID";
                }

                sql += " ORDER BY InstallmentNumber ASC";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (tuitionFeeID.HasValue)
                        cmd.Parameters.AddWithValue("@TuitionFeeID", tuitionFeeID.Value);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

    }
}
