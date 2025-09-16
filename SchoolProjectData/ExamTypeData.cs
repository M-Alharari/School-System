using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsExamTypeData
    {
        

        // Get all active exam types with weight, max score, and term name
        public static DataTable GetAllExamTypes()
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 
                    et.ExamTypeID, 
                    et.Title AS ExamTypeName, 
                    et.Weight, 
                    et.MaxScore,
                    t.TermName
                FROM ExamTypes et
                INNER JOIN Terms t ON et.TermID = t.TermID
                WHERE et.IsActive = 1
                ORDER BY et.Title";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                try
                {
                    conn.Open();
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching exam types: " + ex.Message);
                }
            }

            return dt;
        }
        public static float GetMaxScore(int examTypeID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT MaxScore FROM ExamTypes WHERE ExamTypeID = @ExamTypeID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result == null || result == DBNull.Value) return 1f; // default safe
                    float max = Convert.ToSingle(result);
                    return max > 0 ? max : 1f; // avoid zero
                }
            }
        }
        // Get exam type by ID
        public static DataRow GetExamTypeByID(int examTypeID)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT 
    et.ExamTypeID, 
    et.Title AS ExamTypeName, 
    et.Weight,
    et.TermID
FROM ExamTypes et
WHERE et.ExamTypeID = @ExamTypeID AND et.IsActive = 1
";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                        return dt.Rows.Count == 1 ? dt.Rows[0] : null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error fetching exam type by ID: " + ex.Message);
                    }
                }
            }
        }

        // Check existence by ID
        public static bool ExamTypeExists(int examTypeID)
        {
            string query = "SELECT COUNT(1) FROM ExamTypes WHERE ExamTypeID = @ExamTypeID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking exam type existence: " + ex.Message);
                }
            }
        }

        // Check existence by Name
        public static bool ExamTypeExists(string examTypeName)
        {
            string query = "SELECT COUNT(1) FROM ExamTypes WHERE Title = @ExamTypeName";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeName", examTypeName);

                try
                {
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking exam type existence by name: " + ex.Message);
                }
            }
        }

        // Add new exam type
        public static int AddExamType(string examTypeName, int termID, double weight, float maxScore)
        {
            string query = @"
                INSERT INTO ExamTypes (Title, TermID, Weight, MaxScore, IsActive)
                OUTPUT INSERTED.ExamTypeID
                VALUES (@ExamTypeName, @TermID, @Weight, @MaxScore, 1)";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeName", examTypeName);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@Weight", weight);
                cmd.Parameters.AddWithValue("@MaxScore", maxScore);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding exam type: " + ex.Message);
                }
            }
        }

        // Update existing exam type
        public static bool UpdateExamType(int examTypeID, string examTypeName, int termID, double weight, float maxScore, bool isActive)
        {
            string query = @"
                UPDATE ExamTypes
                SET Title = @ExamTypeName, TermID = @TermID, Weight = @Weight, MaxScore = @MaxScore, IsActive = @IsActive
                WHERE ExamTypeID = @ExamTypeID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);
                cmd.Parameters.AddWithValue("@ExamTypeName", examTypeName);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@Weight", weight);
                cmd.Parameters.AddWithValue("@MaxScore", maxScore);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating exam type: " + ex.Message);
                }
            }
        }

        // Soft-delete (disable) exam type
        public static bool DeleteExamType(int examTypeID)
        {
            string query = "UPDATE ExamTypes SET IsActive = 0 WHERE ExamTypeID = @ExamTypeID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting (disabling) exam type: " + ex.Message);
                }
            }
        }
    }
}
