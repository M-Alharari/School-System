using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class GraduationData
    {
        public static bool IsAlreadyGraduated(int enrollmentID, int termID)
        {
            string query = @"SELECT COUNT(*) 
                     FROM Enrollments 
                     WHERE EnrollmentID = @EnrollmentID 
                       AND TermID = @TermID 
                       AND IsGraduated = 1";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static bool GraduateAndPromoteStudent(int enrollmentID, int termID, decimal percentage, int createdBy)
        {
            bool passed = percentage >= 50m;

            string query = @"
        UPDATE Enrollments
        SET 
            IsGraduated = @IsGraduated,
            GraduationDate = GETDATE(),
            GraduatedByUserID = @CreatedBy
        WHERE EnrollmentID = @EnrollmentID 
          AND TermID = @TermID 
          AND IsGraduated = 0; -- ✅ only update once
    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@IsGraduated", passed ? 1 : 0);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool GraduationRecordExists(int enrollmentID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Graduation WHERE EnrollmentID=@EnrollmentID AND TermID=@TermID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }

        public static bool UpdateGraduationRecord(int enrollmentID, int termID, decimal finalAverage, string letterGrade, bool isGraduated)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Graduation
                         SET FinalAverage=@FinalAverage,
                             LetterGrade=@LetterGrade,
                             IsGraduated=@IsGraduated
                             
                         WHERE EnrollmentID=@EnrollmentID AND TermID=@TermID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@FinalAverage", finalAverage);
                cmd.Parameters.AddWithValue("@LetterGrade", letterGrade);
                cmd.Parameters.AddWithValue("@IsGraduated", isGraduated);
           

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static DataTable GetStudentTotalsForGraduation(int termID)
        {
            string sql = @"
       SELECT 
    e.EnrollmentID,
    p.PersonID,
    p.FirstName + ' ' + ISNULL(p.SecondName,'') + ' ' + p.LastName AS FullName,
    g.GradeName,
    c.ClassName,
    SUM(sd.TestScore + sd.ExamScore) AS TotalScore,
    SUM(sd.TestScore + sd.ExamScore) * 100.0 / SUM(30 + 70) AS Percentage,
    gr.IsGraduted AS IsGraduated  -- ✅ important
FROM Enrollments e
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People p ON s.PersonID = p.PersonID
INNER JOIN Grades g ON e.GradeID = g.GradeID
INNER JOIN Classes c ON e.ClassID = c.ClassID
LEFT JOIN ScoreDetailsPerTerm sd ON sd.EnrollmentID = e.EnrollmentID
LEFT JOIN Graduation gr ON gr.EnrollmentID = e.EnrollmentID AND gr.TermID = @TermID
WHERE e.TermID = @TermID
GROUP BY e.EnrollmentID, p.PersonID, p.FirstName, p.SecondName, p.LastName, g.GradeName, c.ClassName, gr.IsGraduted;

    ";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("@TermID", termID);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static bool AddGraduationRecord(int enrollmentID, int termID, decimal finalAverage, string letterGrade, bool isGraduated, int createdBy)
        {
            string sql = @"
                INSERT INTO Graduation (EnrollmentID, TermID, GraduationDate, FinalAverage, LetterGrade, IsGraduated, CreatedBy)
                VALUES (@EnrollmentID, @TermID, GETDATE(), @FinalAverage, @LetterGrade, @IsGraduated, @CreatedBy)";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@FinalAverage", finalAverage);
                cmd.Parameters.AddWithValue("@LetterGrade", letterGrade);
                cmd.Parameters.AddWithValue("@IsGraduated", isGraduated);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
