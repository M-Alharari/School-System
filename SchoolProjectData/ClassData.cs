using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsClassData
    {
        public static int AddNewClass(string className, int gradeID)
        {
            int newClassID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO Classes (ClassName, GradeID)
                                 VALUES (@ClassName, @GradeID);
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassName", className);
                command.Parameters.AddWithValue("@GradeID", gradeID);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null)
                        newClassID = Convert.ToInt32(result);
                }
                catch
                {
                    // Log or handle exception if needed
                    newClassID = -1;
                }
            }

            return newClassID;
        }

        public static bool UpdateClass(int classID, string className)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Classes SET ClassName = @ClassName
                                 WHERE ClassID = @ClassID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassName", className);
                command.Parameters.AddWithValue("@ClassID", classID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return (rowsAffected > 0);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool GetClassByID(int classID, ref string className, ref int gradeID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ClassName, GradeID FROM Classes WHERE ClassID = @ClassID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassID", classID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        className = reader["ClassName"].ToString();
                        gradeID = Convert.ToInt32(reader["GradeID"]);
                        isFound = true;
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

        public static bool GetClassByName(string className, ref int classID, ref int gradeID)
        {
            bool isFound = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT ClassID, GradeID FROM Classes WHERE ClassName = @ClassName";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassName", className);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        classID = Convert.ToInt32(reader["ClassID"]);
                        gradeID = Convert.ToInt32(reader["GradeID"]);
                        isFound = true;
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

        public static DataTable GetAllClasses()
        {
            DataTable dt = new DataTable();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT * FROM Classes ORDER BY ClassName";

                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                adapter.Fill(dt);
            }

            return dt;
        }

        public static DataTable GetClassesByGradeID(int gradeID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT ClassID, ClassName FROM Classes WHERE GradeID = @GradeID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@GradeID", gradeID);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }


        public static bool DeleteClass(int classID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Classes WHERE ClassID = @ClassID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassID", classID);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return (rowsAffected > 0);
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool DoClassExists(int classID)
        {
            bool exists = false;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 1 FROM Classes WHERE ClassID = @ClassID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClassID", classID);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    exists = result != null;
                }
                catch
                {
                    exists = false;
                }
            }

            return exists;
        }
    }
}
