using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsScoreData
    {
        public static int AddScore(int enrollmentID, int subjectID, int examTypeID, int termID,
            int createdByUserID)
        {
            string query = @"
                INSERT INTO Scores (EnrollmentID, SubjectID, ExamTypeID, TermID, CreatedByUserID, CreatedAt)
                OUTPUT INSERTED.ScoreID
                VALUES (@EnrollmentID, @SubjectID, @ExamTypeID, @TermID,  @CreatedByUserID, GETDATE())";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);
                cmd.Parameters.AddWithValue("@TermID", termID);
             
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding score: " + ex.Message);
                }
            }
        }

        public static bool UpdateScore(int scoreID, double rawScore,  
            int? modifiedByUserID)
        {
            string query = @"
                UPDATE Scores
                SET  
                    ModifiedByUserID = @ModifiedByUserID,
                    ModifiedAt = GETDATE()
                WHERE ScoreID = @ScoreID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@RawScore", rawScore);
      
                cmd.Parameters.AddWithValue("@ModifiedByUserID", (object)modifiedByUserID ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ScoreID", scoreID);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating score: " + ex.Message);
                }
            }
        }

        public static bool DeleteScore(int scoreID)
        {
            string query = "DELETE FROM Scores WHERE ScoreID = @ScoreID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ScoreID", scoreID);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting score: " + ex.Message);
                }
            }
        }

        public static DataTable GetScoresByEnrollment(int enrollmentID)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT * FROM Scores WHERE EnrollmentID = @EnrollmentID ORDER BY SubjectID, ExamTypeID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error fetching scores by enrollment: " + ex.Message);
                    }
                }
            }
            return dt;
        }

        public static DataRow GetScoreByID(int scoreID)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Scores WHERE ScoreID = @ScoreID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@ScoreID", scoreID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                        if (dt.Rows.Count == 1)
                            return dt.Rows[0];
                        return null;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error fetching score by ID: " + ex.Message);
                    }
                }
            }
        }
    }
}
