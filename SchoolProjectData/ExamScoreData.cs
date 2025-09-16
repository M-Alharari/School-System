using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsExamScoreData
    {
        public static int AddNewExamScore(int enrollmentID, int subjectID, int examTypeID,
           int createdByUserID, DateTime createdAt)
        {
            int newScoreID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO ExamScores
                        (EnrollmentID, SubjectID, ExamTypeID,
                         CreatedByUserID, CreatedAt)
                    VALUES
                        (@EnrollmentID, @SubjectID, @ExamTypeID,
                         @CreatedByUserID, @CreatedAt);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                    cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);
                  
                    cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                    cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            newScoreID = Convert.ToInt32(result);
                    }
                    catch (Exception ex)
                    {
                        // عالج الخطأ حسب الحاجة
                    }
                }
            }

            return newScoreID;
        }

        public static bool UpdateExamScore(int scoreID, int enrollmentID, int subjectID, int examTypeID,
          int modifiedByUserID, DateTime modifiedAt)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE ExamScores SET
                        EnrollmentID = @EnrollmentID,
                        SubjectID = @SubjectID,
                        ExamTypeID = @ExamTypeID,
                       
                        ModifiedByUserID = @ModifiedByUserID,
                        ModifiedAt = @ModifiedAt
                    WHERE ScoreID = @ScoreID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreID", scoreID);
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                    cmd.Parameters.AddWithValue("@ExamTypeID", examTypeID);
                  
                    cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                    cmd.Parameters.AddWithValue("@ModifiedAt", modifiedAt);

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // سجل أو تعامل مع الخطأ
                    }
                }
            }

            return rowsAffected > 0;
        }

        public static bool DeleteExamScore(int scoreID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM ExamScores WHERE ScoreID = @ScoreID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreID", scoreID);

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // سجل أو تعامل مع الخطأ
                    }
                }
            }

            return rowsAffected > 0;
        }

        public static DataTable GetAllExamScores()
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT 
                    ES.ScoreID,
                    E.EnrollmentID,
                    S.SubjectName,
                    ET.ExamTypeName,
                   
                    U1.UserName AS CreatedBy,
                    ES.CreatedAt,
                    U2.UserName AS ModifiedBy,
                    ES.ModifiedAt
                FROM ExamScores ES
                INNER JOIN Enrollments E ON ES.EnrollmentID = E.EnrollmentID
                INNER JOIN Subjects S ON ES.SubjectID = S.SubjectID
                INNER JOIN ExamTypes ET ON ES.ExamTypeID = ET.ExamTypeID
                LEFT JOIN Users U1 ON ES.CreatedByUserID = U1.UserID
                LEFT JOIN Users U2 ON ES.ModifiedByUserID = U2.UserID
                ORDER BY ES.ScoreID DESC";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        dt.Load(reader);
                    }
                    catch (Exception ex)
                    {
                        // سجل أو تعامل مع الخطأ
                    }
                }
            }

            return dt;
        }
        public static DataTable GetScoresByEnrollmentID(int enrollmentID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT * FROM ExamScores
            WHERE EnrollmentID = @EnrollmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        // سجل الخطأ أو تعامل مع الاستثناء هنا
                    }
                }
            }

            return dt;
        }

        public static bool ExamScoreExists(int scoreID)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM ExamScores WHERE ScoreID = @ScoreID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreID", scoreID);

                    try
                    {
                        conn.Open();
                        exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                    catch (Exception ex)
                    {
                        // تعامل مع الخطأ
                    }
                }
            }

            return exists;
        }
    }
}
