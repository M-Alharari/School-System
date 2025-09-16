using SchoolProject.Data;
using SchoolProjectBusiness;
using System.Data;

namespace SchoolProject.Business
{
    public static class clsScoresSummary
    {
        public static DataTable GetSafeClassStudentScores(int classID)
        {
            int currentTermID = clsTerm.GetCurrentTermIDSafe();
            if (currentTermID == -1)
                return new DataTable(); // جدول فارغ لتجنب أي خطأ

            return clsScoresSummaryData.GetClassStudentScores(classID, currentTermID);
        }

        /// <summary>
        /// Get all students’ scores for a class using the current term.
        /// </summary>
        /// 
        public static DataTable GetClassStudentScores(int classID)
        {
            int currentTermID = clsTerm.GetCurrentTermID(); // your method to get active term
            return clsScoresSummaryData.GetClassStudentScores(classID, currentTermID);
        }
        public static DataTable GetClassStudentScores(int classID, int subjectID)
        {
            return clsScoresSummaryData.GetClassStudentScoress(classID, subjectID);
        }

        public static DataTable GetSubjectsByClass(int classID)
        {
            return clsScoresSummaryData.GetSubjectsByClass(classID);
        }
    }
}
