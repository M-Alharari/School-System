using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsExamType
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int ExamTypeID { get; set; }
        public string Title { get; set; }
        public int TermID { get; set; }
        public double Weight { get; set; }  // 0.3 = 30%
        public float MaxScore { get; set; } // <-- Add this
        public bool IsActive { get; set; } = true;

        public clsExamType()
        {
            ExamTypeID = -1;
            Title = "";
            TermID = -1;
            Weight = 0.0;
            MaxScore = 0f; // initialize
            Mode = enMode.AddNew;
            IsActive = true;
        }

        private clsExamType(int examTypeID, string title, int termID, double weight, float maxScore, bool isActive)
        {
            ExamTypeID = examTypeID;
            Title = title;
            TermID = termID;
            Weight = weight;
            MaxScore = maxScore;
            IsActive = isActive;
            Mode = enMode.Update;
        }

        public static clsExamType FindByID(int examTypeID)
        {
            DataRow row = clsExamTypeData.GetExamTypeByID(examTypeID);
            if (row != null)
            {
                return new clsExamType(
                    Convert.ToInt32(row["ExamTypeID"]),
                    Convert.ToString(row["ExamTypeName"]),
                    Convert.ToInt32(row["TermID"]),
                    Convert.ToDouble(row["Weight"]),
                    row.Table.Columns.Contains("MaxScore") && row["MaxScore"] != DBNull.Value ? Convert.ToSingle(row["MaxScore"]) : 0f,
                    row.Table.Columns.Contains("IsActive") && row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : true
                );
            }
            return null;
        }
       
        public bool Save()
        {
            if (string.IsNullOrWhiteSpace(Title))
                throw new Exception("يجب إدخال اسم نوع الامتحان.");

            if (TermID <= 0)
                throw new Exception("يجب تحديد التيرم.");

            if (Weight < 0 || Weight > 1)
                throw new Exception("الوزن يجب أن يكون بين 0 و 1.");

            if (MaxScore <= 0)
                throw new Exception("الحد الأقصى للدرجة يجب أن يكون أكبر من صفر.");

            if (Mode == enMode.AddNew)
            {
                if (clsExamTypeData.ExamTypeExists(Title))
                    throw new Exception("اسم نوع الامتحان موجود بالفعل.");

                ExamTypeID = clsExamTypeData.AddExamType(Title, TermID, Weight, MaxScore);
                return ExamTypeID != -1;
            }
            else // Update
            {
                if (!clsExamTypeData.ExamTypeExists(ExamTypeID))
                    throw new Exception("نوع الامتحان غير موجود.");

                return clsExamTypeData.UpdateExamType(ExamTypeID, Title, TermID, Weight, MaxScore, IsActive);
            }
        }
        public bool Delete()
        {
            if (!clsExamTypeData.ExamTypeExists(ExamTypeID))
                throw new Exception("نوع الامتحان غير موجود.");

            return clsExamTypeData.DeleteExamType(ExamTypeID);
        }


        // Quick static helpers
        // Safe max score to avoid division by zero
        public static float GetMaxScoreSafe(int examTypeID)
        {
            return clsExamTypeData.GetMaxScore(examTypeID);
        }



        public static double GetWeight(int examTypeID)
        {
            var et = FindByID(examTypeID);
            return et?.Weight ?? 0.0;
        }

        public static DataTable GetAllExamTypes()
        {
            return clsExamTypeData.GetAllExamTypes();
        }
    }
}
