using SchoolProjectData;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace SchoolProjectBusiness
{
    public class clsTerm
    {
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ModifiedByUserID { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsFinal { get; set; } // <-- added property
        public enum enMode { AddNew, Update }
        public enMode Mode { get; set; } = enMode.AddNew;

        // Get all terms
        public static DataTable GetAll()
        {
            return clsTermData.GetAllTerms();
        }

        // Insert new term
        private bool Add()
        {
            int newID = clsTermData.InsertTerm(TermName, StartDate, EndDate, CreatedByUserID);

            if (newID > 0)
            {
                this.TermID = newID;
                this.CreatedAt = DateTime.Now; // set locally (DB already has it)
                this.Mode = enMode.Update;
                return true;
            }
            return false;
        }

        // Update existing term
        private bool Update()
        {
            if (TermID <= 0) throw new Exception("Invalid Term ID");

            bool isUpdated = clsTermData.UpdateTerm(
                TermID, TermName, StartDate, EndDate, ModifiedByUserID ?? 0
            );

            if (isUpdated)
                this.ModifiedAt = DateTime.Now;

            return isUpdated;
        }

        // Save (Add or Update depending on mode)
        public bool Save()
        {
            if (this.Mode == enMode.AddNew)
                return this.Add();
            else
                return this.Update();
        }

        // Delete term
        public bool Delete()
        {
            if (TermID <= 0) throw new Exception("Invalid Term ID");
            return clsTermData.DeleteTerm(TermID);
        }

        // Find term
        public static clsTerm Find(int termID)
        {
            string name = "";
            DateTime start = DateTime.MinValue;
            DateTime end = DateTime.MinValue;
            int createdBy = 0;
            DateTime? createdAt = null;
            int? modifiedBy = null;
            DateTime? modifiedAt = null;

            bool found = clsTermData.Find(termID,
                ref name, ref start, ref end,
                ref createdBy, ref createdAt,
                ref modifiedBy, ref modifiedAt);

            if (!found)
                return null;

            return new clsTerm
            {
                TermID = termID,
                TermName = name,
                StartDate = start,
                EndDate = end,
                CreatedByUserID = createdBy,
                CreatedAt = createdAt ?? DateTime.MinValue,
                ModifiedByUserID = modifiedBy,
                ModifiedAt = modifiedAt,
                Mode = enMode.Update
            };
        }
        // Return the final term if exists, else term containing today, else null
        //public static clsTerm GetCurrentTerm()
        //{
        //    DataTable dtTerms = clsTerm.GetAll();

        //    // 1️⃣ Try to find the final term first
        //    foreach (DataRow row in dtTerms.Rows)
        //    {
        //        if (Convert.ToBoolean(row["IsFinal"]))
        //            return clsTerm.Find(Convert.ToInt32(row["TermID"]));
        //    }

        //    // 2️⃣ Fallback: term containing today
        //    DateTime today = DateTime.Today;
        //    foreach (DataRow row in dtTerms.Rows)
        //    {
        //        DateTime start = Convert.ToDateTime(row["StartDate"]);
        //        DateTime end = Convert.ToDateTime(row["EndDate"]);
        //        if (today >= start && today <= end)
        //            return clsTerm.Find(Convert.ToInt32(row["TermID"]));
        //    }

        //    // 3️⃣ Nothing found
        //    return null;
        //}
        public static clsTerm GetCurrentTerm()
        {
            DataTable dtTerms = clsTerm.GetAll();
            DateTime today = DateTime.Today;

            foreach (DataRow row in dtTerms.Rows)
            {
                if (row["StartDate"] == DBNull.Value || row["EndDate"] == DBNull.Value)
                    continue; // skip rows without dates

                DateTime start = Convert.ToDateTime(row["StartDate"]);
                DateTime end = Convert.ToDateTime(row["EndDate"]);

                if (today >= start && today <= end)
                    return clsTerm.Find(Convert.ToInt32(row["TermID"]));
            }

            // No valid term found for today
            return null;
        }



        // ترجع الـ TermID الحالي أو -1 إذا لم يوجد
        public static int GetCurrentTermIDSafe()
        {
            try
            {
                var term = clsTerm.GetCurrentTerm(); // original method
                if (term == null)
                {
                    MessageBox.Show("There is no active term currently.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return -1;
                }
                return term.TermID;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching the current term: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

        }

        // يمكن إضافة نسخة آمنة لأي دالة أخرى تحتاج TermID

        // In clsTerm
        public static int GetCurrentTermID()
        {
            clsTerm currentTerm = GetCurrentTerm(); // gets the active term object
            if (currentTerm == null)
                throw new Exception("No active term found.");

            return currentTerm.TermID; // now returns the ID as int
        }
        // Get next term object after a given TermID
        public static clsTerm GetNextTerm(int currentTermID)
        {
            DataTable dt = clsTermData.GetNextTermInfo(currentTermID);
            if (dt.Rows.Count == 0)
                return null; // or return current term if you prefer

            DataRow row = dt.Rows[0];
            return new clsTerm
            {
                TermID = Convert.ToInt32(row["TermID"]),
                TermName = row["TermName"].ToString(),
                StartDate = Convert.ToDateTime(row["StartDate"]),
                EndDate = Convert.ToDateTime(row["EndDate"]),
                IsActive = Convert.ToBoolean(row["IsActive"]),
                CreatedByUserID = Convert.ToInt32(row["CreatedByUserID"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                IsFinal = Convert.ToBoolean(row["IsFinal"]),
                Mode = enMode.Update
            };
        }

        // Get next term ID
     
        // Call this AFTER adding a term
        //public static void SetFinalTermAutomatically()
        //{
        //    DataTable terms = clsTermData.GetAllTerms(); // get all terms from DB

        //    if (terms.Rows.Count == 2) // only mark final if there are exactly 2 terms
        //    {
        //        // 1️⃣ Mark all terms as not final
        //        foreach (DataRow row in terms.Rows)
        //        {
        //            int termID = Convert.ToInt32(row["TermID"]);
        //            clsTermData.UpdateTermIsFinal(termID, false);
        //        }

        //        // 2️⃣ Mark the second term as final
        //        int secondTermID = Convert.ToInt32(terms.Rows[1]["TermID"]);
        //        clsTermData.UpdateTermIsFinal(secondTermID, true);
        //    }
        //}

        // Get the term that is marked as final
        public static clsTerm GetFinalTerm()
        {
            DataTable dt = clsTermData.GetAllTerms();
            DataRow finalRow = dt.AsEnumerable().FirstOrDefault(r => Convert.ToBoolean(r["IsFinal"]));

            if (finalRow == null) return null;

            return new clsTerm
            {
                TermID = Convert.ToInt32(finalRow["TermID"]),
                TermName = finalRow["TermName"].ToString(),
                StartDate = Convert.ToDateTime(finalRow["StartDate"]),
                EndDate = Convert.ToDateTime(finalRow["EndDate"]),
                IsActive = Convert.ToBoolean(finalRow["IsActive"]),
                IsFinal = true
            };
        }

        public static int CreateFirstTermOfNextYear()
        {
            // Get the last term of the current year
            DataTable dtLastTerm = GetAll().AsEnumerable()
                .OrderByDescending(r => Convert.ToDateTime(r["StartDate"]))
                .CopyToDataTable();

            if (dtLastTerm.Rows.Count == 0) return -1;

            DateTime lastTermEnd = Convert.ToDateTime(dtLastTerm.Rows[0]["EndDate"]);
            int nextYear = lastTermEnd.Year + 1;

            // Create new first term for next year
            clsTerm newTerm = new clsTerm
            {
                TermName = $"Term 1 {nextYear}",
                StartDate = new DateTime(nextYear, 1, 1),
                EndDate = new DateTime(nextYear, 3, 31), // adjust according to term length
                IsActive = true,
                IsFinal = false,
                 
            };

            if (newTerm.Save())
                return newTerm.TermID;

            return -1;
        }
        public static int GetNextTermID(int currentTermID, int createdBy)
        {
            // Get all terms
            DataTable dtTerms = clsTerm.GetAll();
            if (dtTerms.Rows.Count == 0)
                return -1;

            // Find current term
            var currentRow = dtTerms.AsEnumerable()
                .FirstOrDefault(r => Convert.ToInt32(r["TermID"]) == currentTermID);

            if (currentRow == null)
                return -1;

            bool isFinal = Convert.ToBoolean(currentRow["IsFinal"]);
            DateTime startDate = Convert.ToDateTime(currentRow["StartDate"]);
            int year = startDate.Year;

            if (!isFinal)
            {
                // مش نهائي → رجع الترم اللي بعده في نفس السنة
                var nextTermRow = dtTerms.AsEnumerable()
                    .Where(r => Convert.ToDateTime(r["StartDate"]).Year == year &&
                                Convert.ToDateTime(r["StartDate"]) > startDate)
                    .OrderBy(r => Convert.ToDateTime(r["StartDate"]))
                    .FirstOrDefault();

                if (nextTermRow != null)
                    return Convert.ToInt32(nextTermRow["TermID"]);
            }

            // لو ترم نهائي → دور على أول ترم في السنة الجديدة
            var firstNextYearRow = dtTerms.AsEnumerable()
                .Where(r => Convert.ToDateTime(r["StartDate"]).Year == year + 1)
                .OrderBy(r => Convert.ToDateTime(r["StartDate"]))
                .FirstOrDefault();

            if (firstNextYearRow != null)
                return Convert.ToInt32(firstNextYearRow["TermID"]);

            // 🚨 مفيش ترم للسنة الجديدة → نولّد أوتوماتيك
            int newYear = year + 1;

            // Term 1 → يبدأ سبتمبر وينتهي يناير
            int term1ID = clsTermData.AddNewTerm(
                "Term 1",
                new DateTime(newYear, 9, 1),           // StartDate
                new DateTime(newYear + 1, 1, 31),      // EndDate
                false,
                createdBy);

            // Term 2 → يبدأ فبراير وينتهي يونيو (ونهائي)
            int term2ID = clsTermData.AddNewTerm(
                "Term 2",
                new DateTime(newYear + 1, 2, 1),       // StartDate
                new DateTime(newYear + 1, 6, 30),      // EndDate
                true,
                createdBy);

            // رجّع أول ترم في السنة الجديدة
            return term1ID;
        }

        public static clsTerm GetFirstTermOfNextYear()
        {
            DataTable dtTerms = clsTerm.GetAll();
            if (dtTerms.Rows.Count == 0)
                return null;

            DateTime today = DateTime.Today;
            int currentYear = today.Year;

            // Find the first term whose StartDate is in the NEXT year
            var nextYearTermRow = dtTerms.AsEnumerable()
                .Where(r => Convert.ToDateTime(r["StartDate"]).Year == currentYear + 1)
                .OrderBy(r => Convert.ToDateTime(r["StartDate"]))
                .FirstOrDefault();

            if (nextYearTermRow != null)
            {
                int nextTermID = Convert.ToInt32(nextYearTermRow["TermID"]);
                return clsTerm.Find(nextTermID);
            }

            return null; // no term found for next year
        }

        // Example method: check if given term is final
        public static bool IsFinalTerm(int termID)
        {
            // call Data Layer
            return clsTermData.IsFinalTerm(termID);
        }

     
    }
}
