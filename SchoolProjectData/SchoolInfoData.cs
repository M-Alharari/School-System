using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsSchoolInfoData
    {
        public static DataTable GetSchoolInfo()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT TOP 1 * FROM SchoolInfo ORDER BY SchoolInfoID DESC";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        public static int AddNewSchoolInfo(
          string name,
          string address,
          string phone,
          string email,
          string website,
          string logoPath,
          int createdByUserID,
          DateTime createdAt)
        {
            int newSchoolInfoID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO SchoolInfo 
                        (SchoolName, Address, Phone, Email, Website, Logo, LastUpdatedBy, LastUpdatedDate)
                    VALUES
                        (@Name, @Address, @Phone, @Email, @Website, @Logo, @CreatedByUserID, @CreatedAt);
                    SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", phone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Website", website ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Logo", string.IsNullOrEmpty(logoPath) ? DBNull.Value : (object)logoPath);
                    cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);
                    cmd.Parameters.AddWithValue("@CreatedAt", createdAt);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                            newSchoolInfoID = Convert.ToInt32(result);
                    }
                    catch (Exception ex)
                    {
                        // Log or handle error
                        throw;
                    }
                }
            }

            return newSchoolInfoID;
        }
        public static bool UpdateSchoolInfo(int id, string name, string address, string phone, string email, string website, string logoPath, int lastUpdatedBy)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE SchoolInfo
                                 SET SchoolName=@Name,
                                     Address=@Address,
                                     Phone=@Phone,
                                     Email=@Email,
                                     Website=@Website,
                                     Logo=@Logo,
                                     LastUpdatedBy=@LastUpdatedBy,
                                     LastUpdatedDate=GETDATE()
                                 WHERE SchoolInfoID=@ID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Name", name ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Address", address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Phone", phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Website", website ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Logo", string.IsNullOrEmpty(logoPath) ? DBNull.Value : (object)logoPath);
                cmd.Parameters.AddWithValue("@LastUpdatedBy", lastUpdatedBy);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
