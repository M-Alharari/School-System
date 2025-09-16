using SchoolProjectData;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProject.Data
{
    public static class clsScoresDetailsData
    {
        // Add new score
        public static bool GetScoresDetailsByID(int scoresDetailsID,
         ref int enrollmentID, ref int subjectID, ref int termID,
         ref decimal testScore, ref decimal examScore)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT EnrollmentID, SubjectID, TermID, TestScore, ExamScore
                                 FROM ScoresDetails
                                 WHERE ScoresDetailsID = @ScoresDetailsID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ScoresDetailsID", scoresDetailsID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    isFound = true;
                    enrollmentID = (int)reader["EnrollmentID"];
                    subjectID = (int)reader["SubjectID"];
                    termID = (int)reader["TermID"];
                    testScore = reader["TestScore"] != DBNull.Value ? Convert.ToDecimal(reader["TestScore"]) : 0;
                    examScore = reader["ExamScore"] != DBNull.Value ? Convert.ToDecimal(reader["ExamScore"]) : 0;
                }

                reader.Close();
            }

            return isFound;
        }

        public static bool SaveScore(int enrollmentID, int subjectID, int termID,
            decimal? testScore, decimal? examScore, string currentUser)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // If record exists, update; otherwise insert
                cmd.CommandText = @"
IF EXISTS (SELECT 1 FROM ScoresDetails 
           WHERE EnrollmentID=@enrollmentID 
             AND SubjectID=@subjectID 
             AND TermID=@termID)
BEGIN
    UPDATE ScoresDetails
    SET TestScore=@testScore,
        ExamScore=@examScore,
        ModifiedBy=@user,
        ModifiedDate=GETDATE()
    WHERE EnrollmentID=@enrollmentID 
      AND SubjectID=@subjectID 
      AND TermID=@termID
END
ELSE
BEGIN
    INSERT INTO ScoresDetails (EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy, CreatedDate)
    VALUES (@enrollmentID, @subjectID, @termID, @testScore, @examScore, @user, GETDATE())
