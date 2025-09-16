using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsExamScore
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ScoreID { get; private set; } = -1;
        public int EnrollmentID { get; set; }
        public int SubjectID { get; set; }
        public int ExamTypeID { get; set; }
        public int TermID { get; set; }  // add TermID

        public int CreatedByUserID { get; set; }  // public set to assign before saving
        public DateTime CreatedAt { get; private set; }
        public int? ModifiedByUserID { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        public clsExamScore() { }

        // Save both Test (ExamTypeID=1) and Exam (ExamTypeID=2)
        public bool SaveAllExamTypes()
        {
            if (EnrollmentID <= 0 || SubjectID <= 0 || TermID <= 0 || CreatedByUserID <= 0)
                throw new Exception("Required fields not set.");

            // --- Create parent Score record once ---
            if (ScoreID == -1)
            {
                ScoreID = clsScoreData.AddScore(EnrollmentID, SubjectID, 0, TermID, CreatedByUserID);
                if (ScoreID == -1) return false;
            }

            // --- Save Test ---
            int testScoreID = clsExamScoreData.AddNewExamScore(EnrollmentID, SubjectID, 1, CreatedByUserID, DateTime.Now);

            // --- Save Exam ---
            int examScoreID = clsExamScoreData.AddNewExamScore(EnrollmentID, SubjectID, 2, CreatedByUserID, DateTime.Now);

            return testScoreID != -1 && examScoreID != -1;
        }

        public static DataTable GetScoresByEnrollmentID(int enrollmentID)
        {
            return clsExamScoreData.GetScoresByEnrollmentID(enrollmentID);
        }

        public static bool Delete(int scoreID)
        {
            return clsExamScoreData.DeleteExamScore(scoreID);
        }

        public static bool Exists(int scoreID)
        {
            return clsExamScoreData.ExamScoreExists(scoreID);
        }

        // -------------------- Put it here --------------------
     


     
    }
}
