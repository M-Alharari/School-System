using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsTeacherClassAssignment
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int AssignmentID { get; private set; }
        public int TeacherID { get; set; }
        public int ClassID { get; set; }
        public int GradeID { get; set; } // علشان نعرف المرحلة الدراسية
        public DateTime AssignedDate { get; private set; }
        public int CreatedByUserID { get; set; }

        public clsTeacherClassAssignment()
        {
            AssignmentID = -1;
            TeacherID = -1;
            ClassID = -1;
            GradeID = -1;
            AssignedDate = DateTime.Now;
            CreatedByUserID = -1;
            Mode = enMode.AddNew;
        }

        private clsTeacherClassAssignment(int assignmentID, int teacherID, int classID, int gradeID, DateTime assignedDate, int createdByUserID)
        {
            this.AssignmentID = assignmentID;
            this.TeacherID = teacherID;
            this.ClassID = classID;
            this.GradeID = gradeID;
            this.AssignedDate = assignedDate;
            this.CreatedByUserID = createdByUserID;
            this.Mode = enMode.Update;
        }

        private bool _AddNewAssignment()
        {
            this.AssignmentID = clsTeacherClassAssignmentData.AddNewAssignment(this.TeacherID, this.ClassID, this.CreatedByUserID);
            return (this.AssignmentID != -1);
        }

        private bool _UpdateAssignment()
        {
            return clsTeacherClassAssignmentData.UpdateAssignment(this.AssignmentID, this.TeacherID, this.ClassID);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAssignment())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateAssignment();

                default:
                    return false;
            }
        }

        public static clsTeacherClassAssignment FindByID(int assignmentID)
        {
            int teacherID = -1, classID = -1, gradeID = -1, createdByUserID = -1;
            DateTime assignedDate = DateTime.MinValue;

            bool isFound = clsTeacherClassAssignmentData.GetAssignmentByID(assignmentID, ref teacherID, ref classID, ref gradeID, ref assignedDate, ref createdByUserID);

            if (isFound)
                return new clsTeacherClassAssignment(assignmentID, teacherID, classID, gradeID, assignedDate, createdByUserID);
            else
                return null;
        }

        public static DataTable GetAssignmentsByTeacher(int teacherID)
        {
            return clsTeacherClassAssignmentData.GetAssignmentsByTeacher(teacherID);
        }

        public static DataTable GetAllAssignments()
        {
            return clsTeacherClassAssignmentData.GetAllAssignments();
        }

        public static bool DeleteAssignment(int assignmentID)
        {
            return clsTeacherClassAssignmentData.DeleteAssignment(assignmentID);
        }

        public static bool DoesAssignmentExist(int assignmentID)
        {
            return clsTeacherClassAssignmentData.DoesAssignmentExist(assignmentID);
        }

        public static bool AssignClasses(int teacherID, List<int> classIDs, int CreatedBy)
        {
            try
            {
                // Delete old assignments first
                DataTable oldAssignments = GetAssignmentsByTeacher(teacherID);
                foreach (DataRow row in oldAssignments.Rows)
                {
                    DeleteAssignment((int)row["AssignmentID"]);
                }

                // Add new assignments
                foreach (int classID in classIDs)
                {
                    clsTeacherClassAssignment assignment = new clsTeacherClassAssignment
                    {
                        TeacherID = teacherID,
                        ClassID = classID,
                        CreatedByUserID = CreatedBy

                    };

                    if (!assignment.Save())
                    {
                        // If any save fails, return false
                        return false;
                    }
                }

                // All assignments saved successfully
                return true;
            }
            catch
            {
                return false;
            }
        }

    }

}
 
