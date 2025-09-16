using SchoolProjectData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SchoolProjectBusiness
{
    public class clsInstallment
    {
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int InstallmentID { get; set; }
        public int TuitionFeeID { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime? PaidDate { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public clsInstallment() { Mode = enMode.AddNew; }

        private clsInstallment(int installmentID, int tuitionFeeID, int installmentNumber,
            decimal amount, DateTime dueDate, bool isPaid, DateTime? paidDate,
            int createdByUserID, DateTime createdAt, int? modifiedByUserID, DateTime? modifiedAt)
        {
            InstallmentID = installmentID;
            TuitionFeeID = tuitionFeeID;
            InstallmentNumber = installmentNumber;
            Amount = amount;
            DueDate = dueDate;
            IsPaid = isPaid;
            PaidDate = paidDate;
            CreatedByUserID = createdByUserID;
            CreatedAt = createdAt;
            ModifiedByUserID = modifiedByUserID;
            ModifiedAt = modifiedAt;

            Mode = enMode.Update;
        }

        public static clsInstallment Find(int installmentID)
        {
            int tuitionFeeID = 0, installmentNumber = 0, createdByUserID = 0;
            decimal amount = 0;
            DateTime dueDate = DateTime.MinValue, createdAt = DateTime.MinValue;
            bool isPaid = false;
            DateTime? paidDate = null, modifiedAt = null;
            int? modifiedByUserID = null;

            if (clsInstallmentData.GetInstallmentByID(installmentID,
                ref tuitionFeeID, ref installmentNumber, ref amount, ref dueDate,
                ref isPaid, ref paidDate, ref createdByUserID, ref createdAt,
                ref modifiedByUserID, ref modifiedAt))
            {
                return new clsInstallment(installmentID, tuitionFeeID, installmentNumber,
                    amount, dueDate, isPaid, paidDate, createdByUserID,
                    createdAt, modifiedByUserID, modifiedAt);
            }
            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    InstallmentID = clsInstallmentData.AddNewInstallment(
                        TuitionFeeID, InstallmentNumber, Amount, DueDate, CreatedByUserID);

                    return (InstallmentID != -1);

                case enMode.Update:
                    return clsInstallmentData.UpdateInstallment(
                        InstallmentID, Amount, DueDate, IsPaid, PaidDate, ModifiedByUserID ?? 0);
            }
            return false;
        }
        // 1️⃣ Get the first unpaid installment for a tuition fee
        public static int GetFirstUnpaidInstallmentID(int tuitionFeeID)
        {
            return clsInstallmentData.GetFirstUnpaidInstallmentID(tuitionFeeID);
        }

        // 2️⃣ Get the amount of a specific installment
        public static decimal GetInstallmentAmount(int installmentID)
        {
            return clsInstallmentData.GetInstallmentAmount(installmentID);
        }

        // 3️⃣ Pay installment and optionally generate receipt
        public static void PayInstallment(int installmentID, int userID)
        {
            clsInstallmentData.MarkInstallmentAsPaid(installmentID, userID);
        }
        public static bool Delete(int installmentID) =>
            clsInstallmentData.DeleteInstallment(installmentID);

        public static DataTable GetAllByTuitionFee(int tuitionFeeID) =>
            clsInstallmentData.GetAllInstallmentsByTuitionFee(tuitionFeeID);

        public bool MarkAsPaid(int userID, DateTime paidDate)
        {
            try
            {
                PaidDate = paidDate;
                ModifiedByUserID = userID;
                ModifiedAt = DateTime.Now;

                // isPaid is true if PaidDate is set
                bool isPaid = PaidDate.HasValue;

                // Update DB
                return clsInstallmentData.UpdatePaidDateAndIsPaid(
                    InstallmentID,
                    PaidDate,
                    isPaid,
                    ModifiedByUserID,
                    ModifiedAt
                );
            }
            catch
            {
                return false;
            }
        }

        public static bool MarkFirstInstallmentAsPaid(int tuitionFeeID, int userID)
        {
            // Get all installments as clsInstallment objects
            var dt = clsInstallmentData.GetAllInstallmentsByTuitionFee(tuitionFeeID);

            // Convert DataTable to List<clsInstallment>
            var installments = dt.AsEnumerable()
                .Select(row => new clsInstallment
                {
                    InstallmentID = Convert.ToInt32(row["InstallmentID"]),
                    TuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]),
                    InstallmentNumber = Convert.ToInt32(row["InstallmentNumber"]),
                    Amount = Convert.ToDecimal(row["Amount"]),
                    DueDate = Convert.ToDateTime(row["DueDate"]),
                    PaidDate = row["PaidDate"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(row["PaidDate"]),
                    IsPaid = row["PaidDate"] != DBNull.Value
                })
                .OrderBy(i => i.InstallmentNumber) // sort by InstallmentNumber
                .ToList();

            // Find the first unpaid installment and mark as paid
            foreach (var installment in installments)
            {
                if (!installment.IsPaid)
                {
                    return installment.MarkAsPaid(userID, DateTime.Now);
                }
            }

            return false; // all installments already paid
        }

        public static DataTable GetInstallmentSummaryByTuitionFeeID(int tuitionFeeID)
        {
            DataTable dt = clsInstallmentData.GetInstallmentsWithStudentNamesByTuitionFee(tuitionFeeID);
            DataView dv = dt.DefaultView;
            dv.Sort = "InstallmentNumber ASC";
            return dv.ToTable(); // IsPaid already in the table
        }

        // Load all installments or by tuitionFeeID
        public static DataTable GetAllInstallmentsAsDataTable(int? tuitionFeeID = null)
        {
            // Get full DataTable from data layer
            DataTable dt = clsInstallmentData.GetAllInstallments(tuitionFeeID);

            // Add a calculated IsPaid column (based on PaidDate)
            if (!dt.Columns.Contains("IsPaid"))
                dt.Columns.Add("IsPaid", typeof(bool), "IIF(PaidDate IS NOT NULL, True, False)");

            // Optional: sort by InstallmentNumber
            DataView dv = dt.DefaultView;
            dv.Sort = "InstallmentNumber ASC";

            return dv.ToTable();
        }


    }
}
