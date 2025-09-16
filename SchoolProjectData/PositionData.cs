using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsPositionData
    {
        private static string ConnectionString = @"YOUR_CONNECTION_STRING_HERE";

        public static int AddNewPosition(string positionName)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("INSERT INTO Positions (PositionName) OUTPUT INSERTED.PositionID VALUES (@PositionName)", conn))
            {
                cmd.Parameters.AddWithValue("@PositionName", positionName);
                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        public static bool UpdatePosition(int positionID, string positionName)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("UPDATE Positions SET PositionName = @PositionName WHERE PositionID = @PositionID", conn))
            {
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                cmd.Parameters.AddWithValue("@PositionName", positionName);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DeletePosition(int positionID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Positions WHERE PositionID = @PositionID", conn))
            {
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool GetPositionByPositionID(int positionID, ref string positionName)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT PositionName FROM Positions WHERE PositionID = @PositionID", conn))
            {
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    positionName = result.ToString();
                    return true;
                }
                return false;
            }
        }

        public static bool IsPositionExists(int positionID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(1) FROM Positions WHERE PositionID = @PositionID", conn))
            {
                cmd.Parameters.AddWithValue("@PositionID", positionID);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static DataTable GetAllPositions()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlDataAdapter adapter = new SqlDataAdapter("SELECT PositionID, PositionName FROM Positions", conn))
            {
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
    }
}
