using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsEmployeeAttendance
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int AttendanceID { get; set; }
        public int EmployeeID { get; set; }
        public DateTime AttendanceDate { get; set; }
        public bool IsPresent { get; set; }          // Use bool now
        public string AbsenceReason { get; set; }    // optional
        public string Notes { get; set; }            // optional

        public clsEmployeeAttendance()
        {
            AttendanceID = -1;
            EmployeeID = -1;
            AttendanceDate = DateTime.Today;
            IsPresent = false;  // default to absent
            AbsenceReason = null;
            Notes = null;
            Mode = enMode.AddNew;
        }
        private clsEmployeeAttendance(int attendanceID, int employeeID, DateTime attendanceDate, bool isPresent, string absenceReason, string notes)
        {
            AttendanceID = attendanceID;
            EmployeeID = employeeID;
            AttendanceDate = attendanceDate;
            IsPresent = isPresent;
            AbsenceReason = absenceReason;
            Notes = notes;
            Mode = enMode.Update;
        }

        private bool _AddNewAttendance(int createdByUserID)
        {
            AttendanceID = clsEmployeesAttendanceData.AddAttendance(
                EmployeeID, IsPresent, AbsenceReason, Notes, createdByUserID, AttendanceDate);

            if (AttendanceID != -1)
            {
                Mode = enMode.Update;
                return true;
            }
            return false;
        }

        private bool _UpdateAttendance(int modifiedByUserID)
        {
            return clsEmployeesAttendanceData.UpdateAttendance(
                AttendanceID, EmployeeID, IsPresent, AbsenceReason, Notes, modifiedByUserID, AttendanceDate);
        }

        public bool Save(int userID)
        {
            return Mode == enMode.AddNew ? _AddNewAttendance(userID) : _UpdateAttendance(userID);
        }

        public static clsEmployeeAttendance FindByID(int attendanceID)
        {
            int employeeID = -1;
            bool isPresent = false;
            string absenceReason = null;
            string notes = null;
            DateTime attendanceDate = DateTime.Today;

            bool found = clsEmployeesAttendanceData.GetAttendanceByID(
                attendanceID, ref employeeID, ref isPresent, ref absenceReason, ref notes, ref attendanceDate);

            if (found)
                return new clsEmployeeAttendance(attendanceID, employeeID, attendanceDate, isPresent, absenceReason, notes);

            return null;
        }

        public static DataTable GetAllAttendance() => clsEmployeesAttendanceData.GetAllAttendance();

        public static DataTable GetAttendanceByMonth(int month, int year) =>
            clsEmployeesAttendanceData.GetAttendanceByMonth(month, year);

        public static bool DeleteAttendance(int attendanceID) =>
            clsEmployeesAttendanceData.DeleteAttendance(attendanceID);

        public static bool IsAttendanceExists(int attendanceID) =>
            clsEmployeesAttendanceData.DoesAttendanceExist(attendanceID);

        public static DataTable GetAttendanceByMonth(string yearMonth)
        {
            return clsEmployeesAttendanceData.GetAttendanceByMonth(yearMonth);
        }


        // Business-layer Exists method
        public static bool Exists(int employeeID, DateTime date)
        {
            return clsEmployeesAttendanceData.Exists(employeeID, date);
        }




        // Other properties and methods you already have...

        /// <summary>
        /// Checks if attendance is complete for all employees in a given year/month.
        /// </summary>
        public static bool HasCompleteAttendance(int year, int month)
        {
            return clsEmployeesAttendanceData.HasCompleteAttendance(year, month);
        }




    }
}
