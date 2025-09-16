using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsPaymentTypeData
    {
        public static DataTable GetAllPaymentTypes()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM PaymentTypes";
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                }
                catch
                {
                    // Logging can be added here
                }
            }

            return dt;
        }

        public static bool GetPaymentTypeInfoByID(int id, ref string name)
        {
            bool isFound = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT PaymentTypeName FROM PaymentTypes WHERE PaymentTypeID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        name = (string)reader["PaymentTypeName"];
                        isFound = true;
                    }
                }
                catch { }
            }

            return isFound;
        }

        public static int AddNewPaymentType(string name, int createdByUserID)
        {
            int newID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO PaymentTypes (PaymentTypeName, CreatedByUserID)
                                 VALUES (@name, @userID);
                                 SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@userID", createdByUserID);

                try
                {
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    newID = Convert.ToInt32(result);
                }
                catch { }
            }

            return newID;
        }

        public static bool UpdatePaymentType(int id, string name, int modifiedByUserID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE PaymentTypes 
                                 SET PaymentTypeName = @name,
                                     ModifiedByUserID = @modifiedBy,
                                     ModifiedAt = GETDATE()
                                 WHERE PaymentTypeID = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@modifiedBy", modifiedByUserID);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool DeletePaymentType(int id)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM PaymentTypes WHERE PaymentTypeID = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
