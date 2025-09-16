using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsEnrollmentHistoryData

    {  
        // Get a single history record by enrollment + term
        public static DataRow GetHistoryByEnrollmentIDAndTermID(int enrollmentID, int termID)
        {
            string query = @"SELECT * FROM EnrollmentHistory
                         WHERE EnrollmentID = @EnrollmentID AND TermID = @TermID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                return dt.Rows.Count > 0 ? dt.Rows[0] : null;
            }
        }
        public static bool Exists(int enrollmentID, int termID)
        {
            string query = @"SELECT COUNT(1) FROM EnrollmentHistory
                         WHERE EnrollmentID = @EnrollmentID AND TermID = @TermID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static DataTable GetStudentHistory(int studentID)
        {
            string query = @"
       
SELECT
    e.EnrollmentID,
    CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName) AS FullName,
    g.GradeName,
    c.ClassName,
    CONCAT(t.StartDate, ' - ', t.EndDate) AS TermPeriod,
    ISNULL(gr.FinalAverage, 0) AS FinalAverage,
    CASE 
        WHEN gr.IsGraduated = 1 THEN 'Passed'
        WHEN gr.IsGraduated = 0 THEN 'Failed'
        ELSE 'Ongoing'
    END AS Status,
   
    e.EnrollmentDate
FROM Enrollments e
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People p ON s.PersonID = p.PersonID
LEFT JOIN Grades g ON e.GradeID = g.GradeID
LEFT JOIN Classes c ON e.ClassID = c.ClassID
LEFT JOIN Terms t ON e.TermID = t.TermID
LEFT JOIN Graduation gr ON e.EnrollmentID = gr.EnrollmentID
LEFT JOIN EnrollmentHistory eh ON e.EnrollmentID = eh.EnrollmentID
WHERE e.StudentID = @StudentID
ORDER BY e.EnrollmentDate 


;";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetAllEnrollmentHistories()
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT
                    e.StudentID,
                    CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName) AS FullName,
                    g.GradeName,
                    c.ClassName,
                    e.EnrollmentDate,
                    gr.FinalAverage,
                    CASE 
                        WHEN gr.IsGraduated = 1 THEN 'Graduated' 
                        ELSE 'Not Graduated' 
                    END AS GraduationStatus
                FROM Graduation gr
                INNER JOIN Enrollments e ON gr.EnrollmentID = e.EnrollmentID
                INNER JOIN People p ON e.StudentID = p.PersonID
                INNER JOIN Grades g ON e.GradeID = g.GradeID
                INNER JOIN Classes c ON e.ClassID = c.ClassID
                ORDER BY p.LastName, p.FirstName;";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
   
        

        // Add a new enrollment history record
        public static bool Add(int enrollmentID, int termID, bool isGraduated)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO EnrollmentHistory
                    (EnrollmentID, TermID, IsGraduated)
                    VALUES (@EnrollmentID, @TermID, @IsGraduated)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@TermID", termID);
                    cmd.Parameters.AddWithValue("@IsGraduated", isGraduated);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Update graduation status or term for an existing record
        public static bool Update(int historyID, int termID, bool isGraduated)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE EnrollmentHistory
                    SET TermID = @TermID,
                        IsGraduated = @IsGraduated,
                        ModifiedDate = GETDATE()
                    WHERE HistoryID = @HistoryID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HistoryID", historyID);
                    cmd.Parameters.AddWithValue("@TermID", termID);
                    cmd.Parameters.AddWithValue("@IsGraduated", isGraduated);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Get all history for a student
        public static DataTable GetByStudent(int enrollmentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT h.HistoryID, h.EnrollmentID, h.TermID, t.TermName, h.IsGraduated, h.CreatedDate, h.ModifiedDate
                    FROM EnrollmentHistory h
                    INNER JOIN Terms t ON h.TermID = t.TermID
                    WHERE h.EnrollmentID = @EnrollmentID
                    ORDER BY h.CreatedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
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

        // Optional: Delete a history record
        public static bool Delete(int historyID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM EnrollmentHistory WHERE HistoryID = @HistoryID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@HistoryID", historyID);
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
