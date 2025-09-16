using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsInstallmentFrequencyData
    {
        private static string connectionString => "Server=.;Database=sssdb;User Id=sa;Password=sa123456;";

        /// <summary>
        /// Returns all installment frequencies with only ID and Name.
        /// </summary>
        public static DataTable GetAll()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT InstallmentFrequencyID, Name FROM InstallmentFrequencies ORDER BY InstallmentFrequencyID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }

        /// <summary>
        /// Optional: Get a single frequency by ID.
        /// </summary>
        public static DataRow GetByID(int frequencyID)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT InstallmentFrequencyID, Name FROM InstallmentFrequencies WHERE InstallmentFrequencyID = @ID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@ID", frequencyID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
                return null;
        }
    }
}
