using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SchoolProjectBusiness;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsTimeTableGenerator
    {
        public class TimeSlot
        {
            public int GradeID { get; set; }
            public int ClassID { get; set; }
            public int SubjectID { get; set; }
            public int TeacherID { get; set; }
            public int Period { get; set; }
            public DayOfWeek Day { get; set; }
        }

        public class SchoolPeriod
        {
            public int Period { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public bool IsBreak { get; set; } = false;
        }

        private List<SchoolPeriod> _dailyPeriods;

        // Constructor accepts custom periods; falls back to default if null
        public clsTimeTableGenerator(List<SchoolPeriod> customPeriods = null)
        {
            _dailyPeriods = customPeriods ?? DefaultDailyPeriods();
        }

        // Default periods

        public static List<SchoolPeriod> DefaultDailyPeriods()
        {
            return new List<SchoolPeriod>
    {
        new SchoolPeriod { Period=1, StartTime=TimeSpan.FromHours(8), EndTime=TimeSpan.FromHours(8.45) },
        new SchoolPeriod { Period=2, StartTime=TimeSpan.FromHours(8.50), EndTime=TimeSpan.FromHours(9.35) },
        new SchoolPeriod { Period=3, StartTime=TimeSpan.FromHours(9.40), EndTime=TimeSpan.FromHours(10.25) },
        new SchoolPeriod { Period=4, StartTime=TimeSpan.FromHours(10.25), EndTime=TimeSpan.FromHours(10.40), IsBreak=true },
        new SchoolPeriod { Period=5, StartTime=TimeSpan.FromHours(10.40), EndTime=TimeSpan.FromHours(11.25) }
    };
        }


        public List<SchoolPeriod> DailyPeriods => _dailyPeriods; // Accessor

        // Generate timetable for all grades and classes
        public List<TimeSlot> GenerateWeeklyTimeTable()
        {
            List<TimeSlot> timetable = new List<TimeSlot>();
            DayOfWeek[] weekDays = { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday };

            DataTable dtGrades = clsGradeData.GetAllGrades();

            foreach (DataRow gradeRow in dtGrades.Rows)
            {
                int gradeID = Convert.ToInt32(gradeRow["GradeID"]);
                DataTable dtSubjects = clsGradeSubject.GetSubjectsByGradeID(gradeID);
                DataTable dtClasses = clsClass.GetClassesByGradeID(gradeID);
                DataTable dtAssignments = clsTeacherClassAssignment.GetAllAssignments();

                foreach (DataRow classRow in dtClasses.Rows)
                {
                    int classID = Convert.ToInt32(classRow["ClassID"]);

                    foreach (DayOfWeek day in weekDays)
                    {
                        int periodIndex = 0;

                        foreach (DataRow subjectRow in dtSubjects.Rows)
                        {
                            // Skip break periods
                            while (periodIndex < _dailyPeriods.Count && _dailyPeriods[periodIndex].IsBreak)
                                periodIndex++;

                            if (periodIndex >= _dailyPeriods.Count)
                                break;

                            int subjectID = Convert.ToInt32(subjectRow["SubjectID"]);

                            // Get all teachers assigned to this class/grade
                            DataRow[] teacherRows = dtAssignments.Select($"ClassID={classID} AND GradeID={gradeID}");
                            if (teacherRows.Length == 0)
                                continue;

                            foreach (DataRow teacherRow in teacherRows)
                            {
                                int teacherID = Convert.ToInt32(teacherRow["TeacherID"]);

                                // Check if teacher is already booked
                                if (!clsTimeTableData.IsTeacherBooked(teacherID, day, _dailyPeriods[periodIndex].Period))
                                {
                                    timetable.Add(new TimeSlot
                                    {
                                        GradeID = gradeID,
                                        ClassID = classID,
                                        SubjectID = subjectID,
                                        TeacherID = teacherID,
                                        Period = _dailyPeriods[periodIndex].Period,
                                        Day = day
                                    });

                                    periodIndex++;
                                    if (periodIndex >= _dailyPeriods.Count)
                                        break;
                                }
                            }
                        }

                        // Insert break automatically
                        for (int i = 0; i < _dailyPeriods.Count; i++)
                        {
                            if (_dailyPeriods[i].IsBreak)
                                clsTimeTableData.AddBreakSlot(gradeID, classID, _dailyPeriods[i].Period, day);
                        }
                    }
                }
            }

            return timetable;
        }

        // Save timetable to DB
        public bool SaveWeeklyTimeTable(List<TimeSlot> timetable)
        {
            bool allSaved = true;
            foreach (var slot in timetable)
            {
                if (!clsTimeTableData.AddTimeTableSlot(slot.GradeID, slot.ClassID, slot.SubjectID, slot.TeacherID, slot.Period, slot.Day))
                    allSaved = false;
            }
            return allSaved;
        }

        // Get timetable for a specific grade/class
        public static DataTable GetTimeTable(int gradeID, int classID)
        {
            return clsTimeTableData.GetTimeTable(gradeID, classID);
        }
    }
}
