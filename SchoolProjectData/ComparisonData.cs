using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsScoresData
    {
        public static DataTable GetAllGrades()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT GradeID, GradeName FROM Grades", conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }

            return dt;
        }

        public static DataTable GetGradeAverages(int termID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"
            SELECT 
                g.GradeID,
                g.GradeName,
                AVG(CASE 
                        WHEN sd.EnrollmentID IS NOT NULL 
                        THEN ISNULL(sd.TestScore,0) + ISNULL(sd.ExamScore,0) 
                    END) AS GradeAverage
            FROM Grades g
            INNER JOIN Classes c ON c.GradeID = g.GradeID
            LEFT JOIN Enrollments e ON e.ClassID = c.ClassID
            LEFT JOIN ScoreDetailsPerTerm sd 
                ON sd.EnrollmentID = e.EnrollmentID 
               AND sd.TermID = @TermID
            GROUP BY g.GradeID, g.GradeName
        ";
                cmd.Parameters.AddWithValue("@TermID", termID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        /// <summary>
        /// Returns students' average scores in a class for a specific term, including grade info.
        /// </summary>
        /// <param name="gradeID">Grade ID (optional, use 0 to ignore)</param>
        /// <param name="classID">Class ID</param>
        /// <param name="termID">Term ID</param>
        /// <returns>DataTable with StudentID, FullName, AvgScore, ClassID, ClassName, GradeID, GradeName</returns>
        public static DataTable GetStudentsAverageScores(int gradeID, int classID, int termID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string sql = @"
            SELECT
                p.PersonID AS StudentID,
                CONCAT_WS(' ', p.FirstName, p.SecondName, p.ThirdName, p.LastName) AS FullName,
                c.ClassName,
                SUM(sd.TestScore + sd.ExamScore) AS TotalScore,
                AVG(sd.TestScore + sd.ExamScore) AS AvgScore, -- ✅ Added back AvgScore
                SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) AS Percentage,
                CASE 
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 90 THEN 'A+'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 85 THEN 'A'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 80 THEN 'A-'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 75 THEN 'B+'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 70 THEN 'B'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 65 THEN 'B-'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 60 THEN 'C+'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 55 THEN 'C'
                    WHEN SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) >= 50 THEN 'C-'
                    ELSE 'F'
                END AS LetterGrade
            FROM ScoreDetailsPerTerm sd
            INNER JOIN Enrollments e ON sd.EnrollmentID = e.EnrollmentID
            INNER JOIN Students s ON e.StudentID = s.StudentID
            INNER JOIN People p ON s.PersonID = p.PersonID
            INNER JOIN Classes c ON e.ClassID = c.ClassID
            WHERE e.GradeID = @GradeID
              AND e.ClassID = @ClassID   -- ✅ Added class filtering
              AND sd.TermID = @TermID
            GROUP BY p.PersonID, p.FirstName, p.SecondName, p.ThirdName, p.LastName, c.ClassName
            ORDER BY TotalScore DESC;
        ";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@GradeID", gradeID);
                    cmd.Parameters.AddWithValue("@ClassID", classID);
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        public static DataTable GetGradesAverageScores(int termID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                     SELECT 
      c.ClassID,
      c.ClassName,
      AVG(sd.TotalScore) AS AvgTotalScore
  FROM Enrollments e
  INNER JOIN Classes c ON e.ClassID = c.ClassID
  INNER JOIN ScoresDetails sd ON sd.EnrollmentID = e.EnrollmentID
  WHERE e.GradeID = @GradeID AND sd.TermID = @TermID
  GROUP BY c.ClassID, c.ClassName
  ORDER BY c.ClassName;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TermID", termID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
        private static string connString = clsDataAccessSettings.ConnectionString; // Replace with your DB connection
        public static DataTable GetGradeAverageScoresByTerm(int termID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
        SELECT 
            g.GradeID,
            g.GradeName,
            AVG(sd.TotalScore) AS AvgTotalScore
        FROM Enrollments e
        INNER JOIN Grades g ON e.GradeID = g.GradeID
        INNER JOIN ScoresDetails sd ON sd.EnrollmentID = e.EnrollmentID
        WHERE sd.TermID = @TermID
        GROUP BY g.GradeID, g.GradeName
        ORDER BY AvgTotalScore";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }

        /// <summary>
        /// Returns the average total score of each class in a grade for a specific term.
        /// </summary>
        public static DataTable GetClassAverageScores(int gradeID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                string query = @"
                SELECT 
                    c.ClassID,
                    c.ClassName,
                    AVG(sd.TotalScore) AS AvgTotalScore
                FROM Enrollments e
                INNER JOIN Classes c ON e.ClassID = c.ClassID
                INNER JOIN ScoreDetailsPerTerm sd ON sd.EnrollmentID = e.EnrollmentID
                WHERE e.GradeID = @GradeID AND sd.TermID = @TermID
                GROUP BY c.ClassID, c.ClassName
                ORDER BY c.ClassName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@GradeID", gradeID);
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }


        public static DataTable GetAllClassAverageScores(int termID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                  SELECT 
    c.ClassID,
    c.ClassName,
    g.GradeID,
    g.GradeName,
    AVG(sd.TotalScore) AS AvgTotalScore
FROM ScoreDetailsPerTerm sd
INNER JOIN Enrollments e ON e.EnrollmentID = sd.EnrollmentID
INNER JOIN Classes c ON c.ClassID = e.ClassID
INNER JOIN Grades g ON g.GradeID = c.GradeID
WHERE sd.TermID = @termID
GROUP BY c.ClassID, c.ClassName, g.GradeID, g.GradeName
ORDER BY g.GradeID, c.ClassID



                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TermID", termID);

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
