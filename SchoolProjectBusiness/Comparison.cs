using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SchoolProjectBusiness
{
    public static class clsScores
    {
        public static (DataRow Highest, DataRow Lowest) GetHighestLowestGrade(int termID)
        {
            DataTable dtGrades = clsScoresData.GetGradeAverages(termID);

            if (dtGrades.Rows.Count == 0)
                return (null, null);

            DataRow highest = dtGrades.AsEnumerable()
                .OrderByDescending(r => r.Field<decimal?>("GradeAverage") ?? 0)
                .First();

            DataRow lowest = dtGrades.AsEnumerable()
                .OrderBy(r => r.Field<decimal?>("GradeAverage") ?? 0)
                .First();

            return (highest, lowest);
        }


        /// <summary>
        /// Returns average score per class in a grade for a specific term.
        /// </summary>
        public static DataTable GetClassAverageScores(int gradeID, int termID)
        {
            return clsScoresData.GetClassAverageScores(gradeID, termID);
        }
        public static DataTable GetGradeAverageScoresByTerm(int termID)
        {
            return clsScoresData.GetGradeAverageScoresByTerm(termID);
        }
        public static DataTable GetAllClassAverageScores(int termID)
        {
            return clsScoresData.GetAllClassAverageScores(termID);
        }
        public static DataTable GetGradesAverageScores(int termID)
        {
            return clsScoresData.GetGradesAverageScores(termID);
        }

        /// <summary>
        /// Gets the average total scores of all students in a specific class for a given term.
        /// </summary>
        /// <param name="classID">The class ID</param>
        /// <param name="termID">The term ID</param>
        /// <returns>DataTable containing EnrollmentID, FullName, and AvgScore</returns>
        public static DataTable GetStudentsAverageScores(int gradeID, int classID, int termID)
        {
            return clsScoresData.GetStudentsAverageScores(gradeID, classID, termID);
        }
    }
}
