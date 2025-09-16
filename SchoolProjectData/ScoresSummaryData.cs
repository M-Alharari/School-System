using SchoolProjectData;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProject.Data
{
    public static class clsScoresSummaryData
    {
        public static DataTable GetClassStudentScoress(int classID, int subjectID)
        {
            string sql = @"
        SELECT 
    e.EnrollmentID,                 -- moved to first
    sd.ScoreDetailID,
    s.StudentID,  
    p.FirstName + ' ' 
        + ISNULL(p.SecondName,'') + ' ' 
        + ISNULL(p.ThirdName,'') + ' ' 
        + p.LastName AS StudentName,
    ISNULL(sd.TestScore, 0) AS TestScore,
    ISNULL(sd.ExamScore, 0) AS ExamScore,
    ISNULL(sd.TotalScore, 0) AS TotalScore,
    t.EndDate
FROM Enrollments e
INNER JOIN Students s 
    ON e.StudentID = s.StudentID
INNER JOIN People p 
    ON s.PersonID = p.PersonID
LEFT JOIN ScoreDetailsPerTerm sd 
    ON sd.EnrollmentID = e.EnrollmentID 
   AND sd.SubjectID = @SubjectID
LEFT JOIN Terms t 
    ON t.TermID = sd.TermID
WHERE e.ClassID = @ClassID
ORDER BY p.LastName, p.FirstName 
;";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }


        public static DataTable GetSubjectsByClass(int classID)
        {
            string sql = @"
            SELECT DISTINCT sub.SubjectID, sub.SubjectName
            FROM Subjects sub
            INNER JOIN ScoresDetails sd ON sd.SubjectID = sub.SubjectID
            INNER JOIN Enrollments e ON e.EnrollmentID = sd.EnrollmentID
            WHERE e.ClassID = @ClassID";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@ClassID", classID);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
        /// <summary>
        /// Get all students’ scores for a given class and term.
        /// </summary>
        public static DataTable GetClassStudentScores(int classID, int termID)
        {
            string sql = @"
          SELECT
        s.StudentID,
        p.FirstName + ' ' + p.LastName AS StudentName,
        ISNULL(sd.TestScore, 0) AS TestScore,
        ISNULL(sd.ExamScore, 0) AS ExamScore,
        ((ISNULL(sd.TestScore,0)/30.0)*30 + (ISNULL(sd.ExamScore,0)/70.0)*70) AS TotalScore,
        t.TermName,
        t.EndDate
    FROM Enrollments e
    INNER JOIN Students s ON e.StudentID = s.StudentID
    INNER JOIN People p ON s.PersonID = p.PersonID
    LEFT JOIN ScoresDetails sd
        ON sd.EnrollmentID = e.EnrollmentID AND sd.TermID = @TermID
    LEFT JOIN Terms t
        ON t.TermID = @TermID
    WHERE e.ClassID = @ClassID
    ORDER BY p.LastName, p.FirstName;
 
    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
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

    }
}
