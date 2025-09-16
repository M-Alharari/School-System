using DocumentFormat.OpenXml.Wordprocessing;
using SchoolProjectData;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SchoolProjectBusiness
{
    public class clsTuitionPayment
    {
        public enum enPaymentMethod
        {
            Tuition = 1,
            Full = 2,
            Installment = 3,
            LibraryFee = 4,
            LabFee = 5,
            TransportFee = 6,
            Other = 99
        }
        public enum enPaymentType
        {
            Full = 1,
            Installment = 2
        }

        public enum enInstallmentFrequency
        {
            None = 0,
            Monthly = 1,
            Quarterly = 2,
            SemiAnnual = 3,
            Yearly = 4,
            Experimental = 5
        }

        public int TuitionFeeID { get; set; }
        public int EnrollmentID { get; set; }
        public clsStudent StudentInfo { get; set; }
        public enPaymentType PaymentMode { get; set; } = enPaymentType.Full;
        public enInstallmentFrequency InstallmentFrequencyID { get; set; } = enInstallmentFrequency.None;
        public decimal TotalFees { get; set; }
        public decimal PaidAmount { get; set; }
      
        public bool IsFullyPaid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        // ✅ New property
        public string FullName { get; set; }
        public bool Save()
        {
            if (TuitionFeeID == 0)
            {
                TuitionFeeID = clsTuitionPaymentData.AddNew(
                    EnrollmentID,
                    (int)PaymentMode,
                    (int)InstallmentFrequencyID,
                    TotalFees,
                    PaidAmount,
                   
                    CreatedByUserID
                );
                return TuitionFeeID > 0;
            }
            else
            {
                return clsTuitionPaymentData.Update(
                    TuitionFeeID,
                    (int)PaymentMode,
                    (int)InstallmentFrequencyID,
                    TotalFees,
                    PaidAmount,
                    
                    ModifiedByUserID
                );
            }
        }

        public static clsTuitionPayment FindPaymentByStudentID(int studentID)
        {
            DataRow row = clsTuitionPaymentData.FindByStudentID(studentID);
            if (row == null) return null;

            return MapDataRowToPayment(row);
        }

        public static clsTuitionPayment FindPaymentByTuitionFeeID(int tuitionFeeID)
        {
            DataRow row = clsTuitionPaymentData.FindByTuitionFeeID(tuitionFeeID);
            if (row == null) return null;

            return MapDataRowToPayment(row);
        }
        public static clsTuitionPayment FindPaymentByEnrollmentID(int enrollmentID)
        {
            DataRow row = clsTuitionPaymentData.FindByEnrollmentID2(enrollmentID);
            if (row == null) return null;

            return MapDataRowToPayment(row);
        }

        public static string FindStudentFullName(int tuitionFeeID)
        {
            DataRow row = clsTuitionPaymentData.FindByTuitionFeeID(tuitionFeeID);
            if (row == null) return "Unknown";

            int studentID = Convert.ToInt32(row["EnrollmentID"]);
            clsStudent student = clsStudent.FindStudentByEnrollmentID(studentID);

            return student?.PersonInfo?.FullName ?? "Unknown";
        }
        // ✅ Load enrollment by EnrollmentID
        public static clsEnrollment Find(int enrollmentID)
        {
            int studentID = 0, classID = 0, gradeID = 0, termID = 0, createdByUserID = 0;
            DateTime enrollmentDate = DateTime.MinValue, createdAt = DateTime.MinValue, modifiedAt = DateTime.MinValue;
            bool isActive = false;
            string modifiedByUser = "";

            bool found = clsEnrollmentData.GetEnrollmentByEnrollmentID(
                enrollmentID,
                ref studentID, ref classID, ref gradeID, ref termID,
                ref enrollmentDate, ref isActive,
                ref createdByUserID, ref createdAt, ref modifiedByUser, ref modifiedAt);

            if (found)
            {
                return new clsEnrollment
                {
                    EnrollmentID = enrollmentID,
                    StudentID = studentID,
                    ClassID = classID,
                    GradeID = gradeID,
                    TermID = termID,
                    EnrollmentDate = enrollmentDate,
                    IsActive = isActive,
                    CreatedByUserID = createdByUserID,
                    CreatedAt = createdAt,
                    ModifiedByUser = modifiedByUser,
                    ModifiedAt = modifiedAt  
                };
            }

            return null; // not found
        }
        public static bool UpdateTuitionPaymentStatus(int tuitionFeeID, decimal paidAmount, bool isFullyPaid)
        {
            return clsTuitionPaymentData.UpdateTuitionPaymentStatus(tuitionFeeID, paidAmount, isFullyPaid);
        }

        private static clsTuitionPayment MapDataRowToPayment(DataRow row)
        {
            return new clsTuitionPayment
            {
                TuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]),
                EnrollmentID = Convert.ToInt32(row["EnrollmentID"]),
                PaymentMode = (enPaymentType)Convert.ToInt32(row["PaymentMode"]),
                InstallmentFrequencyID = row.Table.Columns.Contains("InstallmentFrequencyID") && row["InstallmentFrequencyID"] != DBNull.Value
                    ? (enInstallmentFrequency)Convert.ToInt32(row["InstallmentFrequencyID"])
                    : enInstallmentFrequency.None,
                TotalFees = Convert.ToDecimal(row["TotalFees"]),
                PaidAmount = Convert.ToDecimal(row["PaidAmount"]),

                IsFullyPaid = row.Table.Columns.Contains("IsFullyPaid") && row["IsFullyPaid"] != DBNull.Value
                    ? Convert.ToBoolean(row["IsFullyPaid"])
                    : false,
                PaymentDate = row.Table.Columns.Contains("PaymentDate") && row["PaymentDate"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["PaymentDate"])
                    : null,

                CreatedByUserID = row.Table.Columns.Contains("CreatedBy") && row["CreatedBy"] != DBNull.Value
                    ? Convert.ToInt32(row["CreatedBy"])
                    : 0,
                CreatedDate = row.Table.Columns.Contains("CreatedDate") && row["CreatedDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["CreatedDate"])
                    : DateTime.Now,
                ModifiedByUserID = row.Table.Columns.Contains("ModifiedBy") && row["ModifiedBy"] != DBNull.Value
                    ? Convert.ToInt32(row["ModifiedBy"])
                    : (int?)null,
                ModifiedDate = row.Table.Columns.Contains("ModifiedDate") && row["ModifiedDate"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["ModifiedDate"])
                    : null,

                // ✅ New mapping
                FullName = row.Table.Columns.Contains("FullName") && row["FullName"] != DBNull.Value
                    ? row["FullName"].ToString()
                    : string.Empty
            };
        }


        public static DataTable GetAllTuition()
        {
            return clsTuitionPaymentData.GetAllTuitionPayments();
        }



        public void PayInstallmentAndGenerateReceipt(int installmentID, decimal amount, int paymentTypeID, int userID)
        {
            // ✅ Call data layer to mark as paid
            clsInstallmentData.MarkInstallmentAsPaid(installmentID, userID);

            // ✅ Generate receipt
            clsReceipts receipt = new clsReceipts
            {
                ReceiptNumber = clsReceipts.GenerateReceiptNumber(),
                TuitionFeeID = this.TuitionFeeID,
                InstallmentID = installmentID,
                Amount = amount,
                PaymentDate = DateTime.Now,
               
                CreatedByUserID = userID,
                ModifiedByUserID = userID,
                ModifiedDate = DateTime.Now
            };
            receipt.Save();
        }


        // ✅ Business method to get payments by EnrollmentID
        public static DataTable GetPaymentsByEnrollmentID(int enrollmentID)
        {
            return clsTuitionPaymentData.GetPaymentsByEnrollmentID(enrollmentID);
        }

        // (Optional) Business method to check if a student has any payments
        public static bool HasPayments(int enrollmentID)
        {
            DataTable dt = clsTuitionPaymentData.GetPaymentsByEnrollmentID(enrollmentID);
            return dt.Rows.Count > 0;
        }


    }
}
