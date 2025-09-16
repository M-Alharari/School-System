using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsFeeTypeData
    {
        public static DataTable GetAllFeeTypes()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM FeeTypes ORDER BY FeeTypeName", conn))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt;
        }
        public static string GetFeeTypeNameByID(int feeTypeID)
        {
            string feeTypeName = null;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT FeeTypeName FROM FeeTypes WHERE FeeTypeID = @FeeTypeID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FeeTypeID", feeTypeID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    feeTypeName = result.ToString();
                }
            }

            return feeTypeName;
        }
        public static bool AddFeeType(string name, string description, int createdBy)
        {
            string sql = @"INSERT INTO FeeTypes (FeeTypeName, Description, CreatedByUserID, CreatedAt)
                           VALUES (@Name, @Desc, @CreatedBy, GETDATE())";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Desc", description ?? string.Empty);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool UpdateFeeType(int feeTypeID, string name, string description, int modifiedBy)
        {
            string sql = @"UPDATE FeeTypes
                           SET FeeTypeName = @Name,
                               Description = @Desc,
                               ModifiedByUserID = @ModifiedBy,
                               ModifiedAt = GETDATE()
                           WHERE FeeTypeID = @ID";
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Desc", description ?? string.Empty);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);
                cmd.Parameters.AddWithValue("@ID", feeTypeID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        public static DataTable FindByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT * FROM FeeTypes WHERE FeeTypeID = @FeeTypeID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    // Add the missing parameter
                    cmd.Parameters.AddWithValue("@FeeTypeID", id);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    return dt; // Returns an empty DataTable if not found
                }
            }
        }



    }
}
