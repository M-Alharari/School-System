using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsScoreDetailsPerTermData
    { // Returns scores for a specific Enrollment and Term
        public static DataTable GetScoresByEnrollmentAndTerm(int enrollmentID, int termID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT ScoreDetailID, EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy, ModifiedBy
                    FROM ScoreDetailsPerTerm
                    WHERE EnrollmentID = @EnrollmentID AND TermID = @TermID
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
        public static DataRow LoadByEnrollmentTermAndSubject(int enrollmentID, int termID, int subjectID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM ScoresDetails WHERE EnrollmentID=@enrollmentID AND TermID=@termID AND SubjectID=@subjectID",
                conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@enrollmentID", enrollmentID);
                da.SelectCommand.Parameters.AddWithValue("@termID", termID);
                da.SelectCommand.Parameters.AddWithValue("@subjectID", subjectID);

                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }
        public static int AddScore(int enrollmentID, int subjectID, int termID, decimal testScore, decimal examScore, int createdBy)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"INSERT INTO ScoreDetailsPerTerm (EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy, CreatedAt)
                               VALUES (@EnrollmentID, @SubjectID, @TermID, @TestScore, @ExamScore, @CreatedBy, GETDATE());
                               SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                    cmd.Parameters.AddWithValue("@TermID", termID);
                    cmd.Parameters.AddWithValue("@TestScore", testScore);
                    cmd.Parameters.AddWithValue("@ExamScore", examScore);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
        }

        public static DataTable GetTotalScoresByEnrollment(int termID)
        {
            string query = @"
SELECT 
    s.EnrollmentID,
    st.StudentID,
    s.GradeID,
    s.ClassID,
    (p.FirstName + ' ' + COALESCE(p.SecondName, '') + ' ' + COALESCE(p.ThirdName, '') + ' ' + p.LastName) AS FullName,
    c.GradeName,
    cl.ClassName,
    SUM(sd.TestScore + sd.ExamScore) AS TotalScore,
    CAST(SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) AS DECIMAL(5,2)) AS Percentage,
    CASE 
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 90 THEN 'A+'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 85 THEN 'A'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 80 THEN 'A-'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 75 THEN 'B+'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 70 THEN 'B'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 65 THEN 'B-'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 60 THEN 'C+'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 55 THEN 'C'
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 50 THEN 'C-'
        ELSE 'F'
    END AS PredictedLetterGrade,
    CASE 
        WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 50 THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsPredictedPassed
FROM ScoreDetailsperterm sd
INNER JOIN Enrollments s ON sd.EnrollmentID = s.EnrollmentID
INNER JOIN Students st ON s.StudentID = st.StudentID
INNER JOIN People p ON st.PersonID = p.PersonID
INNER JOIN Classes cl ON s.ClassID = cl.ClassID
INNER JOIN Grades c ON s.GradeID = c.GradeID
WHERE sd.TermID = @TermID
  AND s.IsActive = 1
GROUP BY 
    s.EnrollmentID,
    st.StudentID,
    s.GradeID,
    s.ClassID,
    p.FirstName,
    p.SecondName,
    p.ThirdName,
    p.LastName,
    c.GradeName,
    cl.ClassName
ORDER BY c.GradeName, cl.ClassName, FullName;


    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@TermID", termID);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static bool UpdateScore(int scoreDetailID, decimal testScore, decimal examScore, int modifiedBy)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"UPDATE ScoreDetailsPerTerm
                               SET TestScore = @TestScore,
                                   ExamScore = @ExamScore,
                                   ModifiedBy = @ModifiedBy,
                                   ModifiedAt = GETDATE()
                               WHERE ScoreDetailID = @ScoreDetailID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);
                    cmd.Parameters.AddWithValue("@TestScore", testScore);
                    cmd.Parameters.AddWithValue("@ExamScore", examScore);
                    cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool DeleteScore(int scoreDetailID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "DELETE FROM ScoreDetailsPerTerm WHERE ScoreDetailID = @ScoreDetailID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool GetScoreByID(int scoreDetailID, ref int enrollmentID, ref int subjectID, ref int termID, ref decimal testScore, ref decimal examScore)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT * FROM ScoreDetailsPerTerm WHERE ScoreDetailID = @ScoreDetailID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ScoreDetailID", scoreDetailID);
                    conn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            enrollmentID = Convert.ToInt32(dr["EnrollmentID"]);
                            subjectID = Convert.ToInt32(dr["SubjectID"]);
                            termID = Convert.ToInt32(dr["TermID"]);
                            testScore = Convert.ToDecimal(dr["TestScore"]);
                            examScore = Convert.ToDecimal(dr["ExamScore"]);
                            return true;
                        }
                        return false;
                    }
                }
            }
        }

        public static DataTable GetAllScores()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT * FROM ScoreDetailsPerTerm";
                using (SqlDataAdapter da = new SqlDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable LoadByEnrollmentAndTerm(int enrollmentID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM ScoreDetailsPerTerm 
                  WHERE EnrollmentID = @EnrollmentID AND TermID = @TermID", conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        public static DataTable GetScoresByEnrollment(int enrollmentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
  INSERT INTO Graduation (EnrollmentID, TotalTestScore, TotalExamScore, MaxTotalScore, Percentage)
SELECT 
    EnrollmentID,
    SUM(TestScore) AS TotalTestScore,
    SUM(ExamScore) AS TotalExamScore,
    SUM(TotalScore) AS MaxTotalScore,
    CAST(SUM(TestScore + ExamScore) * 100.0 / SUM(TotalScore) AS DECIMAL(5,2)) AS Percentage
FROM ScoreDetailsPerTerm
GROUP BY EnrollmentID;

";



                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
       


    }
}
