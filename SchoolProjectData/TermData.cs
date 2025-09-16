using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsTermData
    {
        public static int AddNewTerm(string termName, DateTime startDate, DateTime endDate, bool isFinal, int createdBy)
        {
            string query = @"INSERT INTO Terms (TermName, StartDate, EndDate, IsFinal, CreatedByUserID) 
                     OUTPUT INSERTED.TermID
                     VALUES (@TermName, @StartDate, @EndDate, @IsFinal, @CreatedByUserID)";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TermName", termName);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@IsFinal", isFinal);
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdBy);

                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public static bool UpdateTermIsFinal(int termID, bool isFinal)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE Terms SET IsFinal = 1 WHERE TermID = @TermID", con))
            {
                cmd.Parameters.AddWithValue("@IsFinal", isFinal);
                cmd.Parameters.AddWithValue("@TermID", termID);

                con.Open();
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
        }
        public static bool IsFinalTerm(int termID)
        {
            bool isFinal = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT IsFinal FROM Terms WHERE TermID = @TermID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TermID", termID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    isFinal = Convert.ToBoolean(result);
            }

            return isFinal;
        }
        public static DataTable GetAllTerms()
        {
            string sql = "SELECT  TermID, TermName,  StartDate,    EndDate,  IsActive, IsFinal FROM Terms  ORDER BY StartDate; ";
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static bool Find(int termID,
           ref string termName,
           ref DateTime startDate,
           ref DateTime endDate,
           ref int createdByUserID,
           ref DateTime? createdAt,
           ref int? modifiedByUserID,
           ref DateTime? modifiedAt)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"SELECT * FROM Terms WHERE TermID = @TermID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TermID", termID);
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            termName = dr["TermName"].ToString();
                            startDate = Convert.ToDateTime(dr["StartDate"]);
                            endDate = Convert.ToDateTime(dr["EndDate"]);
                            createdByUserID = Convert.ToInt32(dr["CreatedByUserID"]);
                            createdAt = dr["CreatedAt"] as DateTime?;
                            modifiedByUserID = dr["ModifiedByUserID"] as int?;
                            modifiedAt = dr["ModifiedAt"] as DateTime?;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static int InsertTerm(string termName, DateTime startDate, DateTime endDate, int createdByUserID)
        {
            int newID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            INSERT INTO Terms (TermName, StartDate, EndDate, CreatedByUserID, CreatedAt)
            OUTPUT INSERTED.TermID
            VALUES (@TermName, @StartDate, @EndDate, @CreatedByUserID, GETDATE());";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TermName", termName);
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                    conn.Open();
                    newID = (int)cmd.ExecuteScalar();
                }
            }

            return newID;
        }
        // Get the next term after a given TermID
        public static DataTable GetNextTermInfo(int currentTermID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT TOP 1 *
            FROM Terms
            WHERE TermID > @CurrentTermID
            ORDER BY TermID ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@CurrentTermID", currentTermID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public static bool UpdateTerm(int termId, string termName, DateTime startDate, DateTime endDate, int modifiedBy)
        {
            string sql = @"UPDATE Terms 
                           SET TermName=@TermName, StartDate=@StartDate, EndDate=@EndDate, 
                               ModifiedByUserID=@ModifiedByUserID, ModifiedAt=GETDATE()
                           WHERE TermID=@TermID";
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@TermID", termId);
                cmd.Parameters.AddWithValue("@TermName", termName);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedBy);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeleteTerm(int termId)
        {
            string sql = "DELETE FROM Terms WHERE TermID=@TermID";
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@TermID", termId);
                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
