using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsTeacherSubjectAssignmentData
    {
        // إضافة تعيين مادة لمعلمة جديدة
        public static int AddNewAssignment(int teacherID, int subjectID, int createdByUserID)
        {
            int assignmentID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            INSERT INTO TeacherSubjectAssignments
            (TeacherID, SubjectID, AssignedDate, CreatedByUserID, ModifiedByUserID, ModifiedAt)
            VALUES
            (@TeacherID, @SubjectID, GETDATE(), @CreatedByUserID, @CreatedByUserID, GETDATE());

            SELECT SCOPE_IDENTITY();
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
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
                    throw new Exception("Error adding new subject assignment", ex);
                }
            }

            return assignmentID;
        }

        // تحديث تعيين مادة لمعلمة (مثلاً لتغيير المادة أو المعلم)
        public static bool UpdateAssignment(int assignmentID, int teacherID, int subjectID, int modifiedByUserID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            UPDATE TeacherSubjectAssignments
            SET TeacherID = @TeacherID,
                SubjectID = @SubjectID,
                ModifiedByUserID = @ModifiedByUserID,
                ModifiedAt = GETDATE()
            WHERE AssignmentID = @AssignmentID
        ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@ModifiedByUserID", modifiedByUserID);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error updating subject assignment", ex);
                }
            }

            return rowsAffected > 0;
        }


        // جلب تعيين معين بالتفصيل عبر معرفه
        public static bool GetAssignmentByID(int assignmentID, ref int teacherID, ref int subjectID, ref DateTime assignedDate, ref int createdByUserID)
        {
            bool isFound = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT AssignmentID, TeacherID, SubjectID, AssignedDate, CreatedByUserID
                    FROM TeacherSubjectAssignments
                    WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            isFound = true;
                            teacherID = (int)reader["TeacherID"];
                            subjectID = (int)reader["SubjectID"];
                            assignedDate = (DateTime)reader["AssignedDate"];
                            createdByUserID = (int)reader["CreatedByUserID"];
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving subject assignment by ID", ex);
                }
            }

            return isFound;
        }

        // جلب كل التعيينات الخاصة بمعلم معين (لعرض قائمة مواد المعلم)
        public static DataTable GetAssignmentsByTeacher(int teacherID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                  SELECT 
    a.AssignmentID, 
    a.TeacherID, 
    -- بناء الاسم الكامل للمعلم من أسماء الشخص
    CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.LastName) AS TeacherName,
    a.SubjectID, 
    s.SubjectName,
    a.AssignedDate
FROM TeacherSubjectAssignments a
INNER JOIN Teachers t ON a.TeacherID = t.TeacherID
INNER JOIN Employees e ON t.EmployeeID = e.EmployeeID
INNER JOIN People p ON e.PersonID = p.PersonID
INNER JOIN Subjects s ON a.SubjectID = s.SubjectID
WHERE a.TeacherID = @TeacherID
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

        // جلب كل التعيينات في النظام (عرض شامل)
        public static DataTable GetAllAssignments()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT a.AssignmentID, a.TeacherID, t.FullName AS TeacherName,
                           a.SubjectID, s.SubjectName,
                           a.AssignedDate
                    FROM TeacherSubjectAssignments a
                    INNER JOIN Teachers t ON a.TeacherID = t.TeacherID
                    INNER JOIN Subjects s ON a.SubjectID = s.SubjectID";

                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error retrieving all subject assignments", ex);
                }
            }

            return dt;
        }

        // حذف تعيين مادة لمعلمة معينة بواسطة معرف التعيين
        public static bool DeleteAssignment(int assignmentID)
        {
            int rowsAffected = 0;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM TeacherSubjectAssignments WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error deleting subject assignment", ex);
                }
            }

            return rowsAffected > 0;
        }

        // التحقق من وجود تعيين معين (حسب المعرف)
        public static bool DoesAssignmentExist(int assignmentID)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT COUNT(*) FROM TeacherSubjectAssignments WHERE AssignmentID = @AssignmentID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AssignmentID", assignmentID);

                try
                {
                    conn.Open();
                    exists = ((int)cmd.ExecuteScalar() > 0);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error checking subject assignment existence", ex);
                }
            }

            return exists;
        }
      
            public static void DeleteAssignmentsByTeacher(int teacherID)
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    string query = "DELETE FROM TeacherSubjectAssignments WHERE TeacherID = @TeacherID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }

        public static bool AddAssignment(int teacherID, int subjectID, int CreatedByUserID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            INSERT INTO TeacherSubjectAssignments
            (TeacherID, SubjectID, CreatedByUserID, AssignedDate, CreatedDate)
            VALUES (@TeacherID, @SubjectID, @CreatedByUserID, @AssignedDate, @CreatedDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                    cmd.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                    cmd.Parameters.AddWithValue("@AssignedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now); // 👈 fix

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

    }
}
 
