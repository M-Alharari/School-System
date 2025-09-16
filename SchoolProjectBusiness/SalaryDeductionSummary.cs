using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsSalaryDeductionSummary
    {
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public decimal TotalAbsenceDays { get; set; }
        public decimal TotalDeduction { get; set; }

        // Generate or update summary for a month
        public static bool GenerateMonthlySummary(int month, int year)
        {
            return clsSalaryDeductionSummaryData.GenerateMonthlySummary(month, year);
        }

        // Get summary for a month
        public static DataTable GetMonthlySummary(int month, int year)
        {
            return clsSalaryDeductionSummaryData.GetMonthlySummary(month, year);
        }
    }
}
