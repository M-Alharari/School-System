using SchoolProject.Global_Classes;
using SchoolProject.Terms;
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

namespace SchoolProject.Assessment_and_Exams
{
    public partial class EnterScores : Form
    {
        private int _StudentID;
        private clsScoreDetailsPerTerm _ScoreDetail;
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;

        private bool _isLoading = false;


        public EnterScores(int studentID)
        {
            InitializeComponent();
            _StudentID = studentID;

            dgvScores.CellValueChanged += dgvScores_CellValueChanged;
            dgvScores.EditingControlShowing += dgvScores_EditingControlShowing;
        }
      
           

        private void EnterScores_Load(object sender, EventArgs e)
        {
            _PrepareDataGridView();
            _LoadTerms();
        }


        private void _ApplyFinalTermRowStyles()
        {
            var currentTerm = clsTerm.Find(Convert.ToInt32(cbTerms.SelectedValue));
            bool isFinalTerm = currentTerm?.IsFinal ?? false;

            foreach (DataGridViewRow row in dgvScores.Rows)
            {
                row.DefaultCellStyle.BackColor = isFinalTerm ? Color.White : Color.LightGray;
                row.ReadOnly = !isFinalTerm;
            }
        }


        private void _PrepareDataGridView()
        {
            dgvScores.Columns.Clear();
            dgvScores.Columns.Add(new DataGridViewTextBoxColumn { Name = "SubjectName", HeaderText = "Subject", ReadOnly = true });
            dgvScores.Columns.Add(new DataGridViewTextBoxColumn { Name = "TestScore", HeaderText = "Test Score (Out of 30)" });
            dgvScores.Columns.Add(new DataGridViewTextBoxColumn { Name = "ExamScore", HeaderText = "Exam Score (Out of 70)" });
            dgvScores.Columns.Add(new DataGridViewTextBoxColumn { Name = "ScaledScore", HeaderText = "Scaled Score (Out of 100)", ReadOnly = true });
            dgvScores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void _LoadTerms()
        {
            _isLoading = true;

            DataTable dtTerms = clsTerm.GetAll();
            cbTerms.DataSource = dtTerms;
            cbTerms.DisplayMember = "TermName";
            cbTerms.ValueMember = "TermID";

            cbTerms.SelectedIndexChanged -= cbTerms_SelectedIndexChanged;
            cbTerms.SelectedIndexChanged += cbTerms_SelectedIndexChanged;

            int currentTermID = clsTerm.GetCurrentTermIDSafe();
            if (currentTermID != -1)
                cbTerms.SelectedValue = currentTermID;

            _isLoading = false;

            cbTerms_SelectedIndexChanged(cbTerms, EventArgs.Empty);
        }

        private void _LoadSubjectsIntoGrid(bool isFinalTerm)
        {
            dgvScores.Rows.Clear();

            int? enrollmentID = clsEnrollment.GetActiveEnrollmentIDByStudentID(_StudentID);
            if (enrollmentID == null)
            {
                MessageBox.Show("No active enrollment for this student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int termID = Convert.ToInt32(cbTerms.SelectedValue);
            DataTable subjects = clsSubject.GetAllSubjects();

            // Get only scores for this specific enrollment and term
            DataTable dtScores = clsScoreDetailsPerTerm.GetScoresByEnrollmentAndTerm(_StudentID, termID);

            foreach (DataRow subj in subjects.Rows)
            {
                int rowIndex = dgvScores.Rows.Add();
                dgvScores.Rows[rowIndex].Cells["SubjectName"].Value = subj["SubjectName"];
                dgvScores.Rows[rowIndex].Tag = subj["SubjectID"]; // store SubjectID

                // Look for a score row for this subject
                var scoreRow = dtScores.AsEnumerable()
                                       .FirstOrDefault(r => Convert.ToInt32(r["SubjectID"]) == Convert.ToInt32(subj["SubjectID"]));

                if (scoreRow != null)
                {
                    dgvScores.Rows[rowIndex].Cells["TestScore"].Value = scoreRow["TestScore"];
                    dgvScores.Rows[rowIndex].Cells["ExamScore"].Value = scoreRow["ExamScore"];
                }

                _UpdateScaledScore(dgvScores.Rows[rowIndex]);

                // Set read-only / color based on final term
                dgvScores.Rows[rowIndex].ReadOnly = !isFinalTerm;
                dgvScores.Rows[rowIndex].DefaultCellStyle.BackColor = isFinalTerm ? Color.White : Color.LightGray;
            }

            lblRecordCount.Text = dgvScores.Rows.Count.ToString();
        }

        void _UpdateScaledScore(DataGridViewRow row)
        {
            if (row == null) return;

            decimal testScore = 0m, examScore = 0m;

            // Clamp TestScore to 0-30
            if (!decimal.TryParse(row.Cells["TestScore"].Value?.ToString(), out testScore))
                testScore = 0;
            else if (testScore > 30) testScore = 30;
            else if (testScore < 0) testScore = 0;

            row.Cells["TestScore"].Value = testScore;

            // Clamp ExamScore to 0-70
            if (!decimal.TryParse(row.Cells["ExamScore"].Value?.ToString(), out examScore))
                examScore = 0;
            else if (examScore > 70) examScore = 70;
            else if (examScore < 0) examScore = 0;

            row.Cells["ExamScore"].Value = examScore;

            decimal scaled = (testScore / 30m) * 30m + (examScore / 70m) * 70m;
            row.Cells["ScaledScore"].Value = scaled.ToString("0.##");

            _UpdateAverageLabel(); // update live
        }


        private void _UpdateAverageLabel()
        {
            if (dgvScores.Rows.Count == 0) return;

            decimal total = 0;
            int count = 0;

            foreach (DataGridViewRow row in dgvScores.Rows)
            {
                if (row.IsNewRow) continue;
                if (decimal.TryParse(row.Cells["ScaledScore"].Value?.ToString(), out decimal val))
                {
                    total += val;
                    count++;
                }
            }

            decimal avg = count > 0 ? total / count : 0;
            lblAverageTotal.Text = $"Avg. Total: {avg:0.##}%";
        }

        private void ScoreCell_TextChanged(object sender, EventArgs e)
        {
            if (dgvScores.CurrentCell?.RowIndex < 0) return;
            _UpdateScaledScore(dgvScores.Rows[dgvScores.CurrentCell.RowIndex]);
        }
        private void dgvScores_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _UpdateScaledScore(dgvScores.Rows[e.RowIndex]);
        }

        private void dgvScores_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.TextChanged -= ScoreCell_TextChanged;
                tb.TextChanged += ScoreCell_TextChanged;

                tb.KeyPress -= ScoreCell_KeyPress;
                tb.KeyPress += ScoreCell_KeyPress;
            }
        }
     


