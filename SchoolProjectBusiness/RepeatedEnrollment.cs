using GrapeCity.Documents.DX.Direct2D;
using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsRepeatedEnrollment
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int RepeatedEnrollmentID { get; set; }
        public int OriginalEnrollmentID { get; set; }
        public int GradeID { get; set; }
        public int ClassID { get; set; }
        public int TermID { get; set; }
        public string Reason { get; set; }  // Optional
        public int CreatedBy { get; set; }


        public clsRepeatedEnrollment()
        {
            RepeatedEnrollmentID = -1;
            OriginalEnrollmentID = -1;
            GradeID = -1;
            ClassID = -1;
            TermID = -1;
            Reason = null;
            CreatedBy = -1;
            Mode = enMode.AddNew;
        }

        private clsRepeatedEnrollment(int repeatedEnrollmentID, int originalEnrollmentID, int gradeID, int classID, int termID, string reason, int createdBy)
        {
            RepeatedEnrollmentID = repeatedEnrollmentID;
            OriginalEnrollmentID = originalEnrollmentID;
            GradeID = gradeID;
            ClassID = classID;
            TermID = termID;
            Reason = reason;
            CreatedBy = createdBy;
            Mode = enMode.Update;
        }

        private bool _AddNew( )
        {
            RepeatedEnrollmentID = clsRepeatedEnrollmentData.AddRepeatedEnrollment(
                OriginalEnrollmentID, GradeID, ClassID, TermID, Reason, CreatedBy);

            if (RepeatedEnrollmentID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        public bool _UpdateRepeatedEnrollment()
        {
            return clsRepeatedEnrollmentData.UpdateRepeatedEnrollment(
                RepeatedEnrollmentID,
                OriginalEnrollmentID,
                GradeID,
                ClassID,
                TermID,
                Reason
            );
        }


        public bool Save( )
        {
            return Mode == enMode.AddNew ? _AddNew( ) : _UpdateRepeatedEnrollment( );
        }

        public static clsRepeatedEnrollment FindByEnrollmentID(int enrollmentID)
        {
            DataTable dt = clsRepeatedEnrollmentData.GetByEnrollment(enrollmentID);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0]; // pick first repeated enrollment if multiple
                return new clsRepeatedEnrollment(
                    Convert.ToInt32(row["RepeatedEnrollmentID"]),
                    Convert.ToInt32(row["OriginalEnrollmentID"]),
                    Convert.ToInt32(row["GradeID"]),
                    Convert.ToInt32(row["ClassID"]),
                    Convert.ToInt32(row["TermID"]),
                    row["Reason"].ToString(),
                    Convert.ToInt32(row["CreatedBy"])
          
                );
            }
            return null;
        }


        public static DataTable GetAll() => clsRepeatedEnrollmentData.GetAllRepeated();
        public static DataTable GetByEnrollment(int enrollmentID) => clsRepeatedEnrollmentData.GetByEnrollment(enrollmentID);
        public static bool Delete(int repeatedEnrollmentID) => clsRepeatedEnrollmentData.DeleteRepeatedEnrollment(repeatedEnrollmentID);
    }
}
