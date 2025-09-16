using System;

namespace SchoolProjectBusiness
{
    public class clsTeacherAttendanceSummary
    {
        public int TotalDays { get; set; }
        public int DaysPresent { get; set; }
        public int DaysAbsent => TotalDays - DaysPresent;
        public double AttendancePercentage => TotalDays == 0 ? 0 : (DaysPresent * 100.0) / TotalDays;
        public DateTime? LastDayPresent { get; set; }
        public string MostCommonAbsenceReason { get; set; }
    }
}