        private void btnSave_Click(object sender, EventArgs e)
        {
            int? enrollmentID = clsEnrollment.GetActiveEnrollmentIDByStudentID(_StudentID);
            if (enrollmentID == null)
            {
                MessageBox.Show("No active enrollment found for this student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int termID = Convert.ToInt32(cbTerms.SelectedValue);
            int currentUserID = clsGlobal.CurrentUser.UserID;
            bool allSuccess = true;

            DataTable dtScores = clsScoreDetailsPerTerm.GetScoresByEnrollmentAndTerm(_StudentID, termID);

            foreach (DataGridViewRow row in dgvScores.Rows)
            {
                if (row.IsNewRow || row.Tag == null) continue;

                int subjectID = Convert.ToInt32(row.Tag);
                decimal testScore = decimal.TryParse(row.Cells["TestScore"].Value?.ToString(), out decimal ts) ? ts : 0;
                decimal examScore = decimal.TryParse(row.Cells["ExamScore"].Value?.ToString(), out decimal es) ? es : 0;

                var existingRow = dtScores.AsEnumerable()
                                          .FirstOrDefault(r => Convert.ToInt32(r["SubjectID"]) == subjectID);

                if (existingRow == null)
                {
                    _ScoreDetail = new clsScoreDetailsPerTerm
                    {
                        EnrollmentID = _StudentID,
                        SubjectID = subjectID,
                        TermID = termID,
                        TestScore = testScore,
                        ExamScore = examScore,
                        CreatedBy = currentUserID
                    };
                }
                else
                {
                    Mode = enMode.Update;
                    _ScoreDetail = clsScoreDetailsPerTerm.Find(Convert.ToInt32(existingRow["ScoreDetailID"]));
                    if (_ScoreDetail != null)
                    {
                        _ScoreDetail.TestScore = testScore;
                        _ScoreDetail.ExamScore = examScore;
                        _ScoreDetail.ModifiedBy = currentUserID;
                    }
                }

                if (!_ScoreDetail.Save())
                    allSuccess = false;

                row.Tag = _ScoreDetail;
                _UpdateScaledScore(row);
            }

            MessageBox.Show(allSuccess ? "Scores saved successfully." : "Some scores could not be saved.",
                            "Save Results", MessageBoxButtons.OK);
        }


        private void ScoreCell_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow digits, backspace, and dot only
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;

            // Optional: allow only one dot
            TextBox tb = sender as TextBox;
            if (e.KeyChar == '.' && tb.Text.Contains('.'))
                e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbTerms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return; // skip while loading

            if (cbTerms.SelectedValue == null) return;

            int selectedTermID;

            if (cbTerms.SelectedValue is DataRowView drv)
                selectedTermID = Convert.ToInt32(drv["TermID"]);
            else
                selectedTermID = Convert.ToInt32(cbTerms.SelectedValue);

            bool isFinalTerm = clsTerm.IsFinalTerm(selectedTermID);
            

            lblTitle.Text = isFinalTerm
                ? $"Final Term – Entering Scores"
                : $"Term: {cbTerms.Text}";

            // Show notice **only after initial load**
            if (!isFinalTerm && !_isLoading)
                MessageBox.Show(
                    "You are entering scores for a non-final term. These scores will not be used for graduation.",
                    "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);

            _LoadSubjectsIntoGrid(isFinalTerm);
        }
    }
    
}
