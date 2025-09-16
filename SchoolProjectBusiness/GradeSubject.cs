using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsGradeSubject
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int GradeID { get; set; }
        public int SubjectID { get; set; }

        // كونستركتور جديد
        public clsGradeSubject()
        {
            GradeID = -1;
            SubjectID = -1;
            Mode = enMode.AddNew;
        }

        // كونستركتور للتحميل
        private clsGradeSubject(int gradeID, int subjectID)
        {
            GradeID = gradeID;
            SubjectID = subjectID;
            Mode = enMode.Update;  // العلاقة موجودة
        }

        // إضافة ربط جديد
        private bool _AddNew()
        {
            return clsGradeSubjectData.AddGradeSubject(GradeID, SubjectID);
        }

        // حذف الرابط (يمكن نستخدمه بدل تحديث)
        private bool _Delete()
        {
            return clsGradeSubjectData.DeleteGradeSubject(GradeID, SubjectID);
        }

        // حفظ (AddNew فقط لأنه لا تحديث مباشر لعلاقة)
        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return _AddNew();

                case enMode.Update:
                    // لا يوجد تحديث لعلاقة مباشرة
                    return false;

                default:
                    return false;
            }
        }

        // البحث عن علاقة بين صف ومادة
        //public static clsGradeSubject Find(int gradeID, int subjectID)
        //{
        //    // لو في طبقة بيانات تدعم بحث علاقة
        //    bool exists = clsGradeSubjectData.Exists(gradeID, subjectID);
        //    if (exists)
        //        return new clsGradeSubject(gradeID, subjectID);
        //    else
        //        return null;
        //}

        // جلب كل المواد لصف معين
        public static DataTable GetSubjectsByGradeID(int gradeID)
        {
            return clsGradeSubjectData.GetSubjectsByGradeID(gradeID);
        }

        // حذف كل المواد المرتبطة بصف (لتعيين جديد)
        public static bool DeleteAllByGradeID(int gradeID)
        {
            return clsGradeSubjectData.DeleteAllSubjectsByGradeID(gradeID);
        }

        // تعيين المواد للصف (حذف القديم + إضافة الجديد)
        public static bool AssignSubjectsToGrade(int gradeID, List<int> subjectIDs, out string errorMessage)
        {
            errorMessage = "";

            //if (!clsGradeSubjectData.DeleteAllSubjectsByGradeID(gradeID, out errorMessage))
            //    return false;

            foreach (int subjectID in subjectIDs)
            {
                if (!clsGradeSubjectData.AddGradeSubject(gradeID, subjectID, out errorMessage))
                {
                    return false;
                }
            }

            return true;
        }

    }
}
