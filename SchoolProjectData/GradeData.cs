using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectData
{
    public class clsGradeData
    {

        public static bool GetGradeByID(int GradeID, ref string GradeName)
        {
            bool IsFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM Grades WHERE GradeID = @GradeID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@GradeID", GradeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    IsFound = true;
                    GradeName = (string)reader["GradeName"];


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

        public static bool GetGradeByName(string GradeName, ref int GradeID)
        {
            bool IsFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM Grades WHERE GradeName = @GradeName";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@GradeName", GradeName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    IsFound = true;
                    GradeID = (int)reader["GradeID"];


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

        //public static bool GetUserByTeacherID(ref int GradeID, ref string GradeName)
        //{
        //    bool IsFound = false;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"SELECT * FROM Grades WHERE GradeID = @GradeID";
        //    SqlCommand command = new SqlCommand(query, connection);

        //    command.Parameters.AddWithValue("@GradeID", GradeID);

        //    try
        //    {
        //        connection.Open();
        //        SqlDataReader reader = command.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            IsFound = true;
        //            GradeID = (int)reader["GradeID"];
        //            GradeName = (string)reader["GradeName"];


        //        }
        //        else
        //        {
        //            IsFound = false;
        //        }
        //    }
        //    catch (Exception ex) { IsFound = false; }
        //    finally { connection.Close(); }

        //    return IsFound;
        //}


        public static int AddNewGrade(string GradeName)
        {
            int ClassID = -1;
            SqlConnection connection = new SqlConnection((string)clsDataAccessSettings.ConnectionString);
            string query = @"insert into Grades (GradeName)
                VALUES (@GradeName);
                SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(@query, connection);

            command.Parameters.AddWithValue("@GradeName", GradeName);



            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    ClassID = insertedID;
                }
            }
            catch (Exception ex)
            {

            }
            finally { connection.Close(); }




            return ClassID;
        }

        public static bool UpdateGrade(int GradeID, string GradeName)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query =
                            @"UPDATE Grades 
                          set 
                              GradeName=@GradeName,  
                                  
                               where GradeID=@GradeID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@GradeID", GradeID);
            command.Parameters.AddWithValue("@GradeName", GradeName);



            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }


            return (rowsAffected > 0);
        }


        public static bool DeleteGrade(int GradeID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "delete Grades where GradeID=@GradeID";

            SqlCommand command = new SqlCommand(query, connection);


            command.Parameters.AddWithValue("@GradeID", GradeID);


            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }


            return (rowsAffected > 0);
        }


        public static DataTable GetAllGrades()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"select  Grades.GradeID, Grades.GradeName from Grades";

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


        public static bool DoGradeExists(int GradeID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "Select Found=1 from Grades where GradeID=@GradeID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@GradeID", GradeID);

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




        //public static int AssignTeacher(int TeacherID, int StudentID, string GradeName)
        //{
        //    int GradeID = -1;
        //    SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
        //    string query = @"INSERT INTO Teachers (TeacherID,StudentID,GradeName)
        //                     VALUES (@TeacherID, @StudentID,@GradeName);
        //                     SELECT SCOPE_IDENTITY();";

        //    SqlCommand command = new SqlCommand(@query, connection);

        //    command.Parameters.AddWithValue("@TeacherID", TeacherID);
        //    command.Parameters.AddWithValue("@UserName", StudentID);
        //    command.Parameters.AddWithValue("@GradeName", GradeName);

        //    try
        //    {
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null && int.TryParse(result.ToString(), out int insertedID))
        //        {
        //            GradeID = insertedID;
        //        }

        //    }
        //    catch { return GradeID; }
        //    finally { connection.Close(); }
        //    return GradeID;
        //}










    }
}
