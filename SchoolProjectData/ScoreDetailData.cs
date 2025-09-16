using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsScoreDetailsData
    {
        public static bool GetScoreDetailByID(int scoreDetailID,
            ref int studentID, ref int subjectID, ref int termID,
            ref decimal testScore, ref decimal examScore,
            ref int createdBy, ref DateTime createdAt,
            ref int modifiedBy, ref DateTime modifiedAt)
        {
            bool isFound = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT StudentID, SubjectID, TermID, TestScore, ExamScore, 
                                        CreatedBy, CreatedAt, ModifiedBy, ModifiedAt
                                 FROM ScoreDetails
                                 WHERE ScoreDetailID = @ScoreDetailID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;

                    studentID = (int)reader["StudentID"];
                    subjectID = (int)reader["SubjectID"];
                    termID = (int)reader["TermID"];
                    testScore = (decimal)reader["TestScore"];
                    examScore = (decimal)reader["ExamScore"];
                    createdBy = (int)reader["CreatedBy"];
                    createdAt = (DateTime)reader["CreatedAt"];
                    modifiedBy = reader["ModifiedBy"] != DBNull.Value ? (int)reader["ModifiedBy"] : 0;
                    modifiedAt = reader["ModifiedAt"] != DBNull.Value ? (DateTime)reader["ModifiedAt"] : DateTime.MinValue;
                }

                reader.Close();
            }

            return isFound;
        }

        public static int AddNewScoreDetail(int studentID, int subjectID, int termID,
            decimal testScore, decimal examScore, int createdBy)
        {
            int newID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO ScoreDetails 
                                (StudentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy, CreatedAt) 
                                VALUES (@StudentID, @SubjectID, @TermID, @TestScore, @ExamScore, @CreatedBy, GETDATE());
                                SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@TestScore", testScore);
                cmd.Parameters.AddWithValue("@ExamScore", examScore);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                    newID = Convert.ToInt32(result);
            }

            return newID;
        }

        public static bool UpdateScoreDetail(int scoreDetailID,
            int studentID, int subjectID, int termID,
            decimal testScore, decimal examScore, int modifiedBy)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE ScoreDetails 
                                SET StudentID = @StudentID,
                                    SubjectID = @SubjectID,
                                    TermID = @TermID,
                                    TestScore = @TestScore,
                                    ExamScore = @ExamScore,
                                    ModifiedBy = @ModifiedBy,
                                    ModifiedAt = GETDATE()
                                WHERE ScoreDetailID = @ScoreDetailID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@TestScore", testScore);
                cmd.Parameters.AddWithValue("@ExamScore", examScore);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteScoreDetail(int scoreDetailID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM ScoreDetails WHERE ScoreDetailID = @ScoreDetailID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllScoreDetails()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM ScoreDetails";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }
    }
}
