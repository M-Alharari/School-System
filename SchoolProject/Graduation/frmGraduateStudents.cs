using SchoolProject.Business;
using SchoolProject.Global_Classes;
using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
 

namespace SchoolProject.Graduation
{
    public partial class frmGraduateStudents : Form
    {
        private DataTable _dtGraduation;
        private int _currentTermID;
        public frmGraduateStudents()
        {
            InitializeComponent();
        }

        private void frmGraduateStudents_Load(object sender, EventArgs e)
        {
            _currentTermID = clsTerm.GetCurrentTermIDSafe();
            if (_currentTermID <= 0)
            {
                MessageBox.Show("No active term. Cannot perform graduation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGraduate.Enabled = false;
                return;
            }

            if (!clsTerm.IsFinalTerm(_currentTermID))
            {
                MessageBox.Show("Graduation can only be done for the final term.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGraduate.Enabled = false;
            }

            LoadPreviewGraduationGrid(); // first load = predictions
        }

        private void LoadPreviewGraduationGrid()
        {
            _dtGraduation = clsGraduation.GetStudentsWithPredictedGraduation(_currentTermID);

            dgvGraduates.DataSource = _dtGraduation;
            FormatGridColumns();
            UpdatePreviewStats();
        }
        private void UpdateStats(DataTable dt, string passedColumn = "IsPredictedPassed")
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                lblPassedCount.Text = "Passed: 0";
                lblFailedCount.Text = "Failed: 0";
                lblTotalCount.Text = "Total Students: 0";
                return;
            }

            int passed = dt.AsEnumerable().Count(r => r[passedColumn] != DBNull.Value && Convert.ToBoolean(r[passedColumn]));
            int failed = dt.Rows.Count - passed;

            lblPassedCount.Text = $"Passed: {passed}";
            lblFailedCount.Text = $"Failed: {failed}";
            lblTotalCount.Text = $"Total Students: {dt.Rows.Count}";
        }


