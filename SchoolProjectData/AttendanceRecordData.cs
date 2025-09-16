using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectData
{
    public class clsAttendanceRecordData
    {

        static clsAttendanceRecordData() { }




        public static bool GetAttendanceRecordByAttendanceID(int AttendanceID,
            ref int PersonID, ref DateTime AttendanceDate, ref bool IsPresent,
            ref TimeSpan? CheckInTime, ref TimeSpan? CheckOutTime, ref string Notes,
            ref int? RecordedBy, ref DateTime? CreatedAt)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
                using (SqlCommand cmd = new SqlCommand("AttendanceRecords_GetAttendanceRecordByAttendanceID", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AttendanceID", AttendanceID);

                    connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            AttendanceID = Convert.ToInt32(reader["AttendanceID"]);
                            PersonID = Convert.ToInt32(reader["PersonID"]);
                            AttendanceDate = Convert.ToDateTime(reader["AttendanceDate"]);
                            IsPresent = Convert.ToBoolean(reader["IsPresent"]);

                            CheckInTime = reader["CheckInTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["CheckInTime"];
                            CheckOutTime = reader["CheckOutTime"] == DBNull.Value ? (TimeSpan?)null : (TimeSpan)reader["CheckOutTime"];
                            Notes = reader["Notes"] == DBNull.Value ? null : reader["Notes"].ToString();
                            RecordedBy = reader["RecordedBy"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["RecordedBy"]);
                            CreatedAt = reader["CreatedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["CreatedAt"]);

                            IsFound = true;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                //clsErrorEventLog.LogError(ex.Message);
            }

            return IsFound;
        }
        public static DataTable GetSummaryForTeacher(int teacherID)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TotalDays", typeof(int));
            dt.Columns.Add("DaysPresent", typeof(int));
            dt.Columns.Add("LastDayPresent", typeof(string));
            dt.Columns.Add("AttendancePercentage", typeof(double));

            string query = @"
       SELECT 
    COUNT(*) AS TotalDays,
    SUM(CASE WHEN IsPresent = 1 THEN 1 ELSE 0 END) AS DaysPresent,
    MAX(CASE WHEN IsPresent = 1 THEN AttendanceDate END) AS LastDayPresent
FROM EmployeeAttendance
WHERE EmployeeID = @EmployeeID;

    ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int total = reader["TotalDays"] != DBNull.Value ? Convert.ToInt32(reader["TotalDays"]) : 0;
                            int present = reader["DaysPresent"] != DBNull.Value ? Convert.ToInt32(reader["DaysPresent"]) : 0;
                            string lastDay = reader["LastDayPresent"] != DBNull.Value ? Convert.ToDateTime(reader["LastDayPresent"]).ToShortDateString() : "-";

                            double percentage = (total > 0) ? (present * 100.0 / total) : 0;

                            dt.Rows.Add(total, present, lastDay, percentage);
                        }
                    }
                }
            }

            return dt;
        }
        public static DataTable GetTeacherAttendanceSummary(int teacherID)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("FullName", typeof(string)); // ✅ هذا هو العمود الناقص
            dt.Columns.Add("TotalDays", typeof(int));
            dt.Columns.Add("DaysPresent", typeof(int));
            dt.Columns.Add("LastDayPresent", typeof(string));
            dt.Columns.Add("AttendancePercentage", typeof(double));


            string query = @"
            SELECT 
    CONCAT(P.FirstName, ' ', 
           ISNULL(P.SecondName, ''), ' ',
           ISNULL(P.ThirdName, ''), ' ',
           P.LastName) AS FullName,
    COUNT(*) AS TotalDays,
    SUM(CASE WHEN A.IsPresent = 1 THEN 1 ELSE 0 END) AS DaysPresent,
    SUM(CASE WHEN A.IsPresent = 0 THEN 1 ELSE 0 END) AS DaysAbsent,
    MAX(CASE WHEN A.IsPresent = 1 THEN A.AttendanceDate END) AS LastDayPresent
