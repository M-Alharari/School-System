using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsTeacherSubjectAssignment
    {
        public int AssignmentID { get; private set; }
        public int TeacherID { get; set; }
        public int SubjectID { get; set; }
        public DateTime AssignedDate { get; private set; }
        public int CreatedByUserID { get; private set; }
        public int ModifiedByUserID { get; private set; }
        public DateTime ModifiedAt { get; private set; }
        public clsTeacherSubjectAssignment() { }

        public clsTeacherSubjectAssignment(int assignmentID)
        {
            // تحميل بيانات التعيين من الداتا طبقة
            LoadByID(assignmentID);
        }

        public void LoadByID(int assignmentID)
        {
            int teacherID = 0, subjectID = 0, createdByUserID = 0   ;
            DateTime assignedDate = DateTime.MinValue     ;

            bool found = clsTeacherSubjectAssignmentData.GetAssignmentByID(
                assignmentID, ref teacherID, ref subjectID, ref assignedDate, ref createdByUserID);

            if (!found)
                throw new Exception($"Assignment ID {assignmentID} not found.");

            AssignmentID = assignmentID;
            TeacherID = teacherID;
            SubjectID = subjectID;
            AssignedDate = assignedDate;
            CreatedByUserID = createdByUserID;
        }

        public void AddNew(int teacherID, int subjectID, int createdByUserID)
        {
            int id = clsTeacherSubjectAssignmentData.AddNewAssignment(teacherID, subjectID, createdByUserID);
            if (id <= 0)
                throw new Exception("Failed to add new subject assignment.");

            AssignmentID = id;
            TeacherID = teacherID;
            SubjectID = subjectID;
            CreatedByUserID = createdByUserID;
            AssignedDate = DateTime.Now;
        }

        public bool Update(int assignmentID, int teacherID, int subjectID, int modifiedByUserID)
        {
            bool success = clsTeacherSubjectAssignmentData.UpdateAssignment(assignmentID, teacherID, subjectID, modifiedByUserID);
            if (success)
            {
                AssignmentID = assignmentID;
                TeacherID = teacherID;
                SubjectID = subjectID;
                ModifiedByUserID
                    = modifiedByUserID;
            }
            return success;
        }

        public bool Delete(int assignmentID)
        {
            return clsTeacherSubjectAssignmentData.DeleteAssignment(assignmentID);
        }

        public static DataTable GetAll()
        {
            return clsTeacherSubjectAssignmentData.GetAllAssignments();
        }

        public static DataTable GetByTeacher(int teacherID)
        {
            return clsTeacherSubjectAssignmentData.GetAssignmentsByTeacher(teacherID);
        }

        public static bool Exists(int assignmentID)
        {
            return clsTeacherSubjectAssignmentData.DoesAssignmentExist(assignmentID);
        }

        // AssignSubjectsToTeacher تحفظ المواد المعينة للمعلم.
        // أولاً تحذف جميع التعيينات القديمة، ثم تضيف الجديدة.
        public static bool AssignSubjectsToTeacher(int teacherID, List<int> subjectIDs,int CreatedByUserID)
        {
            

            try
            {
                // حذف التعيينات القديمة
                clsTeacherSubjectAssignmentData.DeleteAssignmentsByTeacher(teacherID);

                // إضافة التعيينات الجديدة
                foreach (var subjectID in subjectIDs)
                {
                    clsTeacherSubjectAssignmentData.AddAssignment(teacherID, subjectID, CreatedByUserID);
                }

                return true;
            }
            catch (Exception ex)
            {
              
                return false;
            }
        }

        // دالة لاسترجاع المواد المعينة للمعلم
        public static System.Data.DataTable GetSubjectsByTeacherID(int teacherID)
        {
            return clsTeacherSubjectAssignmentData.GetAssignmentsByTeacher(teacherID);
        }


















    }
}
