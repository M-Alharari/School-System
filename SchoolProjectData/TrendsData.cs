using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsTrendsData
    {
        public static double GetStudentAverageAttendance(int studentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString)) 
            {
                string query = @"
                    SELECT 
                        CASE WHEN COUNT(*) = 0 THEN 0
                             ELSE CAST(SUM(CAST(IsPresent AS int)) * 100.0 / COUNT(*) AS float)
                        END AS AvgAttendance
                    FROM EmployeeAttendance ea
                    INNER JOIN Enrollments e ON ea.EmployeeID = e.StudentID
                    WHERE e.StudentID = @StudentID
                    -- If this is students, replace EmployeeAttendance with StudentAttendance
                    ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != DBNull.Value)
                        return Convert.ToDouble(result);
                    else
                        return 0;
                }
            }
        }
        public static double GetStudentAverageGradeByEnrollment(int enrollmentID, int? termID = null)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT AVG(sd.TotalScore) AS AvgGrade
            FROM ScoreDetailsPerTerm sd
            WHERE sd.EnrollmentID = @EnrollmentID";

                if (termID.HasValue)
                    query += " AND sd.TermID = @TermID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    if (termID.HasValue)
                        cmd.Parameters.AddWithValue("@TermID", termID.Value);

                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    return (result != DBNull.Value) ? Convert.ToDouble(result) : 0;
                }
            }
        }

        public static double GetStudentAverageGrade(int studentID)
        {
            using (SqlConnection conn = new SqlConnection( clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT AVG(sd.TotalScore) AS AvgGrade
                    FROM ScoreDetailsPerTerm sd
                    INNER JOIN Enrollments e ON sd.EnrollmentID = e.EnrollmentID
                    WHERE e.StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    conn.Close();

                    if (result != DBNull.Value)
                        return Convert.ToDouble(result);
                    else
                        return 0;
                }
            }
        }
        // 1️⃣ Student trend: درجات الطالب عبر التيرمز
        public static DataTable GetStudentScoresByTerms(int studentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                 SELECT t.TermName, 
       AVG(sd.TestScore + sd.ExamScore) AS AvgScore
FROM ScoreDetailsPerTerm sd
INNER JOIN Terms t ON sd.TermID = t.TermID
INNER JOIN Enrollments e ON sd.EnrollmentID = e.EnrollmentID
WHERE e.StudentID = @StudentID
GROUP BY t.TermName, t.StartDate
ORDER BY t.StartDate;

";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 2️⃣ Class trend: متوسط الفصل لكل مادة عبر التيرمز
        public static DataTable GetClassAverageByTerms(int classID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        s.SubjectName,
                        t.TermName,
                        AVG(es.Score) AS AvgScore
                    FROM ExamScores es
                    INNER JOIN Students st ON es.StudentID = st.StudentID
                    INNER JOIN Subjects s ON es.SubjectID = s.SubjectID
                    INNER JOIN Terms t ON es.TermID = t.TermID
                    WHERE st.ClassID = @ClassID
                    GROUP BY s.SubjectName, t.TermName, t.TermOrder
                    ORDER BY t.TermOrder, s.SubjectName;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ClassID", classID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // 3️⃣ Grade trend: متوسط الصف لكل مادة عبر التيرمز
        public static DataTable GetGradeAverageByTerms(int gradeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        s.SubjectName,
                        t.TermName,
                        AVG(es.Score) AS AvgScore
                    FROM ExamScores es
                    INNER JOIN Students st ON es.StudentID = st.StudentID
                    INNER JOIN Subjects s ON es.SubjectID = s.SubjectID
                    INNER JOIN Terms t ON es.TermID = t.TermID
                    WHERE st.GradeID = @GradeID
                    GROUP BY s.SubjectName, t.TermName, t.TermOrder
                    ORDER BY t.TermOrder, s.SubjectName;";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }
    }
}
