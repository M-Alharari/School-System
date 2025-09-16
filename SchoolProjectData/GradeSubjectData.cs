using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsGradeSubjectData
    {
        // جلب جميع المواد المرتبطة بصف معين (GradeID)
        public static DataTable GetSubjectsByGradeID(int gradeID)
        {
            DataTable dt = new DataTable();

            string query = @"
                SELECT gs.SubjectID, s.SubjectName
                FROM GradeSubjects gs
                INNER JOIN Subjects s ON gs.SubjectID = s.SubjectID
                WHERE gs.GradeID = @GradeID
                ORDER BY s.SubjectName";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GradeID", gradeID);

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
                    throw new Exception("Error fetching subjects by GradeID: " + ex.Message);
                }
            }

            return dt;
        }
        public static bool AddGradeSubject(int gradeID, int subjectID, out string errorMessage)
        {
            errorMessage = "";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO GradeSubjects (GradeID, SubjectID) VALUES (@GradeID, @SubjectID)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    errorMessage = ex.Message;
                    return false;
                }
            }
        }


        // إضافة ربط مادة جديدة مع صف معين
        public static bool AddGradeSubject(int gradeID, int subjectID)
        {
            SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO GradeSubjects (GradeID, SubjectID) VALUES (@GradeID, @SubjectID)";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@GradeID", gradeID);
            cmd.Parameters.AddWithValue("@SubjectID", subjectID);

            try
            {
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return (rowsAffected > 0);
            }
            catch (SqlException ex)
            {
                //MessageBox.Show("SQL Error: " + ex.Message);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }


        // حذف ربط مادة مع صف معين
        public static bool DeleteGradeSubject(int gradeID, int subjectID)
        {
            string query = "DELETE FROM GradeSubjects WHERE GradeID = @GradeID AND SubjectID = @SubjectID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);

                try
                {
                    conn.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        // حذف كل المواد المرتبطة بصف معين (مثلاً عند حذف الصف أو إعادة تعيين المواد)
        public static bool DeleteAllSubjectsByGradeID(int gradeID)
        {
            string query = "DELETE FROM GradeSubjects WHERE GradeID = @GradeID";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@GradeID", gradeID);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}
