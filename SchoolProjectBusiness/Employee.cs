using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectBusiness
{

    public class clsEmployee
    {
        enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        public int EmployeeID { get; set; }
        public int PersonID { get; set; }
        public clsPerson PersonInfo;
        public string JobTitle { get; set; }
        public double MonthlySalary { get; set; }
        public string HiredDate { get; set; }
        public bool EmployeeStatus { get; set; }


        public clsEmployee()
        {
            EmployeeID = -1;
            PersonID = -1;
            JobTitle = "";
            MonthlySalary = -1;
            HiredDate = DateTime.Now.ToString();
            EmployeeStatus = false;


            Mode = enMode.AddNew;

        }

        private clsEmployee(int employeeID, int personID, string JobTitle, double monthlySalary, string hiredDate, bool EmployeeStuts)
        {

            this.EmployeeID = employeeID;
            this.PersonID = personID;
            this.JobTitle = JobTitle;
            this.MonthlySalary = monthlySalary;
            this.HiredDate = hiredDate;
            this.EmployeeStatus = EmployeeStuts;
            this.PersonInfo = clsPerson.Find(PersonID);
            Mode = enMode.Update;

        }

        private bool _AddNewEmployee()
        {
            this.EmployeeID = clsEmployeeData.AddNewEmployee(this.PersonID, this.JobTitle, this.MonthlySalary, this.HiredDate, this.EmployeeStatus);
            return (this.EmployeeID != -1);
        }

        private bool _UpdateEmployee()
        {
            return clsEmployeeData.UpdateEmployee(this.EmployeeID, this.PersonID, this.JobTitle, this.MonthlySalary, this.HiredDate, this.EmployeeStatus);

        }

        public static clsEmployee FindByEmployeeID(int EmployeeID)
        {
            int PersonID = -1; double MonthlySalary = -1;
            string HiredDate = DateTime.Now.ToString();
            string JobTitle = ""; bool EmployeeStuts = false;
            bool isFound = clsEmployeeData.GetEmployeeByID(EmployeeID, ref PersonID, ref JobTitle, ref MonthlySalary, ref HiredDate, ref EmployeeStuts);

            if (isFound)
            {
                return new clsEmployee(EmployeeID, PersonID, JobTitle, MonthlySalary, HiredDate, EmployeeStuts);
            }
            else
                return null;
        }

        public static clsEmployee FindByPersonID(int PersonID)
        {
            int EmployeeID = -1; double MonthlySalary = -1;
            string HiredDate = DateTime.Now.ToString();
            string JobTitle = ""; bool EmployeeStuts = false;
            bool isFound = clsEmployeeData.GetEmployeeByPersonID(PersonID, ref EmployeeID, ref JobTitle, ref MonthlySalary, ref HiredDate, ref EmployeeStuts);

            if (isFound)
            {
                return new clsEmployee(EmployeeID, PersonID, JobTitle, MonthlySalary, HiredDate, EmployeeStuts);
            }
            else
                return null;
        }







        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    if (_AddNewEmployee())
                    {

                        Mode = enMode.Update;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:

                    return _UpdateEmployee();

            }

            return false;
        }





        public static bool DeleteEmployee(int EmployeeID)
        {
            return clsEmployeeData.DeleteEmployee(EmployeeID);
        }








        public static DataTable GetAllEmployees()
        {
            return clsEmployeeData.GetAllEmployees();
        }

        public static bool DoEmployeeExists(int EmployeeID)
        {
            return clsEmployeeData.DoEmployeeExists(EmployeeID);
        }
        public static bool DoEmployeeExistsForPersonID(int PersonID)
        {
            return clsEmployeeData.DoEmployeeExistsForPersonID(PersonID);
        }

        public static bool IsPersonEmployee(int personID)
        {
            return clsEmployeeData.IsPersonEmployee(personID);
        }




    }
}
