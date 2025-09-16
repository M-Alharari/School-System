using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsStudentAttendance
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int AttendanceID { get; set; }
        public int StudentID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }          // true = present, false = absent
        public string AbsenceReason { get; set; }    // optional
        public string Notes { get; set; }            // optional

        public clsStudentAttendance()
        {
            AttendanceID = -1;
            StudentID = -1;
            AttendanceDate = DateTime.Today;
            IsPresent = false;  // default to absent
            AbsenceReason = null;
            Notes = null;
            Mode = enMode.AddNew;
        }

        private clsStudentAttendance(int attendanceID, int studentID, DateTime attendanceDate, bool isPresent, string absenceReason, string notes)
        {
            AttendanceID = attendanceID;
            StudentID = studentID;
            AttendanceDate = attendanceDate;
            IsPresent = isPresent;
            AbsenceReason = absenceReason;
            Notes = notes;
            Mode = enMode.Update;
        }

        private bool _AddNewAttendance(int createdByUserID)
        {
            AttendanceID = clsStudentsAttendanceData.AddAttendance(
                StudentID, IsPresent, AbsenceReason, Notes, createdByUserID, AttendanceDate);

            if (AttendanceID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateAttendance(int modifiedByUserID)
        {
            return clsStudentsAttendanceData.UpdateAttendance(
                AttendanceID, StudentID, IsPresent, AbsenceReason, Notes, modifiedByUserID, AttendanceDate);
        }

        public bool Save(int userID)
        {
            return Mode == enMode.AddNew ? _AddNewAttendance(userID) : _UpdateAttendance(userID);
        }

        public static clsStudentAttendance FindByID(int attendanceID)
        {
            int studentID = -1;
            bool isPresent = false;
            string absenceReason = null;
            string notes = null;
            DateTime attendanceDate = DateTime.Today;

            bool found = clsStudentsAttendanceData.GetAttendanceByID(
                attendanceID, ref studentID, ref isPresent, ref absenceReason, ref notes, ref attendanceDate);

            if (found)
                return new clsStudentAttendance(attendanceID, studentID, attendanceDate, isPresent, absenceReason, notes);

            return null;
        }

        public static DataTable GetAllAttendance() => clsStudentsAttendanceData.GetAllAttendance();

        public static DataTable GetAttendanceByMonth(int month, int year) =>
            clsStudentsAttendanceData.GetAttendanceByMonth(month, year);

        public static bool DeleteAttendance(int attendanceID) =>
            clsStudentsAttendanceData.DeleteAttendance(attendanceID);

        public static bool IsAttendanceExists(int attendanceID) =>
            clsStudentsAttendanceData.DoesAttendanceExist(attendanceID);

        public static bool Exists(int studentID, DateTime date) =>
            clsStudentsAttendanceData.Exists(studentID, date);
    }
}
