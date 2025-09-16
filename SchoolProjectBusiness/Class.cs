using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsClass
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int ClassID { get; private set; }
        public string ClassName { get; set; }
        public int GradeID { get; set; }

        // إذا لاحقًا بدك ترجع معلومات الموظف المسؤول مثلاً
        // public clsEmployee EmployeeInfo { get; set; }

        public clsClass()
        {
            ClassID = -1;
            GradeID = -1;
            ClassName = "";
            Mode = enMode.AddNew;
        }

        private clsClass(int classID, string className, int gradeID)
        {
            this.ClassID = classID;
            this.ClassName = className;
            this.GradeID = gradeID;
            this.Mode = enMode.Update;
        }

        private bool _AddNewClass()
        {
            this.ClassID = clsClassData.AddNewClass(this.ClassName, this.GradeID);
            return (this.ClassID != -1);
        }

        private bool _UpdateClass()
        {
            return clsClassData.UpdateClass(this.ClassID, this.ClassName);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewClass())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _UpdateClass();

                default:
                    return false;
            }
        }

        public static clsClass FindClassByID(int classID)
        {
            string className = "";
            int gradeID = -1;

            bool isFound = clsClassData.GetClassByID(classID, ref className, ref gradeID);

            if (isFound)
                return new clsClass(classID, className, gradeID);
            else
                return null;
        }

        public static clsClass FindClassByName(string className)
        {
            int classID = -1;
            int gradeID = -1;

            bool isFound = clsClassData.GetClassByName(className, ref classID, ref gradeID);

            if (isFound)
                return new clsClass(classID, className, gradeID);
            else
                return null;
        }

        public static DataTable GetAllClasses()
        {
            return clsClassData.GetAllClasses();
        }
        

        public static DataTable GetClassesByGradeID(int gradeID)
        {
            return clsClassData.GetClassesByGradeID(gradeID);
        }

        public static bool DeleteClass(int classID)
        {
            return clsClassData.DeleteClass(classID);
        }

        public static bool DoClassExist(int classID)
        {
            return clsClassData.DoClassExists(classID);
        }
    }
}
