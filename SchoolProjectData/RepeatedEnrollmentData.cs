using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsRepeatedEnrollmentData
    {
        public static int AddRepeatedEnrollment(int originalEnrollmentID, int gradeID, int classID, int termID, string reason, int userID)
        {
            int repeatedID = -1;
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO RepeatedEnrollments
                    (OriginalEnrollmentID, GradeID, ClassID, TermID, Reason, CreatedBy , CreatedDate)
                    VALUES (@OriginalEnrollmentID, @GradeID, @ClassID, @TermID, @Reason, @CreatedBy, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@OriginalEnrollmentID", originalEnrollmentID);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@Reason", (object)reason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", userID);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int id))
                    repeatedID = id;
            }
            return repeatedID;
        }

        public static bool UpdateRepeatedEnrollment(int repeatedEnrollmentID, int originalEnrollmentID, int gradeID, int classID, int termID, string reason)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE RepeatedEnrollment
                    SET OriginalEnrollmentID = @OriginalEnrollmentID,
                        GradeID = @GradeID,
                        ClassID = @ClassID,
                        TermID = @TermID,
                        Reason = @Reason,
                        ModifiedDate = GETDATE()
                    WHERE RepeatedEnrollmentID = @RepeatedEnrollmentID;
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@RepeatedEnrollmentID", repeatedEnrollmentID);
                cmd.Parameters.AddWithValue("@OriginalEnrollmentID", originalEnrollmentID);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@Reason", (object)reason ?? DBNull.Value);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static bool DeleteRepeatedEnrollment(int repeatedID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM RepeatedEnrollments WHERE RepeatedEnrollmentID = @ID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", repeatedID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static DataTable GetByEnrollment(int enrollmentID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT r.*, t.TermName, g.GradeName, c.ClassName
                    FROM RepeatedEnrollments r
                    INNER JOIN Terms t ON r.TermID = t.TermID
                    INNER JOIN Grades g ON r.GradeID = g.GradeID
                    INNER JOIN Classes c ON r.ClassID = c.ClassID
                    WHERE r.OriginalEnrollmentID = @EnrollmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAllRepeated()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT r.*, t.TermName, g.GradeName, c.ClassName
                    FROM RepeatedEnrollments r
                    INNER JOIN Terms t ON r.TermID = t.TermID
                    INNER JOIN Grades g ON r.GradeID = g.GradeID
                    INNER JOIN Classes c ON r.ClassID = c.ClassID
                    ORDER BY r.CreatedDate DESC";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }
    }
}
