using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsSalaryDeductionSummaryData
    {
        public static bool AreDeductionsCalculated(int year, int month)
        {
            string query = @"
         SELECT COUNT(DISTINCT e.EmployeeID) AS TotalEmployees,
        (SELECT COUNT(DISTINCT d.EmployeeID)
         FROM SalaryDeductionSummary d
         WHERE d.Year = @Year AND d.Month = @Month) AS EmployeesWithDeductions
 FROM Employees e
 "; // again, filter only active employees

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int totalEmployees = reader.GetInt32(0);
                        int employeesWithDeductions = reader.GetInt32(1);

                        return employeesWithDeductions == totalEmployees && totalEmployees > 0;
                    }
                }
            }

            return false;
        }

        // Generate or update monthly summary
        public static bool GenerateMonthlySummary(int month, int year)
        {
            string sql = @"
        MERGE SalaryDeductionSummary AS target
        USING (
            SELECT 
                e.EmployeeID,
                @Month AS Month,
                @Year AS Year,
                SUM(CASE WHEN a.IsPresent = 0 THEN 1 ELSE 0 END) AS TotalAbsenceDays,
                SUM(CASE WHEN a.IsPresent = 0 THEN e.MonthlySalary / 30 ELSE 0 END) AS TotalDeduction
            FROM EmployeeAttendance a
            INNER JOIN Employees e ON a.EmployeeID = e.EmployeeID
            WHERE MONTH(a.AttendanceDate) = @Month AND YEAR(a.AttendanceDate) = @Year
            GROUP BY e.EmployeeID
        ) AS source
        ON target.EmployeeID = source.EmployeeID AND target.Month = source.Month AND target.Year = source.Year
        WHEN MATCHED THEN 
            UPDATE SET TotalAbsenceDays = source.TotalAbsenceDays,
                       TotalDeduction = source.TotalDeduction,
                       ModifiedDate = GETDATE()
        WHEN NOT MATCHED THEN
            INSERT (EmployeeID, Month, Year, TotalAbsenceDays, TotalDeduction, CreatedDate)
            VALUES (source.EmployeeID, source.Month, source.Year, source.TotalAbsenceDays, source.TotalDeduction, GETDATE());";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }


        // Get summary for a specific month
        public static DataTable GetMonthlySummary(int month, int year)
        {
            string sql = @"
        SELECT 
            s.EmployeeID,
            p.FirstName + ' ' + 
            ISNULL(p.SecondName + ' ', '') + 
            ISNULL(p.ThirdName + ' ', '') + 
            p.LastName AS FullName,
            e.MonthlySalary  AS BaseSalary,
            s.TotalAbsenceDays,
            s.TotalDeduction
        FROM SalaryDeductionSummary s
        INNER JOIN Employees e ON s.EmployeeID = e.EmployeeID
        INNER JOIN People p ON e.PersonID = p.PersonID
        WHERE s.Month = @Month AND s.Year = @Year
        ORDER BY FullName;
    ";

            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

    }
}
