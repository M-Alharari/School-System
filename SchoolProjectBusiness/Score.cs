using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsScore
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int ScoreID { get; private set; } = -1;
        public int EnrollmentID { get; set; }
        public int SubjectID { get; set; }
        public int ExamTypeID { get; set; }
        public int TermID { get; set; }
   
        public int CreatedByUserID { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int? ModifiedByUserID { get; private set; }
        public DateTime? ModifiedAt { get; private set; }

        // Constructor for new score
        // Constructor for new score with CreatedByUserID
        public clsScore(int createdByUserID)
        {
            if (createdByUserID <= 0)
                throw new Exception("CreatedByUserID must be greater than zero.");
            CreatedByUserID = createdByUserID;
            Mode = enMode.AddNew;
        }




        // Constructor for existing score from data row
        private clsScore(DataRow row)
        {
            ScoreID = Convert.ToInt32(row["ScoreID"]);
            EnrollmentID = Convert.ToInt32(row["EnrollmentID"]);
            SubjectID = Convert.ToInt32(row["SubjectID"]);
            ExamTypeID = Convert.ToInt32(row["ExamTypeID"]);
            TermID = Convert.ToInt32(row["TermID"]);
           
            CreatedByUserID = Convert.ToInt32(row["CreatedByUserID"]);
            CreatedAt = Convert.ToDateTime(row["CreatedAt"]);
            ModifiedByUserID = row["ModifiedByUserID"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["ModifiedByUserID"]);
            ModifiedAt = row["ModifiedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["ModifiedAt"]);

            Mode = enMode.Update;
        }

        private bool _AddNew()
        {
            int id = clsScoreData.AddScore(EnrollmentID, SubjectID, ExamTypeID, TermID, CreatedByUserID);
            if (id != -1)
            {
                ScoreID = id;
                return true;
            }
            return false;
        }

        private bool _Update()
        {
            return clsScoreData.UpdateScore(ScoreID,  CreatedByUserID, ModifiedByUserID);
        }

        public bool Save()
        {
            if (Mode == enMode.AddNew)
            {
                if (CreatedByUserID <= 0)
                    throw new Exception("CreatedByUserID must be set for new records.");
                Mode = enMode.Update;
                return _AddNew();

            }
            else
            {
                return _Update();
            }
        }

        public static clsScore FindByID(int scoreID)
        {
            DataRow row = clsScoreData.GetScoreByID(scoreID);
            if (row != null)
                return new clsScore(row);
            return null;
        }
        public static bool SaveScore(int enrollmentID, int subjectID, int examTypeID, int termID, double rawScore, double scaledScore, int createdByUserID)
        {
            clsScore score = new clsScore(createdByUserID)
            {
                EnrollmentID = enrollmentID,
                SubjectID = subjectID,
                ExamTypeID = examTypeID,
                TermID = termID
            };

            return score.Save();
        }


        public static DataTable GetScoresByEnrollment(int enrollmentID)
        {
            return clsScoreData.GetScoresByEnrollment(enrollmentID);
        }

        public static bool Delete(int scoreID)
        {
            return clsScoreData.DeleteScore(scoreID);
        }
    }
}
