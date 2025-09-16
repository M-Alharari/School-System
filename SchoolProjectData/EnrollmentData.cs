using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsEnrollmentData
    {
        public static bool DoesEnrollmentExist(int studentID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM Enrollments 
                    WHERE StudentID = @StudentID AND TermID = @TermID AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public static bool GetEnrollmentByEnrollmentID(int enrollmentID,
    ref int studentID, ref int classID, ref int gradeID,
    ref int termID, ref DateTime enrollmentDate, ref bool isActive,
    ref int createdByUserID, ref DateTime createdAt, ref string modifiedByUser, ref DateTime modifiedAt)
        {
            bool found = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT * FROM Enrollments 
            WHERE EnrollmentID = @EnrollmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                found = true;
                                studentID = Convert.ToInt32(reader["StudentID"]);
                                classID = Convert.ToInt32(reader["ClassID"]);
                                gradeID = Convert.ToInt32(reader["GradeID"]);
                                termID = Convert.ToInt32(reader["TermID"]);
                                enrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]);
                                isActive = Convert.ToBoolean(reader["IsActive"]);
                                createdByUserID = Convert.ToInt32(reader["CreatedByUserID"]);
                                createdAt = Convert.ToDateTime(reader["CreatedAt"]);

                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByUser")))
                                    modifiedByUser = reader["ModifiedByUser"].ToString();
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedAt")))
                                    modifiedAt = Convert.ToDateTime(reader["ModifiedAt"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                    }
                }
            }

            return found;
        }

        public static bool MarkAsCompleted(int enrollmentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Enrollments
                                 SET IsActive = 0, CompletedDate = GETDATE()
                                 WHERE EnrollmentID = @EnrollmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                conn.Open();
                int affected = cmd.ExecuteNonQuery();
                return affected > 0;
            }
        }
        public static int AddNewEnrollment(int studentID, int classID, int gradeID, int termID,
                                     DateTime enrollmentDate, bool isActive,
                                     int createdByUserID, DateTime createdAt)
        {
            int newID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();

                // Step 1: Get the next AttemptNo for this Student + Grade
                SqlCommand getAttemptCmd = new SqlCommand(@"
            SELECT ISNULL(MAX(AttemptNo), 0) + 1
            FROM Enrollments
            WHERE StudentID = @StudentID AND GradeID = @GradeID;", conn);

                getAttemptCmd.Parameters.AddWithValue("@StudentID", studentID);
                getAttemptCmd.Parameters.AddWithValue("@GradeID", gradeID); // ✅ fixed

                int nextAttemptNo = (int)getAttemptCmd.ExecuteScalar();

                // Step 2: Insert enrollment with next AttemptNo
                SqlCommand cmd = new SqlCommand(@"
            INSERT INTO Enrollments
            (StudentID, ClassID, GradeID, TermID, EnrollmentDate, IsActive, CreatedByUserID, CreatedAt, AttemptNo)
            VALUES (@StudentID, @ClassID, @GradeID, @TermID, @EnrollmentDate, @IsActive, @CreatedByUserID, @CreatedAt, @AttemptNo);
            SELECT SCOPE_IDENTITY();", conn);

                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@TermID", termID);
                cmd.Parameters.AddWithValue("@EnrollmentDate", enrollmentDate);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                cmd.Parameters.AddWithValue("@AttemptNo", nextAttemptNo);

                newID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return newID;
        }

        public static bool DeactivateEnrollment(int enrollmentID)
        {
            string query = @"UPDATE Enrollments
                     SET IsActive = 0
                     WHERE EnrollmentID = @EnrollmentID AND IsActive = 1"; // only deactivate if active

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0; // returns true only if actually deactivated
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool UpdateEnrollment(int enrollmentID, int studentID, int classID,
            int gradeID, int termID, /*DateTime enrollmentDate,*/ bool isActive
           /* int createdByUserID, DateTime createdAt*/)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE Enrollments SET
                        StudentID = @StudentID,
                        ClassID = @ClassID,
                        GradeID = @GradeID,
                        TermID = @TermID,
                       -- EnrollmentDate = @EnrollmentDate,
                        IsActive = @IsActive 
                       -- CreatedByUserID = @CreatedByUserID,
                       -- CreatedAt = @CreatedAt
                    WHERE EnrollmentID = @EnrollmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    cmd.Parameters.AddWithValue("@ClassID", classID);
                    cmd.Parameters.AddWithValue("@GradeID", gradeID);
                    cmd.Parameters.AddWithValue("@TermID", termID);
                    //cmd.Parameters.AddWithValue("@EnrollmentDate", enrollmentDate);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    //cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                    //cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log error
                    }
                }
            }

            return rowsAffected > 0;
        }

        public static bool GetEnrollmentByStudentID(int studentID, ref int enrollmentID, ref int classID, ref int gradeID,
            ref int termID, ref DateTime enrollmentDate, ref bool isActive,
            ref int createdByUserID, ref DateTime createdAt, ref string modifiedByUser, ref DateTime modifiedAt)
        {
            bool found = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT TOP 1 * FROM Enrollments 
                    WHERE StudentID = @StudentID
                    ORDER BY CreatedAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                found = true;
                                enrollmentID = Convert.ToInt32(reader["EnrollmentID"]);
                                classID = Convert.ToInt32(reader["ClassID"]);
                                gradeID = Convert.ToInt32(reader["GradeID"]);
                                termID = Convert.ToInt32(reader["TermID"]);
                                enrollmentDate = Convert.ToDateTime(reader["EnrollmentDate"]);
                                isActive = Convert.ToBoolean(reader["IsActive"]);
                                createdByUserID = Convert.ToInt32(reader["CreatedByUserID"]);
                                createdAt = Convert.ToDateTime(reader["CreatedAt"]);

                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedByUser")))
                                    modifiedByUser = reader["ModifiedByUser"].ToString();
                                if (!reader.IsDBNull(reader.GetOrdinal("ModifiedAt")))
                                    modifiedAt = Convert.ToDateTime(reader["ModifiedAt"]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error
                    }
                }
            }

            return found;
        }

        public static DataTable GetAllEnrollments()
        {
            DataTable dt = new DataTable();

            string query = @"
           SELECT 
    S.StudentID,   
    P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName AS FullName,
    P.Gender,
    CASE WHEN P.Gender = 0 THEN 'Male' ELSE 'Female' END AS GenderCaption,
    C.CountryName,
    G.GradeName,
    Cls.ClassName,
    E.EnrollmentDate,
	E.IsActive,
    E.AttemptNo   -- ✅ Added AttemptNo
FROM Enrollments E
INNER JOIN Students S ON E.StudentID = S.StudentID
INNER JOIN People P ON S.PersonID = P.PersonID
INNER JOIN Countries C ON P.NationalityCountryID = C.CountryID
LEFT JOIN Grades G ON E.GradeID = G.GradeID
LEFT JOIN Classes Cls ON E.ClassID = Cls.ClassID
-- LEFT JOIN Graduation Gt ON E.EnrollmentID = Gt.EnrollmentID  -- ❌ Removed since IsGraduated is not needed
WHERE E.IsActive = 1
ORDER BY E.EnrollmentDate;



";

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        dt.Load(reader);
                    }
                    catch (Exception)
                    {
                        // Log error
                    }
                }
            }

            return dt;
        }
        public static bool DeactivateEnrollment(int StudentID, int modifiedBy)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        UPDATE Enrollments
        SET IsActive = 0,
            ModifiedByUser = @ModifiedByUser,
            ModifiedAt = GETDATE()
        WHERE StudentID = @StudentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", StudentID);
                    command.Parameters.AddWithValue("@ModifiedByUser", modifiedBy);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }

            return (rowsAffected > 0);
        }

        public static bool DeleteEnrollment(int StudentID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM Enrollments WHERE StudentID = @StudentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", StudentID);

                    try
                    {
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                    }
                }
            }

            return rowsAffected > 0;
        }

        public static bool EnrollmentExists(int enrollmentID)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM Enrollments WHERE EnrollmentID = @EnrollmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    try
                    {
                        conn.Open();
                        exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                    }
                }
            }

            return exists;
        }

        public static DataTable GetScoresByEnrollmentID(int enrollmentID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM ExamScores WHERE EnrollmentID = @EnrollmentID";

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
                        // Log or handle the exception
                    }
                }
            }

            return dt;
        }

        public static int? GetActiveEnrollmentIDByStudentID(int studentID)
        {
            int? enrollmentID = null;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                  SELECT   EnrollmentID
FROM Enrollments
WHERE EnrollmentID = EnrollmentID
  AND IsActive = 1
ORDER BY EnrollmentDate DESC;
";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", studentID);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            enrollmentID = Convert.ToInt32(result);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception
                    }
                }
            }

            return enrollmentID;
        }
        public static DataRow GetEnrollmentByID(int enrollmentID)
        {
            string query = "SELECT * FROM Enrollments WHERE EnrollmentID = @EnrollmentID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                DataTable dt = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                    return dt.Rows[0];

                return null;
            }
        }
        // Get all enrollments for a student
        public static System.Data.DataTable GetEnrollmentsByStudentID(int studentID)
        {
            using (var conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var cmd = new SqlCommand("SELECT * FROM Enrollments WHERE StudentID=@StudentID ORDER BY CreatedAt DESC", conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                var dt = new System.Data.DataTable();
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        // Get total enrollments
        public static int GetTotalEnrollments()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Enrollments", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        // Get converted enrollments (students who have paid)
        public static int GetConvertedEnrollments()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TuitionPayments WHERE TuitionFeeID IS NOT NULL", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }




        // ✅ New function: Enrollment by Gender
        public static DataTable GetEnrollmentByGender()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT 
            CASE p.Gender
                WHEN 0 THEN 'Male'
                WHEN 1 THEN 'Female'
                ELSE 'Other'
            END AS Gender,
            COUNT(e.EnrollmentID) AS Total
        FROM Enrollments e
        INNER JOIN Students s ON e.StudentID = s.StudentID
        INNER JOIN People p ON s.PersonID = p.PersonID
        GROUP BY p.Gender;", con))
            {
                DataTable dt = new DataTable();
                con.Open();
                dt.Load(cmd.ExecuteReader());
                return dt;
            }
        }


        // ✅ Yearly Enrollment Trend
        public static DataTable GetYearlyEnrollmentTrend()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT 
                YEAR(EnrollmentDate) AS Year,
                COUNT(*) AS Total
            FROM Enrollments
            GROUP BY YEAR(EnrollmentDate)
            ORDER BY Year;";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }



        // ✅ Grade distribution with repeaters
        public static DataTable GetGradeDistributionWithRepeaters()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Grade", typeof(string));
            dt.Columns.Add("Total", typeof(int));
            dt.Columns.Add("Repeaters", typeof(int));

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                conn.Open();

                // Query to get total enrollments and repeaters per grade
                string query = @"
                    SELECT g.GradeName AS Grade,
                           COUNT(e.EnrollmentID) AS Total,
                           SUM(CASE 
                                   WHEN gr.IsGraduated = 0 THEN 1 
                                   ELSE 0 
                               END) AS Repeaters
                    FROM Enrollments e
                    INNER JOIN Grades g ON e.GradeID = g.GradeID
                    LEFT JOIN Graduation gr ON e.EnrollmentID = gr.EnrollmentID
                    GROUP BY g.GradeName
                    ORDER BY g.GradeName
                ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
    }











}
 
 
