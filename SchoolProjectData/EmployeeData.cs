using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectData
{
    public class clsEmployeeData
    {
        public static bool GetEmployeeByID(int EmployeeID, ref int PersonID, ref string JobTitle, ref double MonthlySalary, ref string HiredDate, ref bool EmployeeStatus)
        {
            bool IsFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    IsFound = true;
                    PersonID = (int)reader["PersonID"];
                    JobTitle = (string)reader["Position"];
                    MonthlySalary = (double)reader["MonthlySalary"];
                    HiredDate = (string)reader["HiredDate"];
                    EmployeeStatus = (bool)reader["EmployeeStatus"];
                }
                else
                {
                    IsFound = false;
                }
            }
            catch (Exception ex)
            {
                IsFound = false; Console.WriteLine("Exception Message: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
            }
            finally { connection.Close(); }

            return IsFound;
        }



        public static bool GetEmployeeByPersonID(int PersonID, ref int EmployeeID, ref string JobTitle, ref double MonthlySalary, ref string HiredDate, ref bool EmployeeStatus)
        {
            bool IsFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM Employees where PersonID=@PersonID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    IsFound = true;
                    PersonID = (int)reader["EmployeeID"];
                    JobTitle = (string)reader["Position"];

                    MonthlySalary = (double)reader["MonthlySalary"];
                    HiredDate = (string)reader["HiredDate"];
                    EmployeeStatus = (bool)reader["EmployeeStatus"];
                }
                else
                {
                    IsFound &= false;
                }
            }
            catch (Exception ex) { IsFound = false; }
            finally { connection.Close(); }

            return IsFound;
        }



        public static int AddNewEmployee(int PersonID, string JobTitle, double MonthlySalary, string HiredDate, bool EmployeeStatus)
        {
            int EmpolyeeID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"INSERT INTO Employees (PersonID,Position, MonthlySalary, HiredDate, EmployeeStatus)
                 VALUES (@PersonID,@Position, @MonthlySalary, @HiredDate, @EmployeeStatus);
                 SELECT SCOPE_IDENTITY();";





            SqlCommand command = new SqlCommand(@query, connection);

            //command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@Position", JobTitle);
            command.Parameters.AddWithValue("@MonthlySalary", MonthlySalary);
            command.Parameters.AddWithValue("@HiredDate", HiredDate);
            command.Parameters.AddWithValue("@EmployeeStatus", EmployeeStatus);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    EmpolyeeID = insertedID;
                }

            }
            catch (Exception ex) { }
            finally { connection.Close(); }




            return EmpolyeeID;
        }


        public static bool UpdateEmployee(int EmployeeID, int PersonID, string JobTitle, double MonthlySalary, string HiredDate, bool EmployeeStatus)
        {
            int rowsAffected = -1;
            SqlConnection connection = new SqlConnection(@clsDataAccessSettings.ConnectionString);
            string query = @"UPDATE Employees
                          SET Position = @Position,
                              MonthlySalary = @MonthlySalary,
                              HiredDate     = @HiredDate,
                              EmployeeStatus = @EmployeeStatus
                              WHERE EmployeeID = @EmployeeID;";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            command.Parameters.AddWithValue("@Position", JobTitle);
            command.Parameters.AddWithValue("@MonthlySalary", MonthlySalary);
            command.Parameters.AddWithValue("@HiredDate", HiredDate);
            command.Parameters.AddWithValue("@EmployeeStatus", EmployeeStatus);
            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception ex) { return false; }
            finally { connection.Close(); }


            return rowsAffected > 0;
        }



        public static bool DeleteEmployee(int EmployeeID)
        {
            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "delete Employees where EmployeeID = @EmployeeID";
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            finally { connection.Close(); }
            return rowsAffected > 0;



        }

        public static DataTable GetAllEmployees()
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT \r\n    Employees.EmployeeID, \r\n    Employees.PersonID, \r\n    FullName = People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '') + ' ' + People.LastName,\r\n    People.Gender,  \r\n    CASE\r\n        WHEN People.Gender = 0 THEN 'Male'\r\n        ELSE 'Female'\r\n    END AS GenderCaption,\r\n    Employees.MonthlySalary, \r\n    Employees.HiredDate,\r\n    Employees.Position, \r\n    Employees.EmployeeStatus  \r\nFROM Employees \r\nINNER JOIN People ON Employees.PersonID = People.PersonID;";

            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader != null)
                {
                    dt.Load(reader);
                }
                reader.Close();

            }
            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally { connection.Close(); }

            return dt;

        }


        public static bool DoEmployeeExists(int EmployeeID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT 1 AS Found FROM Employees WHERE EmployeeID = @EmployeeID   AND EmployeeStatus = 1;";
            SqlCommand command = new SqlCommand(@query, connection);

            command.Parameters.AddWithValue("@EmployeeID", EmployeeID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }

            return isFound;
        }



        public static bool DoEmployeeExistsForPersonID(int PersonID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT Found=1 from Employees where PersonID=@PersonID";
            SqlCommand command = new SqlCommand(@query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                isFound = reader.HasRows;
                reader.Close();
            }
            catch (Exception ex) { }
            finally { connection.Close(); }

            return isFound;
        }
        public static bool IsPersonEmployee(int personID)
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Employees WHERE PersonID = @PersonID", con))
                {
                    cmd.Parameters.AddWithValue("@PersonID", personID);
                    con.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }


    }
}
