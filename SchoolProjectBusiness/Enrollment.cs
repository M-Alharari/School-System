using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsEnrollment
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int EnrollmentID { get; set; }
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public int GradeID { get; set; }
        public int TermID { get; set; }              // <-- replaced AcademicYear
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string ModifiedByUser { get; set; }
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        public clsUser UserInfo;
        public clsClass ClassInfo;
        public clsGrade GradeInfo;
        public clsTerm TermInfo;                     // <-- Term info
        public clsStudent StudentInfo;
        // Read-only property to get student's full name via Person class
        public string StudentFullName
        {
            get
            {
                return StudentInfo?.PersonInfo?.FullName ?? "Unknown";
            }
        }
        public static clsEnrollment GetEnrollment(int enrollmentID)
        {
            DataRow row = clsEnrollmentData.GetEnrollmentByID(enrollmentID);
            if (row == null) return null;

            return new clsEnrollment
            {
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                StudentID = Convert.ToInt32(row["StudentID"]),
                GradeID = Convert.ToInt32(row["GradeID"]),
                ClassID = Convert.ToInt32(row["ClassID"]),
                TermID = Convert.ToInt32(row["TermID"]),
                EnrollmentDate = Convert.ToDateTime(row["EnrollmentDate"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedByUserID = Convert.ToInt32(row["CreatedByUserID"])
            };
        }
    
        public clsEnrollment()
        {
            EnrollmentID = -1;
            StudentID = -1;
            ClassID = -1;
            GradeID = -1;
            TermID = -1;
            IsActive = false;
            CreatedByUserID = -1;
        }

        private clsEnrollment(int enrollmentID, int studentID, int classID, int gradeID, int termID,
            DateTime enrollmentDate, bool isActive, int createdByUserID, DateTime createdAt,
            string modifiedByUser, DateTime modifiedAt)
        {
            EnrollmentID = enrollmentID;
            StudentID = studentID;
            ClassID = classID;
            GradeID = gradeID;
            TermID = termID;
            EnrollmentDate = enrollmentDate;
            IsActive = isActive;
            CreatedByUserID = createdByUserID;
            CreatedAt = createdAt;
            ModifiedByUser = modifiedByUser;
            ModifiedAt = modifiedAt;

            ClassInfo = clsClass.FindClassByID(classID);
            GradeInfo = clsGrade.FindGradeByID(gradeID);
            UserInfo = clsUser.FindByUserID(createdByUserID);
            TermInfo = clsTerm.Find(termID);
            StudentInfo= clsStudent.FindStudentByID(studentID);

            Mode = enMode.Update;
        }

        private bool _AddNewEnrollment()
        {
            // Check if student already has active enrollment in this term
            //var existing = GetActiveEnrollmentByStudentID(StudentID);
            //if (existing != null && existing.TermID == this.TermID)
            //{
            //    // Optionally, you can update the existing enrollment instead
            //    EnrollmentID = existing.EnrollmentID;
            //    Mode = enMode.Update;
            //    return _UpdateEnrollment();
            //}

            EnrollmentID = clsEnrollmentData.AddNewEnrollment(StudentID, ClassID, GradeID,
                TermID, EnrollmentDate, IsActive, CreatedByUserID, CreatedAt);
            return EnrollmentID != -1;
        }

        // ✅ Load enrollment by EnrollmentID
        public static clsEnrollment Find(int enrollmentID)
        {
            int studentID = 0, classID = 0, gradeID = 0, termID = 0, createdByUserID = 0;
            DateTime enrollmentDate = DateTime.MinValue, createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;
            bool isActive = false;
            string modifiedByUser = "";

            bool found = clsEnrollmentData.GetEnrollmentByEnrollmentID(
                enrollmentID,
                ref studentID, ref classID, ref gradeID, ref termID,
                ref enrollmentDate, ref isActive,
                ref createdByUserID, ref createdAt, ref modifiedByUser, ref modifiedAt);

            if (found)
            {
                return new clsEnrollment
                {
                    EnrollmentID = enrollmentID,
                    StudentID = studentID,
                    ClassID = classID,
                    GradeID = gradeID,
                    TermID = termID,
                    EnrollmentDate = enrollmentDate,
                    IsActive = isActive,
                    CreatedByUserID = createdByUserID,
                    CreatedAt = createdAt,
                    ModifiedByUser = modifiedByUser,
                    ModifiedAt = modifiedAt  
                };
            }

            return null; // not found
        }
        private bool _UpdateEnrollment()
        {
            return clsEnrollmentData.UpdateEnrollment(EnrollmentID, StudentID, ClassID, GradeID,
                TermID,/* EnrollmentDate, */IsActive/*, CreatedByUserID, CreatedAt*/);
        }
         public bool SaveChanges()
        {
            return clsEnrollmentData.DeactivateEnrollment(EnrollmentID );
        }

        public bool Save()
        {
            return Mode == enMode.AddNew ? _AddNewEnrollment() : _UpdateEnrollment();
        }

        public static DataTable GetScoresByEnrollmentID(int enrollmentID)
        {
            return clsEnrollmentData.GetScoresByEnrollmentID(enrollmentID);
        }

        public static int? GetActiveEnrollmentIDByStudentID(int studentID)
        {
            return clsEnrollmentData.GetActiveEnrollmentIDByStudentID(studentID);
        }

        public static clsEnrollment FindByStudentID(int studentID)
        {
            int enrollmentID = -1, classID = -1, gradeID = -1, termID = -1, createdByUserID = -1;
            bool isActive = false;
            DateTime enrollmentDate = DateTime.MinValue, createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;
            string modifiedByUser = "";

            bool found = clsEnrollmentData.GetEnrollmentByStudentID(studentID, ref enrollmentID, ref classID, ref gradeID,
                ref termID, ref enrollmentDate, ref isActive, ref createdByUserID, ref createdAt, ref modifiedByUser, ref modifiedAt);

            if (found)
            {
                return new clsEnrollment(enrollmentID, studentID, classID, gradeID, termID,
                    enrollmentDate, isActive, createdByUserID, createdAt, modifiedByUser, modifiedAt);
            }

            return null;
        }

        public static bool DeleteEnrollment(int enrollmentID)
        {
            return clsEnrollmentData.DeleteEnrollment(enrollmentID);
        }
        public static bool DeactivateEnrollment(int enrollmentID, int modifiedBy)
        {
            return clsEnrollmentData.DeactivateEnrollment(enrollmentID, modifiedBy);
        }

        public static bool DoesEnrollmentExist(int enrollmentID)
        {
            return clsEnrollmentData.EnrollmentExists(enrollmentID);
        }

        public static DataTable GetAllEnrollments()
        {
            return clsEnrollmentData.GetAllEnrollments();
        }
        public static clsEnrollment FindByID(int enrollmentID)
        {
            DataRow row = clsEnrollmentData.GetEnrollmentByID(enrollmentID);
            if (row == null) return null;

            return new clsEnrollment
            {
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                StudentID = Convert.ToInt32(row["EnrollmentID"]),
                TermID = Convert.ToInt32(row["TermID"]),
                GradeID = Convert.ToInt32(row["GradeID"]),
                ClassID = Convert.ToInt32(row["ClassID"]),
                EnrollmentDate = Convert.ToDateTime(row["EnrollmentDate"]),
                IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"])
            };
        }

        // Get the active enrollment for a student
        public static clsEnrollment GetActiveEnrollmentByStudentID(int studentID)
        {
            var dt = clsEnrollmentData.GetEnrollmentsByStudentID(studentID); // returns DataTable
            foreach (System.Data.DataRow row in dt.Rows)
            {
                if (row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]))
                {
                    return FindByID(Convert.ToInt32(row["EnrollmentID"]));
                }
            }
            return null;
        }
        // Mark this enrollment as completed
        public static bool MarkAsCompleted(int enrollmentID)
        {
            if (enrollmentID <= 0) return false;
            return clsEnrollmentData.MarkAsCompleted(enrollmentID);
        }

        // Optional: repeat enrollment for failed student
        public static bool RepeatEnrollment(int enrollmentID, int nextTermID)
        {
            var enrollment = FindByID(enrollmentID);
            if (enrollment == null) return false;

            clsRepeatedEnrollment repeat = new clsRepeatedEnrollment
            {
                OriginalEnrollmentID = enrollment.EnrollmentID,
                GradeID = enrollment.GradeID,
                ClassID = enrollment.ClassID,
                TermID = nextTermID,
                Reason = "Failed"
            };

            return repeat.Save();
        }

        public static int GetTotalEnrollments()
        {
            return clsEnrollmentData.GetTotalEnrollments();
        }

        public static int GetConvertedEnrollments()
        {
            return clsEnrollmentData.GetConvertedEnrollments();
        }


        // ✅ New function: Enrollment by gender
        public static DataTable GetEnrollmentByGender()
        {
            return clsEnrollmentData.GetEnrollmentByGender();
        }

        // ✅ Yearly Enrollment Trend
        public static DataTable GetYearlyEnrollmentTrend()
        {
            return clsEnrollmentData.GetYearlyEnrollmentTrend();
        }

        // ✅ Grade distribution with repeaters
        public static DataTable GetGradeDistributionWithRepeaters()
        {
            return clsEnrollmentData.GetGradeDistributionWithRepeaters();
        }









    }
}
 
