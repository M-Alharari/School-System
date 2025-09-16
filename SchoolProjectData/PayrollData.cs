using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public class clsPayrollData
    {
        public static bool PayrollExists(int employeeID, int year, int month)
        {
            string query = @"SELECT COUNT(*) FROM PayrollPayments
                     WHERE EmployeeID=@EmployeeID
                     AND Year=@Year AND Month=@Month"; // ✅ correct

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);

                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public static DataTable GetPayrollByMonthYear(int year, int month)
        {
            DataTable dt = new DataTable();

            string query = @"
        SELECT 
            e.EmployeeID,
            CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName) AS FullName,
            e.MonthlySalary,
            ISNULL(SUM(sd.DeductionAmount), 0) AS TotalDeductions,
            (e.MonthlySalary - ISNULL(SUM(sd.DeductionAmount), 0)) AS NetSalary,
            ISNULL(pp.IsPaid, 0) AS IsPaid   -- 👈 Added from PayrollPayments
        FROM Employees e
        INNER JOIN People p 
            ON e.PersonID = p.PersonID
        LEFT JOIN EmployeeAttendance ea 
            ON e.EmployeeID = ea.EmployeeID
        LEFT JOIN SalaryDeductions sd 
            ON ea.AttendanceID = sd.AttendanceID
        LEFT JOIN PayrollPayments pp
            ON pp.EmployeeID = e.EmployeeID
           AND pp.Month = @Month
           AND pp.Year = @Year
        WHERE MONTH(ea.AttendanceDate) = @Month
          AND YEAR(ea.AttendanceDate) = @Year
        GROUP BY 
            e.EmployeeID, 
            CONCAT(p.FirstName, ' ', p.SecondName, ' ', p.ThirdName, ' ', p.LastName),
            e.MonthlySalary,
            pp.IsPaid   -- 👈 Added to GROUP BY
        ORDER BY e.EmployeeID;";

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Month", month);
                        cmd.Parameters.AddWithValue("@Year", year);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving payroll data: " + ex.Message);
            }

            return dt;
        }

        // ✅ Add Payroll Payment
        public static bool AddPayrollPayment(int employeeID, int year, int month,
                                        decimal grossSalary, decimal totalDeductions,
                                        decimal netSalary, int createdByUserID)
        {
            if (PayrollExists(employeeID, year, month))
                throw new Exception($"Payroll for EmployeeID={employeeID} in {month}/{year} already exists.");

            string query = @"
        INSERT INTO PayrollPayments 
        (EmployeeID, Year, Month, GrossSalary, TotalDeductions, NetSalary, IsPaid, PaidDate, CreatedDate, CreatedByUserID)
        VALUES (@EmployeeID, @Year, @Month, @GrossSalary, @TotalDeductions, @NetSalary, 0, NULL, GETDATE(), @CreatedByUserID);";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@GrossSalary", grossSalary);
                cmd.Parameters.AddWithValue("@TotalDeductions", totalDeductions);
                cmd.Parameters.AddWithValue("@NetSalary", netSalary);
                cmd.Parameters.AddWithValue("@CreatedByUserID", createdByUserID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // ✅ Mark payroll as paid
        public static bool MarkPayrollAsPaid(int employeeID, int year, int month)
        {
            string query = @"
                UPDATE PayrollPayments
                SET IsPaid = 1, PaidDate = GETDATE()
                WHERE EmployeeID = @EmployeeID AND Year = @Year AND Month = @Month;";

            try
            {
                using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Month", month);

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating payroll payment: " + ex.Message);
            }
        }
    }
}
