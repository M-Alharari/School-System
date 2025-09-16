using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsStudentsAttendanceData
    {
        public static int AddAttendance(int studentID, bool isPresent, string absenceReason, string notes, int userID, DateTime attendanceDate)
        {
            int attendanceID = -1;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    INSERT INTO StudentAttendance 
                    (StudentID, IsPresent, AbsenceReason, Notes, AttendanceDate, CreatedByUserID, CreatedDate)
                    VALUES (@StudentID, @IsPresent, @AbsenceReason, @Notes, @AttendanceDate, @UserID, GETDATE());
                    SELECT SCOPE_IDENTITY();
                ";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
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

        public static bool UpdateAttendance(int attendanceID, int studentID, bool isPresent, string absenceReason, string notes, int userID, DateTime attendanceDate)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    UPDATE StudentAttendance
                    SET StudentID = @StudentID,
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
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@IsPresent", isPresent);
                cmd.Parameters.AddWithValue("@AbsenceReason", (object)absenceReason ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", (object)notes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AttendanceDate", attendanceDate.Date);
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public static bool GetAttendanceByID(int attendanceID, ref int studentID, ref bool isPresent, ref string absenceReason, ref string notes, ref DateTime attendanceDate)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT StudentID, IsPresent, AbsenceReason, Notes, AttendanceDate
                    FROM StudentAttendance
                    WHERE AttendanceID = @AttendanceID
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        studentID = reader.GetInt32(0);
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
                    SELECT AttendanceID, StudentID, IsPresent, AbsenceReason, Notes, AttendanceDate, CreatedByUserID, CreatedDate, ModifiedByUserID, ModifiedDate
                    FROM StudentAttendance
                    ORDER BY AttendanceDate DESC
                ";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.Fill(dt);
            }
            return dt;
        }

        public static DataTable GetAttendanceByStudentMonth(int studentID, int month, int year)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT 
                        AttendanceID,
                        StudentID,
                        CASE WHEN IsPresent = 1 THEN 'Present' ELSE 'Absent' END AS Status,
                        AbsenceReason,
                        Notes,
                        AttendanceDate,
                        DAY(AttendanceDate) AS DayOfMonth
                    FROM StudentAttendance
                    WHERE MONTH(AttendanceDate) = @Month AND YEAR(AttendanceDate) = @Year AND StudentID = @StudentID
                    ORDER BY AttendanceDate;
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@StudentID", studentID);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
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
                        SA.StudentID,
                        P.FirstName + ' ' + P.SecondName + ' ' + P.ThirdName + ' ' + P.LastName AS FullName,
                        SA.AttendanceDate,
                        DATENAME(WEEKDAY, SA.AttendanceDate) AS DayOfWeek,
                        CASE WHEN SA.IsPresent = 1 THEN 'Yes' ELSE 'No' END AS AttendanceStatus,
                        SA.AbsenceReason,
                        DATENAME(MONTH, SA.AttendanceDate) AS MonthName,
                        FORMAT(SA.AttendanceDate, 'yyyy-MM') AS YearMonth
                    FROM 
                        StudentAttendance SA
                    INNER JOIN Students S ON SA.StudentID = S.StudentID
                    INNER JOIN People P ON S.PersonID = P.PersonID
                    WHERE 
                        MONTH(SA.AttendanceDate) = @Month 
                        AND YEAR(SA.AttendanceDate) = @Year
                    ORDER BY SA.AttendanceDate;
                ";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Month", month);
                cmd.Parameters.AddWithValue("@Year", year);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        public static bool Exists(int studentID, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM StudentAttendance 
                    WHERE StudentID = @StudentID 
                      AND CAST(AttendanceDate AS DATE) = @AttendanceDate";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
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
                string query = "DELETE FROM StudentAttendance WHERE AttendanceID = @AttendanceID";
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
                string query = "SELECT COUNT(*) FROM StudentAttendance WHERE AttendanceID = @AttendanceID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AttendanceID", attendanceID);
                conn.Open();
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}
