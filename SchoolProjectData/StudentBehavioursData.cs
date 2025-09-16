using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectData
{
    public static class StudentBehavioursData

    {
        public static DataTable GetBehavioursByEnrollment(int BehaviourID)
        {
            string sql = @"
               SELECT 
    sb.BehaviourID,
     
    sb.DateRecorded,
    sb.BehaviourTypeID,
    sb.CategoryID,
    sb.Description,
    sb.SeverityLevelID,
    sb.ActionTakenID,
    sb.RecordedBy,
    sb.CreatedDate,
    
    -- Compose full name from enrollment -> student -> person
    p.FirstName + ' ' 
    + ISNULL(p.SecondName,'') + ' ' 
    + ISNULL(p.ThirdName,'') + ' ' 
    + ISNULL(p.LastName,'') AS FullName
FROM StudentBehaviours sb
INNER JOIN Enrollments e ON sb.EnrollmentID = e.EnrollmentID
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People p ON s.PersonID = p.PersonID
WHERE sb.BehaviourID = @BehaviourID
ORDER BY sb.DateRecorded DESC;
";

            using (var conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@BehaviourID", BehaviourID);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
                return dt;
            }
        }

        // Example: summary for grade/class/term
        public static DataTable GetBehaviourSummary(int gradeID, int classID, int termID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
                    SELECT 
                        SUM(CASE WHEN bs.Points > 0 THEN 1 ELSE 0 END) AS PositiveBehaviours,
                        SUM(CASE WHEN bs.Points < 0 THEN 1 ELSE 0 END) AS NegativeBehaviours
                    FROM StudentBehaviours sb
                    INNER JOIN Enrollments e ON sb.StudentID = e.StudentID
                    INNER JOIN BehaviourSeverity bs ON sb.SeverityID = bs.SeverityID
                    WHERE e.GradeID = @GradeID AND e.ClassID = @ClassID AND e.TermID = @TermID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@GradeID", gradeID);
                    cmd.Parameters.AddWithValue("@ClassID", classID);
                    cmd.Parameters.AddWithValue("@TermID", termID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        // Example: get behaviour counts for one student/enrollment
        public static DataRow GetBehaviourCountsByEnrollment(int enrollmentID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
                    SELECT 
                        SUM(CASE WHEN bs.Points > 0 THEN 1 ELSE 0 END) AS PositiveBehaviours,
                        SUM(CASE WHEN bs.Points < 0 THEN 1 ELSE 0 END) AS NegativeBehaviours,
                        SUM(CASE WHEN bs.Points > 0 AND sb.SeverityID = 1 THEN 1 ELSE 0 END) AS MinorPositive,
                        SUM(CASE WHEN bs.Points > 0 AND sb.SeverityID = 2 THEN 1 ELSE 0 END) AS ModeratePositive,
                        SUM(CASE WHEN bs.Points > 0 AND sb.SeverityID = 3 THEN 1 ELSE 0 END) AS SeverePositive,
                        SUM(CASE WHEN bs.Points < 0 AND sb.SeverityID = 1 THEN 1 ELSE 0 END) AS MinorNegative,
                        SUM(CASE WHEN bs.Points < 0 AND sb.SeverityID = 2 THEN 1 ELSE 0 END) AS ModerateNegative,
                        SUM(CASE WHEN bs.Points < 0 AND sb.SeverityID = 3 THEN 1 ELSE 0 END) AS SevereNegative
                    FROM StudentBehaviours sb
                    INNER JOIN BehaviourSeverity bs ON sb.SeverityID = bs.SeverityID
                    WHERE sb.StudentID = (SELECT StudentID FROM Enrollments WHERE EnrollmentID = @EnrollmentID)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrollmentID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                    }
                }
            }
        }
        public static DataTable GetGradeClassBehaviourSummary(int gradeID, int classID, int termID)
        {
            DataTable dt = new DataTable();

            string sql = @"
                SELECT 
                    e.EnrollmentID,
                    e.TermID,
                    t.StartDate AS TermStart,
                    t.EndDate AS TermEnd,
                    SUM(CASE WHEN sb.BehaviourTypeID = 1 THEN 1 ELSE 0 END) AS PositiveBehaviours,
                    SUM(CASE WHEN sb.BehaviourTypeID = 2 THEN 1 ELSE 0 END) AS NegativeBehaviours
                FROM Enrollments e
                INNER JOIN StudentBehaviours sb ON e.EnrollmentID = sb.EnrollmentID
                INNER JOIN Terms t ON e.TermID = t.TermID
                INNER JOIN Grades g ON e.GradeID = g.GradeID
                INNER JOIN Classes c ON e.ClassID = c.ClassID
                WHERE g.GradeID = @GradeID
                  AND c.ClassID = @ClassID
                  AND e.TermID = @TermID
                GROUP BY 
                    e.EnrollmentID, e.TermID, t.StartDate, t.EndDate
                ORDER BY 
                    e.EnrollmentID;
            ";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@GradeID", gradeID);
                cmd.Parameters.AddWithValue("@ClassID", classID);
                cmd.Parameters.AddWithValue("@TermID", termID);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return dt;
        }
        public static DataTable GetEnrollmentBehaviourSummaryForTerms(int studentId)
        {
            DataTable dt = new DataTable();
            string query = @"
SELECT 
    t.StartDate AS TermStart,
    t.EndDate AS TermEnd,
    SUM(CASE WHEN sb.BehaviourTypeID = 1 THEN 1 ELSE 0 END) AS PositiveBehaviours,
    SUM(CASE WHEN sb.BehaviourTypeID = 2 THEN 1 ELSE 0 END) AS NegativeBehaviours
FROM Enrollments e
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People p ON s.PersonID = p.PersonID
INNER JOIN Terms t ON e.TermID = t.TermID
INNER JOIN StudentBehaviours sb ON e.EnrollmentID = sb.EnrollmentID
WHERE s.StudentID = @StudentID
GROUP BY 
    s.StudentID,
    p.FirstName, p.SecondName, p.ThirdName, p.LastName,
    e.EnrollmentID, e.TermID, t.StartDate, t.EndDate
ORDER BY 
    s.StudentID, e.EnrollmentID;";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@StudentID", studentId);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching enrollment behaviour summary.", ex);
                }
            }

            return dt;
        }

        public static DataTable GetEnrollmentBehaviourSummary()
        {
            DataTable dt = new DataTable();
            string query = @"
SELECT 
    s.StudentID,
    (p.FirstName + ' ' + ISNULL(p.SecondName, '') + ' ' + ISNULL(p.ThirdName, '') + ' ' + p.LastName) AS FullName,
    e.EnrollmentID,
    e.TermID,
    t.StartDate AS TermStart,
    t.EndDate AS TermEnd,
    SUM(CASE WHEN sb.BehaviourTypeID = 1 THEN 1 ELSE 0 END) AS PositiveBehaviours,
    SUM(CASE WHEN sb.BehaviourTypeID = 2 THEN 1 ELSE 0 END) AS NegativeBehaviours
FROM Enrollments e
INNER JOIN Students s ON e.StudentID = s.StudentID
INNER JOIN People p ON s.PersonID = p.PersonID
INNER JOIN Terms t ON e.TermID = t.TermID
INNER JOIN StudentBehaviours sb ON e.EnrollmentID = sb.EnrollmentID
GROUP BY 
    s.StudentID,
    p.FirstName, p.SecondName, p.ThirdName, p.LastName,
    e.EnrollmentID, e.TermID, t.StartDate, t.EndDate
ORDER BY 
    s.StudentID, e.EnrollmentID;";

            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    dt.Load(reader);
                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching enrollment behaviour summary.", ex);
                }
            }

            return dt;
        }
        public static DataTable GetAll()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT CategoryID, Name FROM BehaviourCategories ORDER BY Name";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        #region CRUD
        public static DataTable GetBehavioursByEnrolledID(int enrolledID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"
SELECT 
    B.BehaviourID,
    B.EnrollmentID,
    BT.Name AS BehaviourType,
    BC.Name AS Category,         -- ← Added category
    BS.Name AS Severity,
    BA.Name AS ActionTaken,
    B.Description,
    B.RecordedBy 
    
FROM StudentBehaviours B
INNER JOIN BehaviourTypes BT ON B.BehaviourTypeID = BT.BehaviourTypeID
INNER JOIN BehaviourCategories BC ON B.CategoryID = BC.CategoryID  -- ← Join categories
INNER JOIN BehaviourSeverity BS ON B.SeverityLevelID = BS.SeverityLevelID
INNER JOIN BehaviourActions BA ON B.ActionTakenID = BA.ActionID
WHERE B.EnrollmentID = @EnrollmentID
ORDER BY B.BehaviourID;

";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrolledID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int AddBehaviour(int enrolledID, int behaviourTypeID, int categoryID,
                                       int severityLevelID, int actionID, string description,
                                       int recordedBy, DateTime CreatedDate)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"INSERT INTO StudentBehaviours 
                               (EnrollmentID, BehaviourTypeID, CategoryID, SeverityLevelID, ActionTakenID, Description, RecordedBy, CreatedDate)
                               VALUES
                               (@EnrollmentID, @BehaviourTypeID, @CategoryID, @SeverityLevelID, @ActionTakenID, @Description, @RecordedBy, @CreatedDate);
                               SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@EnrollmentID", enrolledID);
                    cmd.Parameters.AddWithValue("@BehaviourTypeID", behaviourTypeID);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@SeverityLevelID", severityLevelID);
                    cmd.Parameters.AddWithValue("@ActionTakenID", actionID);
                    cmd.Parameters.AddWithValue("@Description", description ?? "");
                    cmd.Parameters.AddWithValue("@RecordedBy", recordedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", CreatedDate);

                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static void UpdateBehaviour(int behaviourID, int enrolledID, int behaviourTypeID, int categoryID,
                                           int severityLevelID, int actionID, string description,
                                           int recordedBy, int createdBy)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"UPDATE StudentBehaviours
                               SET EnrolledID=@EnrolledID,
                                   BehaviourTypeID=@BehaviourTypeID,
                                   CategoryID=@CategoryID,
                                   SeverityLevelID=@SeverityLevelID,
                                   ActionTakenID=@ActionTakenID,
                                   Description=@Description,
                                   RecordedBy=@RecordedBy,
                                   CreatedBy=@CreatedBy
                               WHERE BehaviourID=@BehaviourID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BehaviourID", behaviourID);
                    cmd.Parameters.AddWithValue("@EnrolledID", enrolledID);
                    cmd.Parameters.AddWithValue("@BehaviourTypeID", behaviourTypeID);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryID);
                    cmd.Parameters.AddWithValue("@SeverityLevelID", severityLevelID);
                    cmd.Parameters.AddWithValue("@ActionTakenID", actionID);
                    cmd.Parameters.AddWithValue("@Description", description ?? "");
                    cmd.Parameters.AddWithValue("@RecordedBy", recordedBy);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion

        #region Get Data

        public static DataTable GetBehaviourByID(int behaviourID)
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = @"SELECT BehaviourID, EnrollmentID, BehaviourTypeID, CategoryID, SeverityLevelID, ActionTakenID, Description 
                               FROM StudentBehaviours
                               WHERE BehaviourID=@BehaviourID";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BehaviourID", behaviourID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static DataTable GetBehaviourTypes()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT BehaviourTypeID, Name FROM BehaviourTypes ORDER BY Name";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetSeverityLevels()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT SeverityLevelID, Name FROM BehaviourSeverity ORDER BY Name";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public static DataTable GetActions()
        {
            using (SqlConnection conn = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string sql = "SELECT ActionID, Name FROM BehaviourActions ORDER BY Name";
                SqlDataAdapter da = new SqlDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        #endregion
    }
}
