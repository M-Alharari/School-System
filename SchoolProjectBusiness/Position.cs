using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolProjectBusiness
{
    public class clsPosition
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int PositionID { get; set; }

       

        public string PositionName { get; set; }



        public clsPosition()
        {
            this.PositionID = default;

            

            this.PositionName = default;

            Mode = enMode.AddNew;
        }

        private clsPosition(int PositionID,  
           string PositionName)
        {
            this.PositionID = PositionID;

           

            this.PositionName = PositionName;

            Mode = enMode.Update;
        }

        private bool _AddNewPosition()
        {
            this.PositionID = (clsPositionData.AddNewPosition(this.PositionName));
            return PositionID != 0;
        }

        private bool _UpdatePosition()
        {
            return clsPositionData.UpdatePosition(PositionID,  
             PositionName);
        }

        public static bool DeletePosition(int PositionID)
        {
            return clsPositionData.DeletePosition(PositionID);
        }

        public static clsPosition Find(int PositionID)
        {

            int EmployeeID = default;
            string PositionName = default;



            bool IsFound = clsPositionData.GetPositionByPositionID
                          (PositionID,
               ref PositionName);

            if (IsFound)
            {
                return new clsPosition
                                (PositionID,
                 PositionName);

            }

            return null;

        }



        public static bool IsPositionExists(int PositionID)
        {

            return clsPositionData.IsPositionExists(PositionID);

        }

        public static DataTable GetAllPositions()
        {

            return clsPositionData.GetAllPositions();

        }

        public bool Save()
        {

            switch (Mode)
            {

                case enMode.AddNew:
                    if (_AddNewPosition())
                    {
                        Mode = enMode.Update;
                        return true;
                    }
                    else
                        return false;

                case enMode.Update:
                    return _UpdatePosition();

            }

            return false;
        }



    }
}
