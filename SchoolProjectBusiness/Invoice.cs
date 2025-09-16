using SchoolProjectData;
using System;
using System.Data;

namespace SchoolProjectBusiness
{
    public class clsInvoice
    {
        public int InvoiceID { get; set; }
        public int EnrollmentID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsFullPayment { get; set; }
        public string Status { get; set; }
        public int CreatedByUserID { get; set; }

        public bool Save()
        {
            if (InvoiceID <= 0)
            {
                InvoiceID = clsInvoiceData.AddNew(EnrollmentID, TotalAmount, DueDate, IsFullPayment, CreatedByUserID);
                return InvoiceID > 0;
            }
            else
            {
                return clsInvoiceData.UpdateStatus(InvoiceID, Status);
            }
        }

        public static clsInvoice Find(int invoiceID)
        {
            DataRow row = clsInvoiceData.FindByID(invoiceID);
            if (row == null) return null;

            return new clsInvoice
            {
                InvoiceID = Convert.ToInt32(row["InvoiceID"]),
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                TotalAmount = Convert.ToDecimal(row["TotalAmount"]),
                DueDate = Convert.ToDateTime(row["DueDate"]),
                IsFullPayment = Convert.ToBoolean(row["IsFullPayment"]),
                Status = row["Status"].ToString()
            };
        }

        public static DataTable GetAll()
        {
            return clsInvoiceData.GetAll();
        }
    }
}
