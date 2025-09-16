using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectBusiness
{
    public class clsAttendanceRecord
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int AttendanceID { get; set; }

        public int PersonID { get; set; }

        public DateTime AttendanceDate { get; set; }

        public bool IsPresent { get; set; }

        public TimeSpan? CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }

        public string Notes { get; set; }

        public int? RecordedBy { get; set; }

        public DateTime? CreatedAt { get; set; }



        public clsAttendanceRecord()
        {
            this.AttendanceID = default;

            this.PersonID = default;

            this.AttendanceDate = default;

            this.IsPresent = default;

            this.CheckInTime = default;

            this.CheckOutTime = default;

            this.Notes = default;

            this.RecordedBy = default;

            this.CreatedAt = default;

            Mode = enMode.AddNew;
        }

        private clsAttendanceRecord(int AttendanceID, int PersonID,
           DateTime AttendanceDate, bool IsPresent, TimeSpan? CheckInTime,
           TimeSpan? CheckOutTime, string Notes, int? RecordedBy,
           DateTime? CreatedAt)
        {
            this.AttendanceID = AttendanceID;

            this.PersonID = PersonID;

            this.AttendanceDate = AttendanceDate;

            this.IsPresent = IsPresent;

            this.CheckInTime = CheckInTime;

            this.CheckOutTime = CheckOutTime;

            this.Notes = Notes;

            this.RecordedBy = RecordedBy;

            this.CreatedAt = CreatedAt;

            Mode = enMode.Update;
        }
        public class clsTeacherAttendanceSummary
        {
            public int TotalDays { get; set; }
            public int DaysPresent { get; set; }
            public int DaysAbsent => TotalDays - DaysPresent;
            public double AttendancePercentage => TotalDays == 0 ? 0 : (DaysPresent * 100.0) / TotalDays;
            public DateTime? LastDayPresent { get; set; }
            public string MostCommonAbsenceReason { get; set; }
        }
        //public static DataTable GetSummaryForTeacher(int teacherID)
        //{
        //    return clsAttendanceRecordData.GetTeacherAttendanceSummary(teacherID);
        //}



        private bool _AddNewAttendanceRecord()
        {
            this.AttendanceID = (clsAttendanceRecordData.AddNewAttendanceRecord
             (this.AttendanceID, this.PersonID,
              this.AttendanceDate, this.IsPresent, this.CheckInTime,
              this.CheckOutTime, this.Notes, this.RecordedBy,
              this.CreatedAt));
            return AttendanceID != 0;
        }

        private bool _UpdateAttendanceRecord()
        {
            return clsAttendanceRecordData.UpdateAttendanceRecord(AttendanceID, PersonID,
             AttendanceDate, IsPresent, CheckInTime,
             CheckOutTime, Notes, RecordedBy,
             CreatedAt);
        }

        public static bool DeleteAttendanceRecord(int AttendanceID)
        {
            return clsAttendanceRecordData.DeleteAttendanceRecord(AttendanceID);
        }

        public static clsAttendanceRecord Find(int AttendanceID)
        {

            int PersonID = default;
            DateTime AttendanceDate = default; bool IsPresent = default; TimeSpan? CheckInTime = default;
            TimeSpan? CheckOutTime = default; string Notes = default; int? RecordedBy = default;
            DateTime? CreatedAt = default;



            bool IsFound = clsAttendanceRecordData.GetAttendanceRecordByAttendanceID
                          (AttendanceID, ref PersonID,
               ref AttendanceDate, ref IsPresent, ref CheckInTime,
               ref CheckOutTime, ref Notes, ref RecordedBy,
               ref CreatedAt);

            if (IsFound)
            {
                return new clsAttendanceRecord
                                (AttendanceID, PersonID,
                 AttendanceDate, IsPresent, CheckInTime,
                 CheckOutTime, Notes, RecordedBy,
                 CreatedAt);

            }

            return null;

        }
        public static DataTable GetSummaryForTeacher(int teacherID)
        {
            return clsAttendanceRecordData.GetTeacherAttendanceSummary(teacherID);
        }


        public static bool IsAttendanceRecordExists(int AttendanceID)
        {

            return clsAttendanceRecordData.IsAttendanceRecordExists(AttendanceID);

        }

        public static DataTable GetAllAttendanceRecords()
        {

            return clsAttendanceRecordData.GetAllAttendanceRecords();

        }

        public bool Save()
        {

            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewAttendanceRecord())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdateAttendanceRecord();

            }

            return false;
        }



    }
}
