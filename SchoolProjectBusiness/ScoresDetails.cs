using SchoolProject.Data;
using SchoolProjectData;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProject.Business
{
    public class clsScoresDetails
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ScoreID { get; set; } = -1;
        public int EnrollmentID { get; set; }
        public int SubjectID { get; set; }
        public int TermID { get; set; }
        public decimal? TestScore { get; set; }
        public decimal? ExamScore { get; set; }
        public decimal TotalScore => (TestScore ?? 0) + (ExamScore ?? 0);

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public clsScoresDetails()
        {
            Mode = enMode.AddNew;
        }

        private clsScoresDetails(int scoreID, int enrollmentID, int subjectID, int termID,
                                decimal? testScore, decimal? examScore)
        {
            ScoreID = scoreID;
            EnrollmentID = enrollmentID;
            SubjectID = subjectID;
            TermID = termID;
            TestScore = testScore;
            ExamScore = examScore;
            Mode = enMode.Update;
        }

        private bool _AddNewScore()
        {
            ScoreID = clsScoresDetailsData.AddScore(EnrollmentID, SubjectID, TermID, TestScore, ExamScore, CreatedBy);
            if (ScoreID > 0)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateScore()
        {
            ModifiedBy = CreatedBy; // or clsGlobal.CurrentUser.UserName
            ModifiedDate = DateTime.Now;
            return clsScoresDetailsData.UpdateScore(ScoreID, TestScore, ExamScore, ModifiedBy);
        }

        public bool Save()
        {
            return clsScoresDetailsData.SaveScore(
                EnrollmentID, SubjectID, TermID, TestScore, ExamScore,
                string.IsNullOrEmpty(ModifiedBy) ? CreatedBy : ModifiedBy
            );
        }

        public static clsScoresDetails LoadByEnrollmentTermAndSubject(int enrollmentID, int termID, int subjectID)
        {
            var row = clsScoresDetailsData.LoadByEnrollmentTermAndSubject(enrollmentID, termID, subjectID);
            if (row == null) return null;

            return new clsScoresDetails
            {
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                SubjectID = Convert.ToInt32(row["SubjectID"]),
                TermID = Convert.ToInt32(row["TermID"]),
                TestScore = row["TestScore"] as decimal?,
                ExamScore = row["ExamScore"] as decimal?,
                CreatedBy = row["CreatedBy"].ToString(),
                ModifiedBy = row["ModifiedBy"]?.ToString()
            };
        }
        public static DataTable GetAll ()
        {
            return clsScoresDetailsData.GetAll ();
        }
        //public static clsScoresDetails FindByEnrollmentTermSubject(int enrollmentID, int termID, int subjectID)
        //{
        //    DataTable dt = clsScoresDetailsData.LoadByEnrollmentTermAndSubject(enrollmentID, termID, subjectID);
        //    if (dt.Rows.Count > 0)
        //    {
        //        var row = dt.Rows[0];
        //        return new clsScoresDetails(
        //            Convert.ToInt32(row["ScoreID"]),
        //            enrollmentID,
        //            subjectID,
        //            termID,
        //            row["TestScore"] as decimal?,
        //            row["ExamScore"] as decimal?
        //        );
        //    }
        //    return null;
        //}
        //public static clsScoresDetails LoadByEnrollmentTermAndSubject(int enrollmentID, int termID, int subjectID)
        //{
        //    DataTable dt = clsScoresDetailsData.LoadByEnrollmentTermAndSubject(enrollmentID, termID, subjectID);
        //    if (dt.Rows.Count == 0) return null;

        //    DataRow row = dt.Rows[0];
        //    return new clsScoresDetails
        //    {
        //        ScoreID = Convert.ToInt32(row["ScoreID"]),
        //        EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
        //        SubjectID = Convert.ToInt32(row["SubjectID"]),
        //        TermID = Convert.ToInt32(row["TermID"]),
        //        TestScore = row["TestScore"] != DBNull.Value ? Convert.ToDecimal(row["TestScore"]) : (decimal?)null,
        //        ExamScore = row["ExamScore"] != DBNull.Value ? Convert.ToDecimal(row["ExamScore"]) : (decimal?)null
        //    };
        //}
        public static DataTable LoadByEnrollmentAndTerm(int enrollmentID, int termID)
        {
            return   clsScoresDetailsData.LoadByEnrollmentAndTerm(enrollmentID, termID);
        }
        public static DataTable GetScoresByClassAndTerm(int classID, int termID)
        {
            // Optionally: validate inputs
            if (classID <= 0) throw new ArgumentException("Invalid class ID");
            if (termID <= 0) throw new ArgumentException("Invalid term ID");

            return clsScoresDetails.GetScoresByClassAndTerm(classID, termID);
        }
        // Calculate total average for an enrollment (scaled to 100%)
        // Calculate total percentage for all subjects of an enrollment

        public static decimal CalculateTotalScoreSafe(int enrollmentID)
        {
            DataTable dtScores = clsScoresDetailsData.GetScoresByEnrollment(enrollmentID);

            if (dtScores == null || dtScores.Rows.Count == 0)
                return 0m;

            decimal totalObtained = 0m;
            decimal totalMax = 0m;

            foreach (DataRow row in dtScores.Rows)
            {
                decimal testScore = row["TestScore"] == DBNull.Value ? 0m : Convert.ToDecimal(row["TestScore"]);
                decimal examScore = row["ExamScore"] == DBNull.Value ? 0m : Convert.ToDecimal(row["ExamScore"]);
                decimal subjectMax = row["TotalScore"] == DBNull.Value ? 100m : Convert.ToDecimal(row["TotalScore"]); // fallback

                totalObtained += (testScore + examScore);
                totalMax += subjectMax;
            }

            if (totalMax == 0) return 0m;

            decimal percentage = (totalObtained / totalMax) * 100m;
            return Math.Round(percentage, 2);
        }


        // Map average score to letter grade
        public static string GetLetterGrade(decimal percentage)
        {
            if (percentage >= 90) return "A+";
            if (percentage >= 80) return "A";
            if (percentage >= 70) return "B";
            if (percentage >= 60) return "C";
            if (percentage >= 50) return "D";
            return "F";
        }

        // Returns a DataTable with EnrollmentID and TotalPercentage across all subjects
        public static DataTable GetTotalScoresByEnrollment(int termID)
        {
            return clsScoresDetailsData.GetTotalScoresByEnrollment(termID);
        }


        // Optional: helper to get just the passed/failed counts
        public static (int Passed, int Failed) GetPassedFailedCounts(int termID, decimal passThreshold = 50m)
        {
            DataTable dtTotals = GetTotalScoresByEnrollment(termID);
            int passCount = 0, failCount = 0;

            if (dtTotals == null || dtTotals.Rows.Count == 0)
                return (0, 0);

            foreach (DataRow row in dtTotals.Rows)
            {
                decimal totalPercentage = 0m;

                if (row["Percentage"] != DBNull.Value)
                    totalPercentage = Convert.ToDecimal(row["Percentage"]);

                if (totalPercentage >= passThreshold)
                    passCount++;
                else
                    failCount++;
            }

            return (passCount, failCount);
        }

    }
}
