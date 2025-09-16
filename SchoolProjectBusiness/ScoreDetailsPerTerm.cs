using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsScoreDetailsPerTerm
    {
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        public int ScoreDetailID { get; set; }
        public int EnrollmentID { get; set; }
        public int SubjectID { get; set; }
        public int TermID { get; set; }
        public decimal TestScore { get; set; }
        public decimal ExamScore { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }

        // AddNew constructor
        public clsScoreDetailsPerTerm()
        {
            ScoreDetailID = -1;
            EnrollmentID = -1;
            SubjectID = -1;
            TermID = -1;
            TestScore = 0;
            ExamScore = 0;

            Mode = enMode.AddNew;
        }

        // Update constructor
        private clsScoreDetailsPerTerm(int scoreDetailID, int enrollmentID, int subjectID, int termID, decimal testScore, decimal examScore)
        {
            ScoreDetailID = scoreDetailID;
            EnrollmentID = enrollmentID;
            SubjectID = subjectID;
            TermID = termID;
            TestScore = testScore;
            ExamScore = examScore;
            Mode = enMode.Update;
        }

        // Save method
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewScore())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateScore();
            }

            return false;
        }

        private bool _AddNewScore()
        {
            ScoreDetailID = clsScoreDetailsPerTermData.AddScore(EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy);
            if (ScoreDetailID != -1)
            {
                string newValue = $"EnrollmentID={EnrollmentID}, SubjectID={SubjectID}, TermID={TermID}, TestScore={TestScore}, ExamScore={ExamScore}";
                clsAuditLog.AddLog("ScoreDetailsPerTerm", ScoreDetailID, "INSERT", null, newValue, CreatedBy);
                return true;
            }
            return false;
        }

        private bool _UpdateScore()
        {
            int oldEnrollmentID = -1, oldSubjectID = -1, oldTermID = -1;
            decimal oldTestScore = 0, oldExamScore = 0;

            clsScoreDetailsPerTermData.GetScoreByID(ScoreDetailID, ref oldEnrollmentID, ref oldSubjectID, ref oldTermID, ref oldTestScore, ref oldExamScore);

            string oldValue = $"EnrollmentID={oldEnrollmentID}, SubjectID={oldSubjectID}, TermID={oldTermID}, TestScore={oldTestScore}, ExamScore={oldExamScore}";
            string newValue = $"EnrollmentID={EnrollmentID}, SubjectID={SubjectID}, TermID={TermID}, TestScore={TestScore}, ExamScore={ExamScore}";

            ModifiedAt = DateTime.Now;
            bool updated = clsScoreDetailsPerTermData.UpdateScore(ScoreDetailID, TestScore, ExamScore, ModifiedBy);
            if (updated)
                clsAuditLog.AddLog("ScoreDetailsPerTerm", ScoreDetailID, "UPDATE", oldValue, newValue, ModifiedBy);

            return updated;
        }

        // Delete
        public static bool DeleteScore(int scoreDetailID, int modifiedBy)
        {
            int oldEnrollmentID = -1, oldSubjectID = -1, oldTermID = -1;
            decimal oldTestScore = 0, oldExamScore = 0;

            clsScoreDetailsPerTermData.GetScoreByID(scoreDetailID, ref oldEnrollmentID, ref oldSubjectID, ref oldTermID, ref oldTestScore, ref oldExamScore);
            string oldValue = $"EnrollmentID={oldEnrollmentID}, SubjectID={oldSubjectID}, TermID={oldTermID}, TestScore={oldTestScore}, ExamScore={oldExamScore}";

            bool deleted = clsScoreDetailsPerTermData.DeleteScore(scoreDetailID);
            if (deleted)
                clsAuditLog.AddLog("ScoreDetailsPerTerm", scoreDetailID, "DELETE", oldValue, null, modifiedBy);

            return deleted;
        }

        // Find existing score by ID
        public static clsScoreDetailsPerTerm Find(int scoreDetailID)
        {
            int enrollmentID = -1, subjectID = -1, termID = -1;
            decimal testScore = 0, examScore = 0;

            bool exists = clsScoreDetailsPerTermData.GetScoreByID(scoreDetailID, ref enrollmentID, ref subjectID, ref termID, ref testScore, ref examScore);
            if (exists)
                return new clsScoreDetailsPerTerm(scoreDetailID, enrollmentID, subjectID, termID, testScore, examScore);

            return null;
        }
        // Total scores by enrollment for a term
        public static DataTable GetTotalScoresByEnrollment(int termID) => clsScoreDetailsPerTermData.GetTotalScoresByEnrollment(termID);

        // Passed/Failed counts
        public static (int Passed, int Failed) GetPassedFailedCounts(int termID, decimal passThreshold = 50m)
        {
            DataTable dtTotals = GetTotalScoresByEnrollment(termID);
            int passed = 0, failed = 0;

            foreach (DataRow row in dtTotals.Rows)
            {
                decimal totalPercentage = row["Percentage"] == DBNull.Value ? 0m : Convert.ToDecimal(row["Percentage"]);
                if (totalPercentage >= passThreshold) passed++; else failed++;
            }

            return (passed, failed);
        }
        // Data retrieval
        public static DataTable GetAllScores() => clsScoreDetailsPerTermData.GetAllScores();
        public static DataTable GetScoresByEnrollmentAndTerm(int enrollmentID, int termID) => clsScoreDetailsPerTermData.GetScoresByEnrollmentAndTerm(enrollmentID, termID);
        public static DataTable LoadByEnrollmentAndTerm(int enrollmentID, int termID) => clsScoreDetailsPerTermData.LoadByEnrollmentAndTerm(enrollmentID, termID);
    }


}
