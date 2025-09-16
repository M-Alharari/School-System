using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsSubjectData
    {
        // Add a new subject with audit info
        public static int AddNewSubject(string SubjectName, int createdBy)
        {
            int ID = -1;

            string query = @"
                INSERT INTO Subjects (SubjectName, CreatedBy, CreatedAt, ModifiedBy, ModifiedAt)
                VALUES (@SubjectName, @CreatedBy, GETDATE(), @ModifiedBy, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SubjectName", SubjectName);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                cmd.Parameters.AddWithValue("@ModifiedBy", createdBy);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        ID = insertedID;
                }
                catch { }
            }

            return ID;
        }

        // Update subject and audit
        public static bool UpdateSubject(int SubjectID, string SubjectName, int modifiedBy)
        {
            string query = @"
                UPDATE Subjects
                SET SubjectName=@SubjectName, ModifiedBy=@ModifiedBy, ModifiedAt=GETDATE()
                WHERE SubjectID=@SubjectID";

            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                cmd.Parameters.AddWithValue("@SubjectName", SubjectName);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch { }
            }

            return rowsAffected > 0;
        }

        // Delete subject (audit done in business layer)
        public static bool DeleteSubject(int SubjectID, int modifiedBy)
        {
            int rowsAffected = 0;
            string query = "DELETE FROM Subjects WHERE SubjectID=@SubjectID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);
                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch { }
            }

            return rowsAffected > 0;
        }

        // Get info by ID with audit fields
        public static bool GetSubjectInfoByID(int SubjectID, ref string SubjectName 
           )
        {
            bool isFound = false;

            string query = "SELECT * FROM Subjects WHERE SubjectID=@SubjectID";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SubjectID", SubjectID);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SubjectName = reader["SubjectName"].ToString();
                            
                            isFound = true;
                        }
                    }
                }
                catch { }
            }

            return isFound;
        }

        // Get info by Name with audit fields
        public static bool GetSubjectInfoByName(string SubjectName, ref int SubjectID,
            ref int createdBy, ref DateTime createdAt, ref int modifiedBy, ref DateTime modifiedAt)
        {
            bool isFound = false;

            string query = "SELECT * FROM Subjects WHERE SubjectName=@SubjectName";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@SubjectName", SubjectName);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            SubjectID = Convert.ToInt32(reader["SubjectID"]);
                            createdBy = Convert.ToInt32(reader["CreatedBy"]);
                            createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                            modifiedBy = Convert.ToInt32(reader["ModifiedBy"]);
                            modifiedAt = Convert.ToDateTime(reader["ModifiedAt"]);
                            isFound = true;
                        }
                    }
                }
                catch { }
            }

            return isFound;
        }

        public static DataTable GetAllSubjects()
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Subjects";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            dt.Load(reader);
                    }
                }
                catch { }
            }

            return dt;
        }

        // Keep this for enrollment-linked subjects
        public static DataTable GetSubjectsByEnrollmentID(int enrollmentID)
        {
            DataTable dt = new DataTable();
            string query = @"
                SELECT s.SubjectID, s.SubjectName
                FROM Subjects s
                INNER JOIN EnrollmentSubjects es ON s.SubjectID = es.SubjectID
                WHERE es.EnrollmentID = @EnrollmentID
                ORDER BY s.SubjectName";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                            dt.Load(reader);
                    }
                }
                catch { }
            }

            return dt;
        }
    }
}
