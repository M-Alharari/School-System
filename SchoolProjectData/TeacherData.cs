using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectData
{
    public class clsTeacherData
    {

        public static bool GetTeachersByID(int TeacherID, ref int EmployeeID/*, ref int ClassID, ref int SubjectID*/)

        {
            bool IsFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT TeacherID, EmployeeID/*, ClassID, SubjectID*/ FROM Teachers WHERE TeacherID = @TeacherID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TeacherID", TeacherID);

                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            IsFound = true;

                            // Handle EmployeeID (NOT NULL)
                            EmployeeID = (int)(reader["EmployeeID"]); // Safer than (int) cast [[1]]

                            //// Handle ClassID (NULLABLE)
                            //ClassID = (int)reader["ClassID"];

                            //// Handle SubjectID (NULLABLE)
                            //SubjectID = (int)reader["SubjectID"];
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Log the exception (do not swallow it silently) [[3]]
                        throw new Exception("Database error: " + ex.Message, ex);
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                            connection.Close();
                    }
                }
            }
            return IsFound;
        }

        public static bool GetTeachersByEmployeeID(int EmployeeID, ref int TeacherID/*, ref int ClassID, ref int SubjectID*/)
        {
            bool IsFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM Teachers WHERE EmployeeID = @EmployeeID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    IsFound = true;


                    TeacherID = (int)reader["TeacherID"];

                    //TeachingSubject: allows null in database so we should handle null

                    //ClassID = (int)reader["ClassID"];



                    ////TeachingClass: allows null in database so we should handle null

                    //SubjectID = (int)reader["SubjectID"];




                }
                else
                {
                    IsFound = false;
                }
            }
            catch (Exception ex) { IsFound = false; }
            finally { connection.Close(); }

            return IsFound;
        }


        public static int AddNewTeacher(int EmployeeID/*, int ClassID, int SubjectID*/)
        {
            int TeacherID = -1;
            string connectionString = (string)clsDataAccessSettings.ConnectionString;

            // Ensure the SQL query includes all parameters
            string query = @"INSERT INTO Teachers (EmployeeID/*, ClassID, SubjectID*/)
                     VALUES (@EmployeeID/*, @ClassID, @SubjectID*/);
                     SELECT SCOPE_IDENTITY();";

            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                // Add all parameters explicitly
                command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
                //command.Parameters.AddWithValue("@ClassID", ClassID);
                //command.Parameters.AddWithValue("@SubjectID", SubjectID);

                try
                {
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        TeacherID = insertedID;
                    }
                }
                catch (SqlException ex)
                {
                    // Handle SQL-specific errors
                    throw new Exception($"SQL Error: {ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    // Handle general errors
                    throw new Exception($"General Error: {ex.Message}", ex);
                }
            }

            return TeacherID;
        }

        public static bool UpdateTeacher(int TeacherID, int EmployeeID/*, int ClassID, int SubjectID*/)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query =
                            @"UPDATE Teachers 
                            set    
                              EmployeeID=@EmployeeID
                              //ClassID=@ClassID,
                              // SubjectID=@SubjectID
                              where TeacherID=@TeacherID";
            SqlCommand command = new SqlCommand(query, connection);


            command.Parameters.AddWithValue("@TeacherID", TeacherID);


            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            //command.Parameters.AddWithValue("@ClassID", ClassID);
            //command.Parameters.AddWithValue("@SubjectID", SubjectID);


            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }


            return (rowsAffected > 0);
        }


        public static bool DeleteTeacher(int TeacherID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "delete Teachers where TeacherID=@TeacherID";

            SqlCommand command = new SqlCommand(query, connection);


            command.Parameters.AddWithValue("@TeacherID", TeacherID);


            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }


            return (rowsAffected > 0);
        }
        public static int GetTotalTeachers()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Teachers", con))
            {
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }


        public static DataTable GetAllTeachers()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"
SELECT 
    Teachers.TeacherID, 
    Teachers.EmployeeID, 
    FullName = COALESCE(People.FirstName, '') + ' ' 
             + COALESCE(People.SecondName, '') + ' ' 
             + COALESCE(People.ThirdName, '') + ' ' 
             + COALESCE(People.LastName, '')
FROM Teachers
INNER JOIN Employees 
    ON Teachers.EmployeeID = Employees.EmployeeID
INNER JOIN People 
    ON Employees.PersonID = People.PersonID;


";

            SqlCommand command = new SqlCommand(@query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)

                {
                    dt.Load(reader);
                }

                reader.Close();

            }
            catch (Exception ex) { }
            finally { connection.Close(); }

            return dt;
        }


        public static bool DoTeacherExists(int TeacherID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select Found=1 from Teachers where TeacherID=@TeacherID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TeacherID", TeacherID);

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    isFound = true;
                }
            }
            catch (Exception ex) { }
            finally { connection.Close(); }




            return isFound;
        }



        public static bool DoEmployeeExists(int TeacherID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select Found=1 from Teachers where TeacherID=@TeacherID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@TeacherID", TeacherID);

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    isFound = true;
                }
            }
            catch (Exception ex) { }
            finally { connection.Close(); }




            return isFound;
        }

















    }
}
