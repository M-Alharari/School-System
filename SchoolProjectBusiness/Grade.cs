using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectBusiness
{

    public class clsGrade
    {
        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode = enMode.AddNew;

        public int GradeID { get; set; }
        public string GradeName { get; set; }


        clsClass ClassInfo;

        public clsGrade()
        {
            GradeID = -1;
            GradeName = "";

            Mode = enMode.AddNew;
        }


        private clsGrade(int GradeID, string GradeName)
        {
            this.GradeID = GradeID;
            this.GradeName = GradeName;

            //this.EmployeeInfo = clsEmployee.FindByEmployeeID(TeacherID);
            Mode = enMode.Update;
        }


        private bool _AddNewGrade()
        {
            this.GradeID = clsGradeData.AddNewGrade(GradeName);
            return (this.GradeID != -1);
        }

        private bool _UpdateGrade()
        {
            return clsGradeData.UpdateGrade(GradeID, GradeName);
        }


        public static clsGrade FindGradeByID(int GradeID)
        {


            string GradeName = "";

            bool isFound = clsGradeData.GetGradeByID(GradeID, ref GradeName);
            if (isFound)
                return new clsGrade(GradeID, GradeName);
            else
                return null;

        }

        public static clsGrade FindGradeByName(string GradeName)
        {

            int GradeID = -1;


            bool isFound = clsGradeData.GetGradeByName(GradeName, ref GradeID);
            if (isFound)
                return new clsGrade(GradeID, GradeName);
            else
                return null;

        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewGrade())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateGrade();

            }

            return false;
        }


        public static DataTable GetAllGrades()
        {
            return clsGradeData.GetAllGrades();
        }

        public static bool DeleteGrade(int GradeID)
        {
            return clsGradeData.DeleteGrade(GradeID);
        }

        public static bool DoGradeExist(int GradeID)
        {
            return clsGradeData.DoGradeExists(GradeID);
        }


        //public static bool DoTeacherExist(int GradeID)
        //{
        //    return clsGradeData.is(GradeID);
        //}


        public static string GetLetterGrade(decimal score)
        {
            if (score >= 95) return "A+";
            if (score >= 90) return "A";
            if (score >= 85) return "A-";
            if (score >= 80) return "B+";
            if (score >= 75) return "B";
            if (score >= 70) return "B-";
            if (score >= 65) return "C+";
            if (score >= 60) return "C";
            if (score >= 50) return "D";
            return "F";
        }













    }


    }
