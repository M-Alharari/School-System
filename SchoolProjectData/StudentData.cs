using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsStudentData
    {
        public static DataRow GetStudentInfoWithTuition(int studentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                      SELECT 
                p.FirstName + ' ' + p.SecondName + ' ' + p.ThirdName + ' ' + p.LastName AS FullName,
                CASE 
                    WHEN tp.IsFullyPaid = 1 THEN 'Paid'
                    WHEN tp.IsFullyPaid = 0 THEN 'Not Fully Paid'
                    ELSE 'No Payment'
                END AS TuitionStatus
            FROM Enrollments e
            INNER JOIN People p ON e.StudentID = p.PersonID
            LEFT JOIN TuitionPayments tp 
                ON tp.EnrollmentID = e.EnrollmentID
            WHERE e.StudentID = @StudentID
            ORDER BY tp.PaymentDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                    else
                        return null;
                }
            }
        }
        public static DataTable GetStudentsByClassID(int classID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
                    SELECT 
                        s.StudentID,
                        RTRIM(
                            COALESCE(p.FirstName,'') + ' ' +
                            COALESCE(p.SecondName,'') + ' ' +
                            COALESCE(p.ThirdName,'') + ' ' +
                            COALESCE(p.LastName,'')
                        ) AS FullName
                    FROM Enrollments e
                    INNER JOIN Students s ON e.StudentID = s.StudentID
                    INNER JOIN People p ON s.PersonID = p.PersonID
                    WHERE e.ClassID = @ClassID
                    ORDER BY p.FirstName, p.SecondName, p.ThirdName, p.LastName;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ClassID", classID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static bool GetStudentInfoByID(int StudentID, ref int PersonID )
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Students WHERE StudentID = @StudentID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentID", StudentID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        isFound = true;
                        PersonID = (int)reader["PersonID"];
                       
                    }
                    reader.Close();
                }
                catch
                {
                    isFound = false;
                }
            }
            return isFound;
        }

        public static int AddNewStudent(int PersonID,  int createdBy)
        {
            int StudentID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO Students (PersonID, CreatedBy, CreatedAt)
                    VALUES (@PersonID, @CreatedBy, GETDATE());
                    SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PersonID", PersonID);
                //command.Parameters.AddWithValue("@GuardianID", GuardianID);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                        StudentID = insertedID;
                }
                catch
                {
                    // handle exceptions if needed
                }
            }
            return StudentID;
        }

        public static bool UpdateStudent(int StudentID,   int modifiedBy)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE Students
                    SET  
                        ModifiedBy = @ModifiedBy,
                        ModifiedAt = GETDATE()
                    WHERE StudentID = @StudentID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentID", StudentID);
              
                command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                try
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
                catch
                {
                    return false;
                }
            }
            return rowsAffected > 0;
        }

        public static bool DeleteStudent(int StudentID, int modifiedBy)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            UPDATE Students
            SET IsActive = @isActive,
                ModifiedBy = @ModifiedBy,
                ModifiedAt = GETDATE()
            WHERE StudentID = @StudentID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StudentID", StudentID);
                    command.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                    command.Parameters.AddWithValue("@isActive", false);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllEnrollments()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                   SELECT 
    E.EnrollmentID,   -- ✅ Added EnrollmentID at first
    S.StudentID,   
    P.FirstName + ' ' + ISNULL(P.SecondName, '') + ' ' + ISNULL(P.ThirdName, '') + ' ' + P.LastName AS FullName,
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
WHERE E.IsActive = 1
ORDER BY E.EnrollmentDate;



";

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) dt.Load(reader);
                    reader.Close();
                }
                catch { throw; }
            }
            return dt;
        }

        public static DataTable GetAllStudents()
        {
            DataTable dt = new DataTable();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        S.StudentID,
                        P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName AS FullName,
                        P.Gender,
                        CASE WHEN P.Gender = 0 THEN 'Male' ELSE 'Female' END AS GenderCaption,
                        C.CountryName,
                        G.GradeName,
                        Cls.ClassName
                    FROM Students S
                    INNER JOIN People P ON S.PersonID = P.PersonID
                    INNER JOIN Countries C ON P.NationalityCountryID = C.CountryID
                    LEFT JOIN Enrollments E ON S.StudentID = E.StudentID
                    LEFT JOIN Grades G ON E.GradeID = G.GradeID
                    LEFT JOIN Classes Cls ON E.ClassID = Cls.ClassID";

                SqlCommand command = new SqlCommand(query, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows) dt.Load(reader);
                    reader.Close();
                }
                catch { }
            }
            return dt;
        }
        public static int GetTotalStudents()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Students", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public static bool DoStudentExists(int studentID)
        {
            bool exists = false;

            string query = @"SELECT 1 
                     FROM Students 
                     WHERE StudentID = @StudentID AND Status = 'Active'";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                conn.Open();

                var result = cmd.ExecuteScalar();
                exists = (result != null);
            }

            return exists;
        }

    }
}
