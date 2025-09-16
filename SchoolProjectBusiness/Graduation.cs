using SchoolProject.Business;
using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public static class clsGraduation
    {
        public static decimal SafeDecimal(object value, decimal defaultValue = 0m)
        {
            return (value == null || value == DBNull.Value) ? defaultValue : Convert.ToDecimal(value);
        }

        public static DataTable GetStudentsWithPredictedGraduation(int termID)
        {
            DataTable dt = clsScoreDetailsPerTerm.GetTotalScoresByEnrollment(termID);

            if (dt == null) return null;

            if (!dt.Columns.Contains("PredictedLetterGrade"))
                dt.Columns.Add("PredictedLetterGrade", typeof(string));

            if (!dt.Columns.Contains("IsPredictedPassed"))
                dt.Columns.Add("IsPredictedPassed", typeof(bool));

            foreach (DataRow row in dt.Rows)
            {
                // ✅ Safe conversion: treat NULL as 0
                decimal percentage = row["Percentage"] == DBNull.Value
                    ? 0m
                    : Convert.ToDecimal(row["Percentage"]);

                row["PredictedLetterGrade"] = GetLetterGrade(percentage);
                row["IsPredictedPassed"] = percentage >= 50m;
            }

            return dt;
        }

        public static DataTable GetStudentsForGraduation(int termID)
        {
            return GraduationData.GetStudentTotalsForGraduation(termID);
        }
        //public static DataTable GetStudentsWithPredictedGraduation(int termID)
        //{
        //    DataTable dt = clsScoresDetails.GetTotalScoresByEnrollment();

        //    if (dt == null) return new DataTable();

        //    // Add columns for predicted graduation
        //    if (!dt.Columns.Contains("IsPredictedPassed"))
        //        dt.Columns.Add("IsPredictedPassed", typeof(bool));

        //    if (!dt.Columns.Contains("PredictedLetterGrade"))
        //        dt.Columns.Add("PredictedLetterGrade", typeof(string));

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        decimal percentage = Convert.ToDecimal(row["Percentage"]);
        //        bool passed = percentage >= 50m;

        //        row["IsPredictedPassed"] = passed;
        //        row["PredictedLetterGrade"] = GetLetterGrade(percentage);
        //    }

        //    return dt;
        //}
        public static bool IsAlreadyGraduated(int enrollmentID, int termID)
        {
            // ✅ Just call the Data Layer
            return GraduationData.IsAlreadyGraduated(enrollmentID, termID);
        }


        public static bool GraduateAndPromoteStudent(int enrollmentID, int termID, decimal finalAverage, int createdBy)
        {
            bool passed = finalAverage >= 50m;
            string letterGrade = GetLetterGrade(finalAverage);

            // 1️⃣ Save or update graduation record
            if (GraduationData.GraduationRecordExists(enrollmentID, termID))
            {
                if (!GraduationData.UpdateGraduationRecord(enrollmentID, termID, finalAverage, letterGrade, passed))
                    return false;
            }
            else
            {
                if (!GraduationData.AddGraduationRecord(enrollmentID, termID, finalAverage, letterGrade, passed, createdBy))
                    return false;
            }

            // 2️⃣ Get current enrollment
            DataRow currentEnrollmentRow = clsEnrollmentData.GetEnrollmentByID(enrollmentID);
            if (currentEnrollmentRow == null)
                return false;

            int studentID = Convert.ToInt32(currentEnrollmentRow["StudentID"]);
            int gradeID = Convert.ToInt32(currentEnrollmentRow["GradeID"]);
            int classID = Convert.ToInt32(currentEnrollmentRow["ClassID"]);

            // 3️⃣ Handle history for current enrollment
            clsEnrollmentHistory currentHistory = clsEnrollmentHistory.Get(enrollmentID, termID);
            if (currentHistory != null)
            {
                if (currentHistory.IsGraduated != passed)
                {
                    currentHistory.IsGraduated = passed;
                    currentHistory.Update();
                }
            }
            else
            {
                clsEnrollmentHistory newHistory = new clsEnrollmentHistory
                {
                    EnrollmentID = enrollmentID,
                    TermID = termID,
                    IsGraduated = passed
                };
                newHistory.Add();
            }

            // 4️⃣ Deactivate current enrollment
            if (!clsEnrollmentData.DeactivateEnrollment(enrollmentID))
                return false;

            // 5️⃣ Create new enrollment for next term (pass → promote, fail → repeat)
            int nextTermID =2; // clsTerm.GetNextTermID(termID, createdBy);
            if (nextTermID <= 0) return false;

            // Only create new enrollment if it doesn't exist
            if (!clsEnrollmentData.DoesEnrollmentExist(studentID, nextTermID))
            {
                clsEnrollment newEnrollment = new clsEnrollment
                {
                    StudentID = studentID,
                    GradeID = passed ? gradeID + 1 : gradeID, // repeat same grade if failed
                    ClassID = classID,
                    TermID = nextTermID,
                    EnrollmentDate = DateTime.Now,
                    IsActive = true,
                    CreatedByUserID = createdBy
                };

                if (!newEnrollment.Save())
                    return false;

                clsEnrollmentHistory nextHistory = new clsEnrollmentHistory
                {
                    EnrollmentID = newEnrollment.EnrollmentID,
                    TermID = nextTermID,
                    IsGraduated = false
                };
                nextHistory.Add();
            }

            return true;
        }

        public static string GetLetterGrade(decimal score)
        {
            if (score >= 90m) return "A+";
            if (score >= 85m) return "A";
            if (score >= 80m) return "A-";
            if (score >= 75m) return "B+";
            if (score >= 70m) return "B";
            if (score >= 65m) return "B-";
            if (score >= 60m) return "C+";
            if (score >= 55m) return "C";
            if (score >= 50m) return "C-";
            return "F";
        }

        public static bool IsFinalTerm(int termID)
        {
            var term = clsTerm.Find(termID);
            return term != null && term.IsFinal;
        }
    }
}
