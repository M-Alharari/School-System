using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class clsTimeTableData
    { /// <summary>
      /// Checks if a teacher is already booked for a specific day and period.
      /// Returns true if booked, false if free.
      /// </summary>
        public static bool IsTeacherBooked(int teacherID, DayOfWeek day, int period)
        {
            bool isBooked = false;

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
    SELECT COUNT(*) 
    FROM TimeTable
    WHERE TeacherID = @TeacherID
      AND DayOfWeek = @Day
      AND Period = @Period";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                    cmd.Parameters.AddWithValue("@Day", (int)day); // ✅ correct: integer

                    cmd.Parameters.AddWithValue("@Period", period);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    isBooked = count > 0;
                }
            }

            return isBooked;
        }
    
        private static string connectionString = @"YourConnectionStringHere";

        /// <summary>
        /// Adds a single timetable slot for a grade, class, subject, teacher, period, and day.
        /// </summary>
        public static bool AddTimeTableSlot(int gradeID, int classID, int subjectID, int teacherID, int period, DayOfWeek day)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO TimeTable
                                 (GradeID, ClassID, SubjectID, TeacherID, Period, DayOfWeek)
                                 VALUES (@GradeID, @ClassID, @SubjectID, @TeacherID, @Period, @DayOfWeek)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@SubjectID", subjectID);
                cmd.Parameters.AddWithValue("@TeacherID", teacherID);
                cmd.Parameters.AddWithValue("@Period", period);
                cmd.Parameters.AddWithValue("@DayOfWeek", (int)day); // Monday = 1, Sunday = 0

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Retrieves the timetable for a specific grade and class.
        /// </summary>
        public static DataTable GetTimeTable(int gradeID, int classID)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"SELECT 
    tt.*,
    s.SubjectName,
    p.FirstName + ' ' + p.SecondName + ' ' + p.ThirdName + ' ' + p.LastName AS TeacherName
FROM TimeTable tt
INNER JOIN Subjects s ON tt.SubjectID = s.SubjectID
INNER JOIN Teachers t ON tt.TeacherID = t.TeacherID
INNER JOIN Employees e ON t.EmployeeID = e.EmployeeID
INNER JOIN People p ON e.PersonID = p.PersonID
WHERE tt.GradeID = @GradeID AND tt.ClassID = @ClassID
ORDER BY tt.DayOfWeek, tt.Period
";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
            }
            return dt;
        }

        /// <summary>
        /// Deletes all timetable slots for a specific grade and class.
        /// Useful before regenerating the timetable.
        /// </summary>
        public static bool DeleteTimeTableForClass(int gradeID, int classID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM TimeTable WHERE GradeID = @GradeID AND ClassID = @ClassID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);

                conn.Open();
                return cmd.ExecuteNonQuery() >= 0;
            }
        }

        /// <summary>
        /// Optional: Add break slots automatically.
        /// Example: insert a “break” as a subject with TeacherID = NULL
        /// </summary>
        public static bool AddBreakSlot(int gradeID, int classID, int period, DayOfWeek day, string breakName = "Break")
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"INSERT INTO TimeTable
                                 (GradeID, ClassID, SubjectID, TeacherID, Period, DayOfWeek)
                                 VALUES (@GradeID, @ClassID, @SubjectID, @TeacherID, @Period, @DayOfWeek)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@SubjectID", DBNull.Value); // No subject
                cmd.Parameters.AddWithValue("@TeacherID", DBNull.Value); // No teacher
                cmd.Parameters.AddWithValue("@Period", period);
                cmd.Parameters.AddWithValue("@DayOfWeek", (int)day);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}
