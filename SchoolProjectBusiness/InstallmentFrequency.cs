using System;
using System.Data;
using SchoolProjectData; // make sure your data layer namespace is referenced

namespace SchoolProjectBusiness
{
    public static class clsInstallmentFrequency
    {
        /// <summary>
        /// Returns all installment frequencies (ID and Name) from the table
        /// </summary>
        public static DataTable GetAll()
        {
            return clsInstallmentFrequencyData.GetAll(); // call the data layer
        }

        /// <summary>
        /// Get the number of installments for a given frequency
        /// </summary>
        public static int GetInstallmentCount(int frequencyID)
        {
            switch (frequencyID)
            {
                case 1: return 12; // Monthly
                case 2: return 4;  // Quarterly
                case 3: return 2;  // SemiAnnual
                case 4: return 1;  // Yearly
                case 5: return 6;  // Experimental
                default: return 1;
            }
        }

        /// <summary>
        /// Get the next due date based on frequency
        /// </summary>
        public static DateTime GetNextDueDate(DateTime currentDate, int frequencyID)
        {
            switch (frequencyID)
            {
                case 1: return currentDate.AddMonths(1);
                case 2: return currentDate.AddMonths(3);
                case 3: return currentDate.AddMonths(6);
                case 4: return currentDate.AddYears(1);
                case 5: return currentDate.AddSeconds(10); // Experimental
                default: return currentDate.AddMonths(1);
            }
        }
    }
}
