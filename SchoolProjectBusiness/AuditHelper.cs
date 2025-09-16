using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectBusiness
{
    public class clsAuditLog
    {
        private static string connectionString = "Server=.;Database=sssdb;User Id=sa;Password=sa123456;";

        public int AuditID { get; private set; } = -1;
        public string TableName { get; set; } = "";
        public int RecordID { get; set; } = -1;
        public string Action { get; set; } = "";
        public string OldValues { get; set; } = "";
        public string NewValues { get; set; } = "";
        public int PerformedBy { get; set; } = -1;
        public DateTime PerformedAt { get; private set; }

        public clsAuditLog() { }

        // ---- Static method to quickly log an action ----
        public static void AddLog(string tableName, int recordID, string action, string oldValue, string newValue, int performedBy)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO AuditLogs 
                               (TableName, RecordID, Action, OldValues, NewValues, PerformedBy, PerformedAt)
                               VALUES (@TableName, @RecordID, @Action, @OldValues, @NewValues, @PerformedBy, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@TableName", tableName);
                    cmd.Parameters.AddWithValue("@RecordID", recordID);
                    cmd.Parameters.AddWithValue("@Action", action);
                    cmd.Parameters.AddWithValue("@OldValues", (object)oldValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@NewValues", (object)newValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@PerformedBy", performedBy);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ---- Get all audit logs ----
        public static DataTable GetAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM AuditLogs ORDER BY PerformedAt DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // ---- Find a specific audit log by ID ----
        public static clsAuditLog Find(int auditID)
        {
            clsAuditLog log = null;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM AuditLogs WHERE AuditID=@AuditID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@AuditID", auditID);
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            log = new clsAuditLog
                            {
                                AuditID = Convert.ToInt32(reader["AuditID"]),
                                TableName = reader["TableName"].ToString(),
                                RecordID = Convert.ToInt32(reader["RecordID"]),
                                Action = reader["Action"].ToString(),
                                OldValues = reader["OldValues"] != DBNull.Value ? reader["OldValues"].ToString() : null,
                                NewValues = reader["NewValues"] != DBNull.Value ? reader["NewValues"].ToString() : null,
                                PerformedBy = Convert.ToInt32(reader["PerformedBy"]),
                                PerformedAt = Convert.ToDateTime(reader["PerformedAt"])
                            };
                        }
                    }
                }
            }
            return log;
        }
    }
}
