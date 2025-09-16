using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsTrends
    {
        // 1️⃣ جلب درجات الطالب عبر التيرمز (Student Trend)
        public static DataTable GetStudentScoresByTerms(int studentID)
        {
            return clsTrendsData.GetStudentScoresByTerms(studentID);
        }
        public static double GetStudentAverageAttendance(int studentID)
        {
            return clsTrendsData.GetStudentAverageAttendance(studentID);
        }
        public static double GetStudentAverageGrade(int studentID)
        {
            return clsTrendsData.GetStudentAverageGrade(studentID);
        }    // Get average grade for a student for a specific term
        public static double GetStudentAverageGradeByTerm(int studentID, int termID)
        {
            return clsTrendsData.GetStudentAverageGradeByEnrollment(studentID, termID);
        }
        // 2️⃣ جلب متوسط الفصل عبر التيرمز (Class Trend)
        public static DataTable GetClassAverageByTerms(int classID)
        {
            return clsTrendsData.GetClassAverageByTerms(classID);
        }

        // 3️⃣ جلب متوسط الصف عبر التيرمز (Grade Trend)
        public static DataTable GetGradeAverageByTerms(int gradeID)
        {
            return clsTrendsData.GetGradeAverageByTerms(gradeID);
        }
    }
}
