using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsPaymentType
    {
        public enum enMode { AddNew, Update }
        public enMode Mode = enMode.AddNew;

        public int PaymentTypeID { get; set; }
        public string PaymentTypeName { get; set; }
        public int CreatedByUserID { get; set; }
        public int? ModifiedByUserID { get; set; }

        public clsPaymentType()
        {
            PaymentTypeID = -1;
            PaymentTypeName = "";
            CreatedByUserID = -1;
        }

        private clsPaymentType(int id, string name)
        {
            PaymentTypeID = id;
            PaymentTypeName = name;
            Mode = enMode.Update;
        }

        public static DataTable GetAllPaymentTypes()
        {
            return clsPaymentTypeData.GetAllPaymentTypes();
        }

        public static clsPaymentType Find(int id)
        {
            string name = "";
            if (clsPaymentTypeData.GetPaymentTypeInfoByID(id, ref name))
                return new clsPaymentType(id, name);

            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    PaymentTypeID = clsPaymentTypeData.AddNewPaymentType(PaymentTypeName, CreatedByUserID);
                    return PaymentTypeID != -1;

                case enMode.Update:
                    return clsPaymentTypeData.UpdatePaymentType(PaymentTypeID, PaymentTypeName, ModifiedByUserID.Value);
            }

            return false;
        }

        public static bool DeletePaymentType(int id)
        {
            return clsPaymentTypeData.DeletePaymentType(id);
        }
    }
}
