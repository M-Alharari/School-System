using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsStudentBehaviour
    {
        public enum BehaviourType
        {
            Positive = 1,  // matches ID in BehaviourTypes table
            Negative = 2,
            Neutral = 3
        }
        public enum BehaviourCategory
        {
            Discipline = 1,
            Academic = 2,
            Social = 3,
            Attendance = 4
        }
        public enum BehaviourSeverity
        {
            Minor = 1,
            Moderate = 2,
            Major = 3
        }
        public enum BehaviourAction
        {
            Warning = 1,
            Detention = 2,
            Reward = 3,
            Counseling = 4,
            ParentCall = 5,
            Suspension = 6,
            Commendation = 7
        }

        public int BehaviourID { get; set; }
        public int EnrolledID { get; set; }
        public int BehaviourTypeID { get; set; }
        public int CategoryID { get; set; } // Optional if used
        public int SeverityLevelID { get; set; }
        public int ActionTakenID { get; set; }
        public string Description { get; set; }
        public int RecordedBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public clsStudentBehaviour() { }

        #region CRUD Methods

        public int Add()
        {
            return StudentBehavioursData.AddBehaviour(
                EnrolledID,
                BehaviourTypeID,
                CategoryID,
                SeverityLevelID,
                ActionTakenID,
                Description,
                RecordedBy,
                CreatedDate
            );
        }

        public void Update()
        {
            StudentBehavioursData.UpdateBehaviour(
                BehaviourID,
                EnrolledID,
                BehaviourTypeID,
                CategoryID,
                SeverityLevelID,
                ActionTakenID,
                Description,
                RecordedBy,
                CreatedBy
            );
        }

        //public void Delete(int behaviourID)
        //{
        //    StudentBehavioursData.DeleteBehaviour(behaviourID);
        //}

        public static DataTable GetByEnrolledID(int enrolledID)
        {
            return StudentBehavioursData.GetBehavioursByEnrolledID(enrolledID);
        }

        public static DataTable GetBehaviourByID(int behaviourID)
        {
            return StudentBehavioursData.GetBehaviourByID(behaviourID);
        }

        #endregion

        #region ComboBox Helpers

        public static DataTable GetBehaviourTypes()
        {
            return StudentBehavioursData.GetBehaviourTypes();
        }

        public static DataTable GetSeverityLevels()
        {
            return StudentBehavioursData.GetSeverityLevels();
        }

        public static DataTable GetActions()
        {
            return StudentBehavioursData.GetActions();
        }
        public static DataTable GetAll()
        {
            return StudentBehavioursData.GetAll();
        }
        public static DataTable GetEnrollmentBehaviourSummary()
        {
            return StudentBehavioursData.GetEnrollmentBehaviourSummary();
        }
        public static DataTable GetEnrollmentBehaviourSummaryforterms(int studendid)
        {
            return StudentBehavioursData.GetEnrollmentBehaviourSummaryForTerms(studendid);
        }
        public static DataTable GetGradeClassBehaviourSummary(int gradeID, int classID, int termID)
        {
            return StudentBehavioursData.GetGradeClassBehaviourSummary(gradeID, classID, termID);
        }
        // Get all behaviour counts for a class/grade/term
        public static DataTable GetGradeClassBehaviourSummarys(int gradeID, int classID, int termID)
        {
            return StudentBehavioursData.GetBehaviourSummary(gradeID, classID, termID);
        }

        // Get behaviours for a specific student/enrollment
        public static DataRow GetBehaviourCountsByEnrollment(int enrollmentID)
        {
            return StudentBehavioursData.GetBehaviourCountsByEnrollment(enrollmentID);
        }
        // Get all behaviours for an enrollment
        public static DataTable GetBehavioursByEnrollment(int BehaviourID)
        {
            return StudentBehavioursData.GetBehavioursByEnrollment(BehaviourID);
        }

        // Optionally: map IDs to enums for easier display
        public static DataTable GetBehavioursByEnrollmentWithEnums(int behaviourID)
        {
            var dt = StudentBehavioursData.GetBehavioursByEnrollment(behaviourID);
            if (!dt.Columns.Contains("FullName"))
                dt.Columns.Add("FullName", typeof(string));

            if (!dt.Columns.Contains("CreatedDateStr"))
                dt.Columns.Add("CreatedDateStr", typeof(string));

            if (!dt.Columns.Contains("BehaviourTypeName"))
                dt.Columns.Add("BehaviourTypeName", typeof(string));

            if (!dt.Columns.Contains("CategoryName"))
                dt.Columns.Add("CategoryName", typeof(string));

            if (!dt.Columns.Contains("SeverityName"))
                dt.Columns.Add("SeverityName", typeof(string));

            if (!dt.Columns.Contains("ActionName"))
                dt.Columns.Add("ActionName", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                row["BehaviourTypeName"] = ((BehaviourType)Convert.ToInt32(row["BehaviourTypeID"])).ToString();
                row["CategoryName"] = ((BehaviourCategory)Convert.ToInt32(row["CategoryID"])).ToString();
                row["SeverityName"] = ((BehaviourSeverity)Convert.ToInt32(row["SeverityLevelID"])).ToString();
                row["ActionName"] = ((BehaviourAction)Convert.ToInt32(row["ActionTakenID"])).ToString();

                

                row["CreatedDateStr"] = row["CreatedDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd")
                    : "-";
            }


            return dt;
        }

        #endregion
    }
}
