using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsStudent
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode Mode = enMode.AddNew;

        public int StudentID { get; set; }
        public int PersonID { get; set; }
        public int GuardianID { get; set; }

        // Metadata
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedAt { get; set; }

        public clsGuardian GuardianInfo;
        public clsPerson PersonInfo;
        public clsClass ClassInfo;
        public clsSubject SubjectInfo;
        public clsGrade GradeInfo;

        public clsStudent()
        {
            this.StudentID = -1;
            this.PersonID = -1;
            this.GuardianID = -1;
            Mode = enMode.AddNew;
        }

        private clsStudent(int StudentID, int PersonID, int GuardianID, int createdBy, DateTime createdAt, int modifiedBy, DateTime modifiedAt)
        {
            this.StudentID = StudentID;
            this.PersonID = PersonID;
            this.GuardianID = GuardianID;

            this.CreatedBy = createdBy;
            this.CreatedAt = createdAt;
            this.ModifiedBy = modifiedBy;
            this.ModifiedAt = modifiedAt;

            PersonInfo = clsPerson.Find(PersonID);
            Mode = enMode.Update;
        }

        private bool _AddNewStudent()
        {
            this.StudentID = clsStudentData.AddNewStudent(PersonID,/* GuardianID,*/ ModifiedBy); // pass ModifiedBy to data layer
            if (this.StudentID != -1)
            {
                string newValue = $"PersonID={PersonID}, GuardianID={GuardianID}";
                clsAuditLog.AddLog("Students", this.StudentID, "INSERT", null, newValue, ModifiedBy);
                return true;
            }
            return false;
        }
        public static DataRow GetStudentInfoWithTuition(int studentID)
        {
            return clsStudentData.GetStudentInfoWithTuition(studentID);
        }
        private bool _UpdateStudent()
        {
            int oldPersonID = -1, oldGuardianID = -1;
            clsStudentData.GetStudentInfoByID(StudentID, ref oldPersonID );
            string oldValue = $"PersonID={oldPersonID}, GuardianID={oldGuardianID}";
            string newValue = $"PersonID={PersonID}, GuardianID={GuardianID}";

            bool updated = clsStudentData.UpdateStudent(StudentID,  ModifiedBy); // Updated data layer
            if (updated)
            {
                clsAuditLog.AddLog("Students", this.StudentID, "UPDATE", oldValue, newValue, ModifiedBy);
            }
            return updated;
        }
        public static clsStudent FindStudentByEnrollmentID(int enrollmentID)
        {
            // 1️⃣ Get the enrollment
            clsEnrollment enrollment = clsEnrollment.Find(enrollmentID);
            if (enrollment == null) return null;

            // 2️⃣ Get the student by StudentID
            return clsStudent.FindStudentByID(enrollment.StudentID);
        }

        public static clsStudent FindStudentByID(int StudentID)
        {
            int PersonID = -1, GuardianID = -1, createdBy = -1, modifiedBy = -1;
            DateTime createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;

            bool isFound = clsStudentData.GetStudentInfoByID(StudentID, ref PersonID );
            if (isFound)
                return new clsStudent(StudentID, PersonID, GuardianID, createdBy, createdAt, modifiedBy, modifiedAt);
            else
                return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewStudent())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateStudent();
            }
            return false;
        }

        public static DataTable GetStudentsByClassID(int classID)
        {
            return clsStudentData.GetStudentsByClassID(classID);
        }

        public static DataTable GetAllStudents()
        {
            return clsStudentData.GetAllEnrollments();
        }

        public static bool DeleteStudent(int StudentID, int modifiedBy)
        {
            // Get old values before delete
            int oldPersonID = -1, oldGuardianID = -1;
            clsStudentData.GetStudentInfoByID(StudentID, ref oldPersonID );
            string oldValue = $"PersonID={oldPersonID},GuardianID={oldGuardianID}";

            // Call data layer with modifiedBy
            bool deleted = clsStudentData.DeleteStudent(StudentID, modifiedBy);

            if (deleted)
            {
                clsAuditLog.AddLog("Students", StudentID, "DELETE", oldValue, null, modifiedBy);
            }

            return deleted;
        }
        public static int GetTotalStudents()
        {
            return clsStudentData.GetTotalStudents();
        }


        public static bool DoStudentExists(int StudentID)
        {
            return clsStudentData.DoStudentExists(StudentID);
        }
    }
}
