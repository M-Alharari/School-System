using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsTeacherClassAssignmentData
    {
        public static int AddNewAssignment(int teacherID, int classID, int createdByUserID)
        {
            int assignmentID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO TeacherClassAssignments (TeacherID, ClassID, GradeID, AssignedDate, CreatedByUserID)
                                 SELECT @TeacherID, @ClassID, c.GradeID, GETDATE(), @CreatedByUserID
                                 FROM Classes c
                                 WHERE c.ClassID = @ClassID;

                                 SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                        assignmentID = Convert.ToInt32(result);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error adding new assignment", ex);
                }
            }

            return assignmentID;
        }

        public static bool UpdateAssignment(int assignmentID, int teacherID, int classID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE TeacherClassAssignments
                                 SET TeacherID = @TeacherID,
                                     ClassID = @ClassID,
                                     GradeID = (SELECT GradeID FROM Classes WHERE ClassID = @ClassID)
                                 WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating assignment", ex);
                }
            }

            return (rowsAffected > 0);
        }

        public static bool GetAssignmentByID(int assignmentID, ref int teacherID, ref int classID, ref int gradeID, ref DateTime assignedDate, ref int createdByUserID)
        {
            bool isFound = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT AssignmentID, TeacherID, ClassID, GradeID, AssignedDate, CreatedByUserID
                                 FROM TeacherClassAssignments
                                 WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        isFound = true;
                        teacherID = (int)reader["TeacherID"];
                        classID = (int)reader["ClassID"];
                        gradeID = (int)reader["GradeID"];
                        assignedDate = (DateTime)reader["AssignedDate"];
                        createdByUserID = (int)reader["CreatedByUserID"];
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving assignment by ID", ex);
                }
            }

            return isFound;
        }

        public static DataTable GetAssignmentsByTeacher(int teacherID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
    a.AssignmentID, 
    a.TeacherID, 
    -- Build teacher full name from People table
    FullName = COALESCE(p.FirstName, '') + ' ' 
             + COALESCE(p.SecondName, '') + ' ' 
             + COALESCE(p.ThirdName, '') + ' ' 
             + COALESCE(p.LastName, ''),
    a.ClassID, 
    c.ClassName, 
    a.GradeID, 
    g.GradeName,
    a.AssignedDate
FROM TeacherClassAssignments a
INNER JOIN Teachers t 
    ON a.TeacherID = t.TeacherID
INNER JOIN Employees e 
    ON t.EmployeeID = e.EmployeeID
INNER JOIN People p 
    ON e.PersonID = p.PersonID
INNER JOIN Classes c 
    ON a.ClassID = c.ClassID
INNER JOIN Grades g 
    ON a.GradeID = g.GradeID
WHERE a.TeacherID = @TeacherID;
";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving assignments for teacher", ex);
                }
            }

            return dt;
        }

        public static DataTable GetAllAssignments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
    a.AssignmentID,
    a.TeacherID,
    p.FirstName + ' ' + p.SecondName + ' ' + p.ThirdName + ' ' + p.LastName AS TeacherFullName,
    a.ClassID,
    c.ClassName,
    a.GradeID,
    g.GradeName,
    a.AssignedDate
FROM TeacherClassAssignments a
INNER JOIN Teachers t ON a.TeacherID = t.TeacherID
INNER JOIN Employees e ON t.EmployeeID = e.EmployeeID
INNER JOIN People p ON e.PersonID = p.PersonID
INNER JOIN Classes c ON a.ClassID = c.ClassID
INNER JOIN Grades g ON a.GradeID = g.GradeID

";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving all assignments", ex);
                }
            }

            return dt;
        }

        public static bool DeleteAssignment(int assignmentID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM TeacherClassAssignments WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting assignment", ex);
                }
            }

            return (rowsAffected > 0);
        }

        public static bool DoesAssignmentExist(int assignmentID)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) FROM TeacherClassAssignments WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    exists = ((int)cmd.ExecuteScalar() > 0);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking assignment existence", ex);
                }
            }

            return exists;
        }
    }
}