        private void FormatGridColumns()
        {
            // Make sure the DataGridView exists
            if (dgvGraduates == null)
                return;

            // Make sure it has columns
            if (dgvGraduates.Columns == null || dgvGraduates.Columns.Count == 0)
                return;

            // Hide unnecessary columns
            if (dgvGraduates.Columns.Contains("EnrollmentID"))
                dgvGraduates.Columns["EnrollmentID"].Visible = false;

            if (dgvGraduates.Columns.Contains("PersonID"))
                dgvGraduates.Columns["PersonID"].Visible = false;

            if (dgvGraduates.Columns.Contains("StudentID"))
                dgvGraduates.Columns["StudentID"].Visible = false;

            // Configure visible columns
            if (dgvGraduates.Columns.Contains("FullName"))
            {
                dgvGraduates.Columns["FullName"].HeaderText = "Full Name";
                dgvGraduates.Columns["FullName"].Width = 100;
            }

            if (dgvGraduates.Columns.Contains("GradeName"))
            {
                dgvGraduates.Columns["GradeName"].HeaderText = "Grade";
                dgvGraduates.Columns["GradeName"].Width = 70;
            }

            if (dgvGraduates.Columns.Contains("ClassName"))
            {
                dgvGraduates.Columns["ClassName"].HeaderText = "Class";
                dgvGraduates.Columns["ClassName"].Width = 70;
            }

            if (dgvGraduates.Columns.Contains("TotalScore"))
            {
                dgvGraduates.Columns["TotalScore"].HeaderText = "Total Score";
                dgvGraduates.Columns["TotalScore"].Width = 70;
            }

            if (dgvGraduates.Columns.Contains("MaxTotalScore"))
            {
                dgvGraduates.Columns["MaxTotalScore"].HeaderText = "Max Total Score";
                dgvGraduates.Columns["MaxTotalScore"].Width = 70;
            }

            if (dgvGraduates.Columns.Contains("Percentage"))
            {
                dgvGraduates.Columns["Percentage"].HeaderText = "Percentage";
                dgvGraduates.Columns["Percentage"].Width = 70;
            }

            if (dgvGraduates.Columns.Contains("PredictedLetterGrade"))
            {
                dgvGraduates.Columns["PredictedLetterGrade"].HeaderText = "Predicted Grade";
                dgvGraduates.Columns["PredictedLetterGrade"].Width = 60;
            }

            if (dgvGraduates.Columns.Contains("IsPredictedPassed"))
            {
                dgvGraduates.Columns["IsPredictedPassed"].HeaderText = "Predicted Pass";
                dgvGraduates.Columns["IsPredictedPassed"].Width = 80;
                dgvGraduates.Columns["IsPredictedPassed"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void UpdateFinalStats()
        {
            if (_dtGraduation == null || _dtGraduation.Rows.Count == 0)
            {
                lblPassedCount.Text = "Passed: 0";
                lblFailedCount.Text = "Failed: 0";
                lblTotalCount.Text = "Total Students: 0";
                return;
            }
            // Count students who passed (IsGraduated = true)
            int passed = _dtGraduation.AsEnumerable()
     .Count(r => r["Percentage"] != DBNull.Value && Convert.ToDecimal(r["Percentage"]) >= 50m);

            // Count failed as the remainder
            int failed = _dtGraduation.Rows.Count - passed;

            // Update labels
            lblPassedCount.Text = $"Passed: {passed}";
            lblFailedCount.Text = $"Failed: {failed}";
            lblTotalCount.Text = $"Total Students: {_dtGraduation.Rows.Count}";


        }

        private void LoadGraduationResultsFromDB()
        {
            //_dtGraduation = clsGraduation.GetGraduatedStudents(_currentTermID);

            dgvGraduates.DataSource = _dtGraduation;
            FormatGridColumns();
            UpdateFinalStats();
        }

        private void UpdatePreviewStats()
        {
            if (_dtGraduation == null || _dtGraduation.Rows.Count == 0)
            {
                lblPassedCount.Text = "Predicted Passed: 0";
                lblFailedCount.Text = "Predicted Failed: 0";
                lblTotalCount.Text = "Total Students: 0";
                return;
            }

            int passed = _dtGraduation.AsEnumerable()
                .Count(r => r["IsPredictedPassed"] != DBNull.Value && Convert.ToBoolean(r["IsPredictedPassed"]));

            int total = _dtGraduation.Rows.Count;
            int failed = total - passed;

            lblPassedCount.Text = $"Predicted Passed: {passed}";
            lblFailedCount.Text = $"Predicted Failed: {failed}";
            lblTotalCount.Text = $"Total Students: {total}";
        }

        private void UpdateGraduationStatsFromGrid()
        {
            if (dgvGraduates.Rows.Count == 0)
            {
                lblPassedCount.Text = "Predicted Passed: 0";
                lblFailedCount.Text = "Predicted Failed: 0";
                lblTotalCount.Text = "Total Students: 0";
                return;
            }

            int passed = 0;
            int failed = 0;
            int total = 0;

            foreach (DataGridViewRow row in dgvGraduates.Rows)
            {
                if (row.IsNewRow) continue;

                total++;

                bool isPassed = false;
                if (row.Cells["IsPredictedPassed"].Value != null)
                    isPassed = Convert.ToBoolean(row.Cells["IsPredictedPassed"].Value);

                if (isPassed) passed++;
                else failed++;
            }

            lblPassedCount.Text = $"Predicted Passed: {passed}";
            lblFailedCount.Text = $"Predicted Failed: {failed}";
            lblTotalCount.Text = $"Total Students: {total}";
        }

        private void UpdateGraduationStats()
        {
            if (_dtGraduation == null || _dtGraduation.Rows.Count == 0)
            {
                lblPassedCount.Text = "Passed Students: 0";
                lblFailedCount.Text = "Failed Students: 0";
                return;
            }

            int passed = 0;
            int failed = 0;

            foreach (DataRow row in _dtGraduation.Rows)
            {
                bool isPassed = Convert.ToBoolean(row["IsGraduated"]);
                if (isPassed) passed++;
                else failed++;
            }

            lblPassedCount.Text = $"Passed Students: {passed}";
            lblFailedCount.Text = $"Failed Students: {failed}";
        }

        private void UpdateGraduateStats()
        {
            DataTable dtTotals = clsScoresDetails.GetTotalScoresByEnrollment(_currentTermID);


            if (dtTotals == null || dtTotals.Rows.Count == 0)
            {
                lblPassedCount.Text = "Passed Students: 0";
                lblFailedCount.Text = "Failed Students: 0";
                return;
            }

            int passCount = 0;
            int failCount = 0;

            foreach (DataRow row in dtTotals.Rows)
            {
                decimal totalPercentage = Convert.ToDecimal(row["Percentage"]); // ✅ use Percentage, not MaxTotalScore

                if (totalPercentage >= 50m) passCount++;
                else failCount++;
            }

            lblPassedCount.Text = $"Passed Students: {passCount}";
            lblFailedCount.Text = $"Failed Students: {failCount}";
        }


        private void GraduateStudentsByEnrollment()
        {
            int termID = _currentTermID; // use the already loaded current term
            var currentTerm = clsTerm.Find(termID);

            if (currentTerm == null)
            {
                MessageBox.Show("Current term not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!clsTerm.IsFinalTerm(termID))
            {
                MessageBox.Show("You cannot graduate students because the current term is not marked as final.",
                                "Final Term Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int createdBy = clsGlobal.CurrentUser.UserID;
            DataTable dtEnrollments = clsScoreDetailsPerTerm.GetTotalScoresByEnrollment(termID); // ✅ filter by term

            if (dtEnrollments == null || dtEnrollments.Rows.Count == 0)
                return;

            int graduatedCount = 0, failedCount = 0, skippedCount = 0;

            foreach (DataRow row in dtEnrollments.Rows)
            {
                if (row["EnrollmentID"] == DBNull.Value) continue;

                int enrollmentID = Convert.ToInt32(row["EnrollmentID"]);
                decimal totalPercentage = clsGraduation.SafeDecimal(row["Percentage"]);
                bool passed = totalPercentage >= 50m;

                // skip if already graduated
                if (clsGraduation.IsAlreadyGraduated(enrollmentID, termID))
                {
                    skippedCount++;
                    continue;
                }

                bool success = clsGraduation.GraduateAndPromoteStudent(
                    enrollmentID, termID, totalPercentage, createdBy
                );

                if (success)
                {
                    if (passed) graduatedCount++;
                    else failedCount++;
                }
            }

            lblPassedCount.Text = $"Passed Students: {graduatedCount}";
            lblFailedCount.Text = $"Failed Students: {failedCount}";
            // lblSkipped.Text = $"Already Graduated (Skipped): {skippedCount}";

            MessageBox.Show($"Graduation complete.\n" +
                            $"Passed: {graduatedCount}\n" +
                            $"Failed: {failedCount}\n" +
                            $"Skipped (already graduated): {skippedCount}",
                            "Graduation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            PreviewGraduationResults(); // refresh grid after commit
        }

        private void PreviewGraduationResults()
        {
            // ✅ get only current term
            DataTable dtEnrollments = clsScoreDetailsPerTerm.GetTotalScoresByEnrollment(_currentTermID);

            if (dtEnrollments == null || dtEnrollments.Rows.Count == 0)
            {
                MessageBox.Show("No enrollment scores found for the current term.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Add extra columns for preview if missing
            if (!dtEnrollments.Columns.Contains("LetterGrade"))
                dtEnrollments.Columns.Add("LetterGrade", typeof(string));

            if (!dtEnrollments.Columns.Contains("IsPassed"))
                dtEnrollments.Columns.Add("IsPassed", typeof(bool));

            int passedCount = 0, failedCount = 0;

            foreach (DataRow row in dtEnrollments.Rows)
            {
                decimal totalPercentage = clsGraduation.SafeDecimal(row["Percentage"]);
                string letter = clsGraduation.GetLetterGrade(totalPercentage);
                bool passed = totalPercentage >= 50m;

                row["LetterGrade"] = letter;
                row["IsPassed"] = passed;

                if (passed) passedCount++;
                else failedCount++;
            }

            dgvGraduates.DataSource = dtEnrollments;
            lblPassedCount.Text = $"Passed Students: {passedCount}";
            lblFailedCount.Text = $"Failed Students: {failedCount}";
        }



        private void btnGraduate_Click(object sender, EventArgs e)
        {
            if (_dtGraduation == null || _dtGraduation.Rows.Count == 0)
            {
                MessageBox.Show("No students to graduate.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int graduatedCount = 0, failedCount = 0;

            foreach (DataRow row in _dtGraduation.Rows)
            {
                int enrollmentID = Convert.ToInt32(row["EnrollmentID"]);
                decimal percentage = clsGraduation.SafeDecimal(row["Percentage"]);


                bool success = clsGraduation.GraduateAndPromoteStudent(
                    enrollmentID,
                    _currentTermID,
                    percentage,
                    clsGlobal.CurrentUser.UserID
                );

                if (success && percentage >= 50m) graduatedCount++;
                else failedCount++;
            }

            MessageBox.Show($"Graduation complete.\nPassed: {graduatedCount}\nFailed: {failedCount}",
                            "Graduation", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadGraduationResultsFromDB(); // ✅ load actual DB results
        }

        private void LoadGraduationGrid()
        {
            // Get the students with predicted graduation
            _dtGraduation = clsGraduation.GetStudentsWithPredictedGraduation(_currentTermID);

            if (_dtGraduation == null)
                return;

            // Add IsGraduated column if it doesn't exist
            if (!_dtGraduation.Columns.Contains("IsGraduated"))
            {
                _dtGraduation.Columns.Add("IsGraduated", typeof(bool));
            }

            // Populate IsGraduated based on Percentage
            foreach (DataRow row in _dtGraduation.Rows)
            {
                if (row["Percentage"] != DBNull.Value)
                {
                    decimal percentage = clsGraduation.SafeDecimal(row["Percentage"]);

                    row["IsGraduated"] = percentage >= 50m; // mark as passed
                }
                else
                {
                    row["IsGraduated"] = false; // treat missing score as failed
                }
            }

            // Bind to DataGridView
            dgvGraduates.DataSource = _dtGraduation;
            FormatGridColumns();

            // Update graduation stats
            UpdateGraduationStatsFromGrid();
        }

    }
}


        //MessageBox.Show($"Graduation complete.\nPassed: {graduatedCount}\nFailed/Repeated: {repeatedCount}", "Graduation");

//// Refresh labels
//lblPassedCount.Text = $"Passed Students: {graduatedCount}";
//lblFailedCount.Text = $"Failed Students: {repeatedCount}";
