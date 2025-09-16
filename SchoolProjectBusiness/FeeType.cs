using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsFeeType
    {
        public int FeeTypeID { get; set; }
        public string FeeTypeName { get; set; }
        public string Description { get; set; }

        public static DataTable GetAll() => clsFeeTypeData.GetAllFeeTypes();

        public bool Save(int currentUserID)
        {
            if (FeeTypeID == 0) // New
                return clsFeeTypeData.AddFeeType(FeeTypeName, Description, currentUserID);
            else
                return clsFeeTypeData.UpdateFeeType(FeeTypeID, FeeTypeName, Description, currentUserID);
        }
        // Load a fee type by ID
        // 2️⃣ Optional: return DataTable directly
        public static DataTable FindByIDAsDataTable(int id)
        {
            return clsFeeTypeData.FindByID(id);
        }
        public static string GetFeeTypeName(int feeTypeID)
        {
            return clsFeeTypeData.GetFeeTypeNameByID(feeTypeID);
        }
    }
}
