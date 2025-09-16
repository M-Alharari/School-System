using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsEmployeesAttendanceData
    {
        public static bool HasCompleteAttendance(int year, int month)
        {
            string query = @"
        SELECT COUNT(DISTINCT e.EmployeeID) AS TotalEmployees,
               (SELECT COUNT(DISTINCT ea.EmployeeID)
                FROM EmployeeAttendance ea
                WHERE YEAR(ea.AttendanceDate) = @Year
                  AND MONTH(ea.AttendanceDate) = @Month) AS EmployeesWithAttendance
        FROM Employees e
         "; // optional: filter only active employees

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
                        int employeesWithAttendance = reader.GetInt32(1);

                        return employeesWithAttendance == totalEmployees && totalEmployees > 0;
                    }
                }
            }

            return false;
        }

        public static int AddAttendance(int employeeID, bool isPresent, string absenceReason, string notes, int userID, DateTime attendanceDate)
        {
            int attendanceID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO EmployeeAttendance 
                    (EmployeeID, IsPresent, AbsenceReason, Notes, AttendanceDate, CreatedByUserID, CreatedDate)
                    VALUES (@EmployeeID, @IsPresent, @AbsenceReason, @Notes, @AttendanceDate, @UserID, GETDATE());
                    SELECT SCOPE_IDENTITY();
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@IsPresent", isPresent);
                cmd.Parameters.AddWithValue("@AbsenceReason", (object)absenceReason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttendanceDate", attendanceDate.Date);
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                object result = cmd.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int id))
                    attendanceID = id;
            }

            return attendanceID;
        }

        public static bool UpdateAttendance(int attendanceID, int employeeID, bool isPresent, string absenceReason, string notes, int userID, DateTime attendanceDate)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE EmployeeAttendance
                    SET EmployeeID = @EmployeeID,
                        IsPresent = @IsPresent,
                        AbsenceReason = @AbsenceReason,
                        Notes = @Notes,
                        AttendanceDate = @AttendanceDate,
                        ModifiedByUserID = @UserID,
                        ModifiedDate = GETDATE()
                    WHERE AttendanceID = @AttendanceID;
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@IsPresent", isPresent);
                cmd.Parameters.AddWithValue("@AbsenceReason", (object)absenceReason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttendanceDate", attendanceDate.Date);
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool GetAttendanceByID(int attendanceID, ref int employeeID, ref bool isPresent, ref string absenceReason, ref string notes, ref DateTime attendanceDate)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT EmployeeID, IsPresent, AbsenceReason, Notes, AttendanceDate
                    FROM EmployeeAttendance
                    WHERE AttendanceID = @AttendanceID
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employeeID = reader.GetInt32(0);
                        isPresent = reader.GetBoolean(1);
                        absenceReason = reader.IsDBNull(2) ? null : reader.GetString(2);
                        notes = reader.IsDBNull(3) ? null : reader.GetString(3);
                        attendanceDate = reader.GetDateTime(4);
                        return true;
                    }
                    return false;
                }
            }
        }

        public static DataTable GetAllAttendance()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT AttendanceID, EmployeeID, IsPresent, AbsenceReason, Notes, AttendanceDate, CreatedByUserID, CreatedDate 
                    FROM EmployeeAttendance
                    ORDER BY AttendanceDate DESC
                ";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAttendanceByEmployeeMonth(int employeeID, int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        AttendanceID,
                        EmployeeID,
                        CASE WHEN IsPresent = 1 THEN 'Present' ELSE 'Absent' END AS Status,
                        AbsenceReason,
                        Notes,
                        AttendanceDate,
                        DAY(AttendanceDate) AS DayOfMonth
                    FROM EmployeeAttendance
                    WHERE MONTH(AttendanceDate) = @Month AND YEAR(AttendanceDate) = @Year AND EmployeeID = @EmployeeID
                    ORDER BY AttendanceDate;
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        public static DataTable GetAttendanceByMonth(string yearMonth)
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        ea.AttendanceID,
                        ea.EmployeeID,
                        e.FullName,
                        ea.AttendanceDate,
                        DATENAME(WEEKDAY, ea.AttendanceDate) AS WeekDayName,
                        ea.Status,
                        ea.YearMonth
                    FROM EmployeeAttendance ea
                    INNER JOIN Employees e ON ea.EmployeeID = e.EmployeeID
                    WHERE ea.YearMonth = @YearMonth
                    ORDER BY ea.AttendanceDate ASC;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@YearMonth", yearMonth);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }

            return dt;
        }
        public static DataTable GetAttendanceByMonth(int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        EA.EmployeeID,
                        P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName AS FullName,
                        EA.AttendanceDate,
                        DATENAME(WEEKDAY, EA.AttendanceDate) AS DayOfWeek,
                        CASE WHEN EA.IsPresent = 1 THEN 'Yes' ELSE 'No' END AS AttendanceStatus,
                        EA.AbsenceReason,
                        DATENAME(MONTH, EA.AttendanceDate) AS MonthName,
                        FORMAT(EA.AttendanceDate, 'yyyy-MM') AS YearMonth
                    FROM 
                        EmployeeAttendance EA
                    INNER JOIN Employees E ON EA.EmployeeID = E.EmployeeID
                    INNER JOIN People P ON E.PersonID = P.PersonID
                    WHERE 
                        MONTH(EA.AttendanceDate) = @Month 
                        AND YEAR(EA.AttendanceDate) = @Year
                    ORDER BY EA.AttendanceDate;
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }
        public static bool Exists(int employeeID, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM EmployeeAttendance 
                    WHERE EmployeeID = @EmployeeID 
                      AND CAST(AttendanceDate AS DATE) = @AttendanceDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeID);
                    cmd.Parameters.AddWithValue("@AttendanceDate", date.Date);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public static bool DeleteAttendance(int attendanceID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "DELETE FROM EmployeeAttendance WHERE AttendanceID = @AttendanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool DoesAttendanceExist(int attendanceID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT COUNT(*) FROM EmployeeAttendance WHERE AttendanceID = @AttendanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
