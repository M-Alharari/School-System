using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsGuardianData
    {
        public static int  GetPersonIDByGuardianID(int guardianID)
        {
            int  personID=-1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT PersonID 
                FROM Guardians 
                WHERE GuardianID = @GuardianID", conn))
            {
                cmd.Parameters.AddWithValue("@GuardianID", guardianID);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                    personID = Convert.ToInt32(result);
            }

            return personID;
        }

        // 1. Get all guardians
        public static DataTable GetAllGuardians()
        {
            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
                    SELECT g.GuardianID, g.PersonID, g.Relationship, g.CreatedBy, g.CreatedDate,
                           g.ModifiedBy, g.ModifiedDate,
                           CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName) AS FullName,
                           p.Phone, p.Email, p.Address
                    FROM Guardians g
                    INNER JOIN People p ON g.PersonID = p.PersonID
                    ORDER BY FullName";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    DataTable dt = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        // 2. Add new guardian
        public static int AddNewGuardian(int personID, string relationship, int createdBy)
        {
            int newID = -1;
            string sql = @"
                INSERT INTO Guardians (PersonID, Relationship, CreatedBy, CreatedDate)
                VALUES (@PersonID, @Relationship, @CreatedBy, GETDATE());
                SELECT SCOPE_IDENTITY();";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@PersonID", personID);
                cmd.Parameters.AddWithValue("@Relationship", relationship ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                try
                {
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int id))
                        newID = id;
                }
                catch { }
            }

            return newID;
        }

        // 3. Update guardian
        public static bool UpdateGuardian(int guardianID, int personID, string relationship, int modifiedBy)
        {
            int rowsAffected = 0;
            string sql = @"
                UPDATE Guardians
                SET PersonID=@PersonID,
                    Relationship=@Relationship,
                    ModifiedBy=@ModifiedBy,
                    ModifiedDate=GETDATE()
                WHERE GuardianID=@GuardianID";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@GuardianID", guardianID);
                cmd.Parameters.AddWithValue("@PersonID", personID);
                cmd.Parameters.AddWithValue("@Relationship", relationship ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifiedBy);

                try
                {
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch { }
            }

            return rowsAffected > 0;
        }

        // 4. Delete guardian
        public static bool DeleteGuardian(int guardianID)
        {
            int rowsAffected = 0;
            string sql = "DELETE FROM Guardians WHERE GuardianID=@GuardianID";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@GuardianID", guardianID);
                try
                {
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch { }
            }

            return rowsAffected > 0;
        }

        // 5. Get guardian info by ID (for audit / business layer)
        public static bool GetGuardianInfoByID(int guardianID, ref int personID, ref string relationship,
                                               ref int createdBy, ref DateTime createdDate,
                                               ref int modifiedBy, ref DateTime? modifiedDate)
        {
            bool isFound = false;
            string sql = @"SELECT * FROM Guardians WHERE GuardianID=@GuardianID";

            using (SqlConnection con = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@GuardianID", guardianID);
                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        isFound = true;
                        personID = Convert.ToInt32(reader["PersonID"]);
                        relationship = reader["Relationship"] != DBNull.Value ? reader["Relationship"].ToString() : string.Empty;
                        createdBy = Convert.ToInt32(reader["CreatedBy"]);
                        createdDate = Convert.ToDateTime(reader["CreatedDate"]);
                        modifiedBy = reader["ModifiedBy"] != DBNull.Value ? Convert.ToInt32(reader["ModifiedBy"]) : -1;
                        modifiedDate = reader["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(reader["ModifiedDate"]) : (DateTime?)null;
                    }
                }
                catch { }
            }

            return isFound;
        }

        // Overload for older clsGuardian methods that only need PersonID & Relationship
        public static bool GetGuardianInfoByID(int guardianID, ref int personID, ref string relationship)
        {
            int dummyCreatedBy = -1;
            DateTime dummyCreatedDate = DateTime.MinValue;
            int dummyModifiedBy = -1;
            DateTime? dummyModifiedDate = null;

            return GetGuardianInfoByID(guardianID, ref personID, ref relationship,
                                       ref dummyCreatedBy, ref dummyCreatedDate,
                                       ref dummyModifiedBy, ref dummyModifiedDate);
        }
    }
}
