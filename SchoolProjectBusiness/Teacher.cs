using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectBusiness
{
    public class clsTeacher
    {
        enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;

        public int TeacherID { get; set; }
        public int PersonID { get; set; }
        public int EmployeeID { get; set; }
        public int ClassID { get; set; }
        public int SubjectID { get; set; }
        public clsClass ClassInfo;
        public clsSubject SubjectInfo;
        public clsEmployee EmployeeInfo;
        public clsPerson PersonInfo;
        // Read-only property to get student's full name via Person class
        public string StudentFullName
        {
            get
            {
                return PersonInfo?.FullName ?? "Unknown";
            }
        }
        public clsTeacher()
        {
            TeacherID = -1;

            EmployeeID = -1;
            ClassID = -1;
            SubjectID = -1;
        }

        private clsTeacher(int TeacherID, int EmployeeID, int ClassID, int SubjectID)
        {
            this.TeacherID = TeacherID;

            this.EmployeeInfo = clsEmployee.FindByEmployeeID(EmployeeID);
            this.ClassInfo = clsClass.FindClassByID(ClassID);
            this.SubjectInfo = clsSubject.FindSubjectByID(SubjectID);
            Mode = enMode.Update;
        }

        private bool _AddNewTeacher()
        {
            this.TeacherID = clsTeacherData.AddNewTeacher(this.EmployeeID);
            return (this.TeacherID != -1);
        }

        private bool _UpdateTeacher()
        {
            return clsTeacherData.UpdateTeacher(TeacherID, EmployeeID);
        }

        public static clsTeacher FindByTeacherID(int teacherID)
        {
            clsTeacher teacher = new clsTeacher();
            int employeeID = -1, classID = -1, subjectID = -1;

            if (clsTeacherData.GetTeachersByID(teacherID, ref employeeID))
            {
                teacher.TeacherID = teacherID;
                teacher.EmployeeID = employeeID;
               
                // تحميل بيانات الصف والموضوع (ClassInfo, SubjectInfo) هنا إذا كنت تستخدمها
                return teacher;
            }
            else
            {
                return null;
            }
        }


        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewTeacher())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    return _UpdateTeacher();

            }
            return false;
        }

        public static int GetTotalTeachers()
        {
            return clsTeacherData.GetTotalTeachers();
        }

        public static DataTable GetAllTeachers()
        {
            return clsTeacherData.GetAllTeachers();
        }

        public static bool DeleteTeahcer(int TeacherID)
        {
            return clsTeacherData.DeleteTeacher(TeacherID);
        }

        public static bool DoTeacherExists(int TeacherID)
        {
            return clsTeacherData.DoTeacherExists(TeacherID);
        }
















    }
}
