using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsReceipts
    {
        public int ReceiptID { get; set; }
        public string ReceiptNumber { get; set; }
        public int TuitionFeeID { get; set; }
        public int? InstallmentID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; private set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsInstallmentMode
        {
            get { return InstallmentID.HasValue; }
        }

        public clsReceipts() { }

        private clsReceipts(DataRow row)
        {
            ReceiptID = Convert.ToInt32(row["ReceiptID"]);
            ReceiptNumber = row["ReceiptNumber"].ToString();
            TuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]);
            InstallmentID = row["InstallmentID"] == DBNull.Value ? null : (int?)row["InstallmentID"];
            Amount = Convert.ToDecimal(row["Amount"]);
            PaymentDate = Convert.ToDateTime(row["PaymentDate"]);
            Notes = row["Notes"] == DBNull.Value ? null : row["Notes"].ToString();
            CreatedByUserID = Convert.ToInt32(row["CreatedByUserID"]);
            CreatedDate = Convert.ToDateTime(row["CreatedDate"]);
            ModifiedByUserID = row["ModifiedByUserID"] == DBNull.Value ? null : (int?)row["ModifiedByUserID"];
            ModifiedDate = row["ModifiedDate"] == DBNull.Value ? null : (DateTime?)row["ModifiedDate"];
        }

        // Save (Insert or Update)
        public bool Save()
        {
            if (ReceiptID == 0) // new receipt → Insert
            {
                ReceiptID = clsReceiptsData.AddReceipt(
                    ReceiptNumber,
                    TuitionFeeID,
                   InstallmentID ?? null,

                    Amount,
                    PaymentDate,
                    Notes,
                    CreatedByUserID,
                    ModifiedByUserID ?? 0,
                    ModifiedDate ?? DateTime.Now
                );
                return (ReceiptID > 0);
            }
            else // existing receipt → Update
            {
                clsReceiptsData.UpdateReceipt(
                    ReceiptID,
                    ReceiptNumber,
                    TuitionFeeID,
                    InstallmentID ?? 0,
                    Amount,
                    PaymentDate,
                    Notes,
                    ModifiedByUserID ?? 0,
                    ModifiedDate ?? DateTime.Now
                );
                return true;
            }
        }

        // Delete
        public static bool Delete(int receiptID)
        {
            clsReceiptsData.DeleteReceipt(receiptID);
            return true;
        }

        // Get by ID
        public static clsReceipts Find(int receiptID)
        {
            DataRow row = clsReceiptsData.GetReceiptByID(receiptID);
            if (row == null) return null;
            return new clsReceipts(row);
        }

        // Get all
        public static DataTable GetAll()
        {
            return clsReceiptsData.GetAllReceipts();
        }

        // Get by TuitionFeeID
        public static DataTable GetByTuitionFee(int tuitionFeeID)
        {
            return clsReceiptsData.GetReceiptsByTuitionFee(tuitionFeeID);
        }
      

        public static string GenerateReceiptNumber()
        {
            int year = DateTime.Now.Year;
            string prefix = $"RCPT-{year}-";

            string lastNumber = clsReceiptsData.GetLastReceiptNumberForYear(year);
            int nextSeq = 1;

            if (!string.IsNullOrEmpty(lastNumber))
            {
                string[] parts = lastNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastSeq))
                    nextSeq = lastSeq + 1;
            }

            return $"{prefix}{nextSeq:D5}";
        }


        // Business layer method to get receipts by tuition fee
        public static DataTable GetReceiptsByTuitionFee(int tuitionFeeID)
        {
            return clsReceiptsData.GetReceiptsByTuitionFee(tuitionFeeID);
        }













    }
}
