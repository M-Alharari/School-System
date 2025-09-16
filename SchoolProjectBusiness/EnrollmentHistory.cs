using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsEnrollmentHistory
    {
        public int HistoryID { get; set; }
        public int EnrollmentID { get; set; }
        public int TermID { get; set; }
        public bool IsGraduated { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        // Add a new history record
        public bool Add()
        {
            return clsEnrollmentHistoryData.Add(EnrollmentID, TermID, IsGraduated);
        }

        // Update an existing history record
        public bool Update()
        {
            return clsEnrollmentHistoryData.Update(HistoryID, TermID, IsGraduated);
        }

        // Load history for a specific student
        public static DataTable GetByStudent(int enrollmentID)
        {
            return clsEnrollmentHistoryData.GetByStudent(enrollmentID);
        }

        // Delete a history record
        public bool Delete()
        {
            return clsEnrollmentHistoryData.Delete(HistoryID);
        }


        public static DataTable GetStudentHistory(int studentID)
        {
            return clsEnrollmentHistoryData.GetStudentHistory(studentID);
        }

        public static DataTable GetAllHistories()
        {
            return clsEnrollmentHistoryData.GetAllEnrollmentHistories();
        }



        public static bool Exists(int enrollmentID, int termID)
        {
            return clsEnrollmentHistoryData.Exists(enrollmentID, termID);
        }

        // Get a single history record by enrollment + term
        public static clsEnrollmentHistory Get(int enrollmentID, int termID)
        {
            DataRow row = clsEnrollmentHistoryData.GetHistoryByEnrollmentIDAndTermID(enrollmentID, termID);

            if (row == null)
                return null;

            return new clsEnrollmentHistory
            {
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                TermID = Convert.ToInt32(row["TermID"]),
                IsGraduated = Convert.ToBoolean(row["IsGraduated"])
            };
        }


    }
}