FROM EmployeeAttendance A
INNER JOIN Employees E ON E.EmployeeID = A.EmployeeID
INNER JOIN People P ON E.PersonID = P.PersonID
WHERE A.EmployeeID = @EmployeeID   -- replace with parameter or specific employee
GROUP BY P.FirstName, P.SecondName, P.ThirdName, P.LastName;



            ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@EmployeeID", teacherID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            string fullName = reader["FullName"].ToString(); // ✅ خذ الاسم من النتيجة
                            int total = reader["TotalDays"] != DBNull.Value ? Convert.ToInt32(reader["TotalDays"]) : 0;
                            int present = reader["DaysPresent"] != DBNull.Value ? Convert.ToInt32(reader["DaysPresent"]) : 0;
                            string lastDay = reader["LastDayPresent"] != DBNull.Value ? Convert.ToDateTime(reader["LastDayPresent"]).ToShortDateString() : "-";
                            double percentage = (total > 0) ? (present * 100.0 / total) : 0;

                            dt.Rows.Add(fullName, total, present, lastDay, percentage); // ✅ اضف الاسم مع الباقي

                        }
                    }
                }
            }

            return dt;
        }

        public static int AddNewAttendanceRecord(int AttendanceID,
      int PersonID, DateTime? AttendanceDate, bool IsPresent,
      TimeSpan? CheckInTime, TimeSpan? CheckOutTime, string Notes,
      int? RecordedBy, DateTime? CreatedAt)
        {


            try
            {




                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

                {




                    using (SqlCommand cmd = new SqlCommand("AttendanceRecords_InsertNewAttendanceRecord", connection))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        connection.Open();

                        cmd.Parameters.AddWithValue("@AttendanceID", AttendanceID);

                        cmd.Parameters.AddWithValue("@PersonID", PersonID);

                        cmd.Parameters.AddWithValue("@AttendanceDate", AttendanceDate);

                        cmd.Parameters.AddWithValue("@IsPresent", IsPresent);

                        cmd.Parameters.AddWithValue("@CheckInTime", (object)CheckInTime ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@CheckOutTime", (object)CheckOutTime ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Notes", (object)Notes ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@RecordedBy", (object)RecordedBy ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@CreatedAt", (object)CreatedAt ?? DBNull.Value);

                        SqlParameter outputIdParam = new SqlParameter("@NewAttendanceID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };

                        cmd.Parameters.Add(outputIdParam);
                        cmd.ExecuteNonQuery();
                        int newAttendanceID = (int)cmd.Parameters["@NewAttendanceID"].Value;
                        return newAttendanceID;

                    }

                }

            }

            catch (SqlException ex)
            {

                //clsErrorEventLog.LogError(ex.Message);
                return 0;
            }
        }


        public static bool UpdateAttendanceRecord(int AttendanceID,
      int PersonID, DateTime AttendanceDate, bool IsPresent,
      TimeSpan? CheckInTime, TimeSpan? CheckOutTime, string Notes,
      int? RecordedBy, DateTime? CreatedAt)
        {
            bool IsRowsAffected = false;


            try
            {




                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

                {




                    using (SqlCommand cmd = new SqlCommand("AttendanceRecords_UpdateAttendanceRecord", connection))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        connection.Open();

                        cmd.Parameters.AddWithValue("@AttendanceID", AttendanceID);

                        cmd.Parameters.AddWithValue("@PersonID", PersonID);

                        cmd.Parameters.AddWithValue("@AttendanceDate", AttendanceDate);

                        cmd.Parameters.AddWithValue("@IsPresent", IsPresent);

                        cmd.Parameters.AddWithValue("@CheckInTime", (object)CheckInTime ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@CheckOutTime", (object)CheckOutTime ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@Notes", (object)Notes ?? DBNull.Value);

                        cmd.Parameters.AddWithValue("@RecordedBy", (object)RecordedBy ?? DBNull.Value);

                        //cmd.Parameters.AddWithValue("@CreatedAt", (object)CreatedAt ?? DBNull.Value);
                        IsRowsAffected = (cmd.ExecuteNonQuery() > 0);



                    }

                }

            }

            catch (SqlException ex)
            {

                //clsErrorEventLog.LogError(ex.Message);
                IsRowsAffected = false;
            }
            return IsRowsAffected;
        }


        public static bool DeleteAttendanceRecord(int AttendanceID)
        {
            bool IsRowsAffected = false;


            try
            {




                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

                {




                    using (SqlCommand cmd = new SqlCommand("AttendanceRecords_DeleteAttendanceRecord", connection))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        connection.Open();

                        cmd.Parameters.AddWithValue("@AttendanceID", AttendanceID);
                        IsRowsAffected = (cmd.ExecuteNonQuery() > 0);



                    }

                }

            }

            catch (SqlException ex)
            {

                //clsErrorEventLog.LogError(ex.Message);
                IsRowsAffected = false;
            }
            return IsRowsAffected;
        }

        public static bool IsAttendanceRecordExists(int AttendanceID)
        {

            bool IsFound = false;



            try
            {


                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

                {



                    using (SqlCommand cmd = new SqlCommand("AttendanceRecords_CheckAttendanceRecordExists", connection))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        connection.Open();

                        cmd.Parameters.AddWithValue("@AttendanceID", AttendanceID);

                        SqlParameter returnParameter = new SqlParameter("@ReturnVal", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.ReturnValue
                        };

                        cmd.Parameters.Add(returnParameter);
                        cmd.ExecuteNonQuery();

                        IsFound = (int)returnParameter.Value == 1;



                    }

                }

            }

            catch (SqlException ex)
            {

                //clsErrorEventLog.LogError(ex.Message);
                IsFound = false;
            }
            return IsFound;
        }

        public static DataTable GetAllAttendanceRecords()
        {
            DataTable dt = new DataTable();



            try
            {




                using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

                {




                    using (SqlCommand cmd = new SqlCommand("AttendanceRecords_GetAllAttendanceRecords", connection))

                    {

                        cmd.CommandType = CommandType.StoredProcedure;


                        connection.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())

                        {

                            if (reader.HasRows)
                                dt.Load(reader);

                            else
                                dt = null;

                        }

                    }

                }

            }

            catch (SqlException ex)
            {

                //clsErrorEventLog.LogError(ex.Message);
                dt = null;
            }
            return dt;
        }
    }
}