END";

                cmd.Parameters.AddWithValue("@enrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@subjectID", subjectID);
                cmd.Parameters.AddWithValue("@termID", termID);
                cmd.Parameters.AddWithValue("@testScore", (object)testScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@examScore", (object)examScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@user", currentUser);

                return cmd.ExecuteNonQuery() > 0;
            }
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
  
        public static int AddScore(int enrollmentID, int subjectID, int termID, decimal? testScore, decimal? examScore, string createdBy)
        {
            if (testScore.HasValue) testScore = Math.Min(testScore.Value, 30m);
            if (examScore.HasValue) examScore = Math.Min(examScore.Value, 70m);

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"INSERT INTO ScoresDetails 
          (EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy, CreatedDate) 
          OUTPUT INSERTED.ScoreID
          VALUES (@EnrollmentID, @SubjectID, @TermID, @TestScore, @ExamScore, @CreatedBy , GETDATE())", conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@TestScore", (object)testScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExamScore", (object)examScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                conn.Open();
                return (int)cmd.ExecuteScalar();
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
FROM ScoresDetails
GROUP BY EnrollmentID;

";



                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetTotalScoresByEnrollment(int termID)
        {
            string query = @"
SELECT 
    s.EnrollmentID,
    p.PersonID,
    st.StudentID,
    (p.FirstName + ' ' 
        + ISNULL(p.SecondName, '') + ' ' 
        + ISNULL(p.ThirdName, '') + ' ' 
        + p.LastName) AS FullName,
    c.GradeName,
    cl.ClassName,
    SUM(sd.TestScore) AS TotalScore,
    CAST((SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) AS DECIMAL(5,2)) AS Percentage,
    CASE 
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 90 THEN 'A+'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 85 THEN 'A'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 80 THEN 'A-'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 75 THEN 'B+'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 70 THEN 'B'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 65 THEN 'B-'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 60 THEN 'C+'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 55 THEN 'C'
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 50 THEN 'C-'
        ELSE 'F'
    END AS PredictedLetterGrade,
    CASE 
        WHEN (SUM(sd.TestScore) + SUM(sd.ExamScore)) * 100.0 / NULLIF(COUNT(*) * 100, 0) >= 50 THEN CAST(1 AS BIT)
        ELSE CAST(0 AS BIT)
    END AS IsPredictedPassed
FROM ScoreDetailsperterm sd
INNER JOIN Enrollments s ON sd.EnrollmentID = s.EnrollmentID
INNER JOIN Students st ON s.StudentID = st.StudentID
INNER JOIN People p ON st.PersonID = p.PersonID
INNER JOIN Classes cl ON s.ClassID = cl.ClassID
INNER JOIN Grades c ON s.GradeID = c.GradeID
WHERE sd.TermID = @TermID
  AND s.IsActive = 1  -- only active enrollments
GROUP BY 
    s.EnrollmentID,
    p.PersonID,
    st.StudentID,
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
                cmd.Parameters.AddWithValue("@TermID", termID); // pass the current term

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                // ✅ Safe DBNull handling for Percentage
                if (!dt.Columns.Contains("Percentage"))
                    dt.Columns.Add("Percentage", typeof(decimal));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["Percentage"] == DBNull.Value)
                        row["Percentage"] = 0m;
                }

                return dt;
            }
        }



        // Update existing score
        public static bool UpdateScore(int scoreID, decimal? testScore, decimal? examScore, string modifiedBy)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"UPDATE ScoresDetails 
                  SET TestScore = @TestScore, ExamScore = @ExamScore, ModifiedBy = @ModifiedBy, ModifiedDate = GETDATE()
                  WHERE ScoreID = @ScoreID", conn))
            {
                cmd.Parameters.AddWithValue("@ScoreID", scoreID);
                cmd.Parameters.AddWithValue("@TestScore", (object)testScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ExamScore", (object)examScore ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static DataTable GetScoresByEnrollmentAllSubjects(int enrollmentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT TestScore, ExamScore, TotalScore
            FROM ScoresDetails
            WHERE EnrollmentID = @EnrollmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        //public static DataTable GetTotalScoresByEnrollment()
        //{
        //    DataTable dt = new DataTable();
        //    using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
        //    {
        //        string query = @"
        //    WITH EnrollmentTotals AS
        //    (
        //        SELECT 
        //            e.EnrollmentID,
        //            SUM(sd.TestScore + sd.ExamScore) AS TotalObtained,
        //            SUM(sd.TotalScore) AS TotalMax,
        //            CASE 
        //                WHEN SUM(sd.TotalScore) = 0 THEN 0
        //                ELSE (SUM(sd.TestScore + sd.ExamScore) * 100.0) / SUM(sd.TotalScore)
        //            END AS TotalPercentage
        //        FROM ScoresDetails sd
        //        INNER JOIN Enrollments e ON e.EnrollmentID = sd.EnrollmentID
        //        GROUP BY e.EnrollmentID
        //    )
        //    SELECT EnrollmentID, TotalPercentage
        //    FROM EnrollmentTotals;
        //";

        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
        //        adapter.Fill(dt);
        //    }
        //    return dt;
        //}

        public static DataTable GetClassScores(int classID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    p.FirstName + ' ' + p.LastName AS StudentName,
                    ISNULL(sd.TestScore, 0) AS TestScore,
                    ISNULL(sd.ExamScore, 0) AS ExamScore,
                    ((ISNULL(sd.TestScore, 0)/30.0)*30 + (ISNULL(sd.ExamScore,0)/70.0)*70) AS TotalScore,
                    t.TermEnd
                FROM Enrollments e
                INNER JOIN Students s ON e.StudentID = s.StudentID
                INNER JOIN People p ON s.PersonID = p.PersonID
                LEFT JOIN ScoresDetails sd
                    ON sd.EnrollmentID = e.EnrollmentID
                    AND sd.TermID = @TermID
                INNER JOIN Terms t
                    ON t.TermID = @TermID
                WHERE e.ClassID = @ClassID
                ORDER BY p.LastName, p.FirstName", conn))
            {
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        // Load by enrollment, term, and subject
        //public static DataTable LoadByEnrollmentTermAndSubject(int enrollmentID, int termID, int subjectID)
        //{
        //    using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
        //    using (SqlCommand cmd = new SqlCommand(
        //        @"SELECT * FROM ScoresDetails 
        //          WHERE EnrollmentID = @EnrollmentID AND TermID = @TermID AND SubjectID = @SubjectID", conn))
        //    {
        //        cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
        //        cmd.Parameters.AddWithValue("@TermID", termID);
        //        cmd.Parameters.AddWithValue("@SubjectID", subjectID);

        //        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
        //        {
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);
        //            return dt;
        //        }
        //    }
        //}

        // Optional: load all scores for enrollment and term
        public static DataTable LoadByEnrollmentAndTerm(int enrollmentID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM ScoresDetails 
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
        public static DataTable GetAll ()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                  select *from ScoresDetails";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        public static DataTable LoadByEnrollment (int enrollmentID )
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT * FROM ScoresDetails 
                  WHERE EnrollmentID = @EnrollmentID ", conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
              

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
