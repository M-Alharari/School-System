using System;
using System.Data;
using SchoolProjectData;

namespace SchoolProjectBusiness
{
    public class clsPayroll
    {
        // ✅ Get payroll for specific month & year
        public static DataTable GetPayroll(int year, int month)
        {
            try
            {
                return clsPayrollData.GetPayrollByMonthYear(year, month);
            }
            catch (Exception ex)
            {
                throw new Exception("Business error in retrieving payroll data: " + ex.Message);
            }
        }

        // ✅ Add payroll payment (record a new payroll entry)
        public static bool AddPayrollPayment(int employeeID, int year, int month,
                                         decimal grossSalary, decimal totalDeductions,
                                         decimal netSalary, int createdByUserID)
        {
            try
            {
                // 1. Check if payroll already exists
                if (clsPayrollData.PayrollExists(employeeID, year, month))
                {
                    // Already exists → don't insert again
                    return false;
                }

                // 2. Add new payroll payment
                return clsPayrollData.AddPayrollPayment(employeeID, year, month,
                                                        grossSalary, totalDeductions,
                                                        netSalary, createdByUserID);
            }
            catch (Exception ex)
            {
                throw new Exception("Business error in adding payroll payment: " + ex.Message);
            }
        }

        // ✅ Mark payroll as paid
        public static bool MarkPayrollAsPaid(int employeeID, int year, int month)
        {
            try
            {
                return clsPayrollData.MarkPayrollAsPaid(employeeID, year, month);
            }
            catch (Exception ex)
            {
                throw new Exception("Business error in marking payroll as paid: " + ex.Message);
            }
        }

        public static bool ValidateAttendanceAndDeductions(int year, int month)
        {
            // 1. Check attendance exists for all employees
            if (!clsEmployeeAttendance.HasCompleteAttendance(year, month))
                return false;

            // 2. Check deductions are calculated
            if (!clsSalaryDeductionSummaryData.AreDeductionsCalculated(year, month))
                return false;

            return true;
        }

    }
}
