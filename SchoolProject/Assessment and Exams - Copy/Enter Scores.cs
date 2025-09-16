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

namespace SchoolProject.Assessment_and_Exams
{
    public partial class TypeScores : Form
    {
        private int _StudentID;
        private enum enMode { AddNew = 0, Update = 1 }
        private enMode Mode = enMode.AddNew;
        public TypeScores(int studentID)
        {
            InitializeComponent();
            _StudentID = studentID;

            dgvScores.CellValueChanged += dgvScores_CellValueChanged;
            dgvScores.EditingControlShowing += dgvScores_EditingControlShowing;

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

        private void _LoadSubjectsIntoGrid()
        {
            dgvScores.Rows.Clear();

            int? enrollmentID = clsEnrollment.GetActiveEnrollmentIDByStudentID(_StudentID);
            if (enrollmentID == null)
            {
                MessageBox.Show("No active enrollment for this student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int currentTermID = clsTerm.GetCurrentTermID();

            DataTable subjects = clsSubject.GetAllSubjects();
            DataTable dtScores = clsScoresDetails.LoadByEnrollmentAndTerm(enrollmentID.Value, currentTermID);

            foreach (DataRow subj in subjects.Rows)
            {
                int rowIndex = dgvScores.Rows.Add();
                dgvScores.Rows[rowIndex].Cells["SubjectName"].Value = subj["SubjectName"];
                dgvScores.Rows[rowIndex].Tag = subj["SubjectID"];

                int subjectID = Convert.ToInt32(subj["SubjectID"]);

                // --- Try to get scores for current term ---
                var scoreRow = dtScores?.AsEnumerable()
                    .FirstOrDefault(r => Convert.ToInt32(r["SubjectID"]) == subjectID);

                if (scoreRow != null)
                {
                    dgvScores.Rows[rowIndex].Cells["TestScore"].Value = scoreRow["TestScore"];
                    dgvScores.Rows[rowIndex].Cells["ExamScore"].Value = scoreRow["ExamScore"];
                    _UpdateScaledScore(dgvScores.Rows[rowIndex]);
                    Mode = enMode.Update;
                }
                else
                {
                    // --- If no current term scores, try previous term ---
                    int prevTermID = currentTermID - 1;
                    if (prevTermID > 0)
                    {
                        var prevScore = clsScoresDetails.LoadByEnrollmentTermAndSubject(enrollmentID.Value, prevTermID, subjectID);
                        if (prevScore != null)
                        {
                            dgvScores.Rows[rowIndex].Cells["TestScore"].Value = prevScore.TestScore;
                            dgvScores.Rows[rowIndex].Cells["ExamScore"].Value = prevScore.ExamScore;
                            _UpdateScaledScore(dgvScores.Rows[rowIndex]);

                            // Optional: mark them as "carried forward"
                            dgvScores.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightYellow;
                        }
                    }
                }
            }
        }




        private void frmClassResultsSummary_Load(object sender, EventArgs e)
        {
            _PrepareDataGridView();
            _LoadSubjectsIntoGrid();

            // Check if current term is final
            int currentTermID = clsTerm.GetCurrentTermID();
            var term = clsTerm.Find(currentTermID); // assuming you have Find/Load by ID

            if (term != null && !term.IsFinal) // if the term is not final
            {
               /* dgvScores.ReadOnly = true;*/   // disable editing
                MessageBox.Show("You cannot enter scores until the final term.",
                                "Term Locked", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid! Hover over the red icon(s) to see the error.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int? enrollmentID = clsEnrollment.GetActiveEnrollmentIDByStudentID(_StudentID);
            if (enrollmentID == null)
            {
                MessageBox.Show("No active enrollment found for this student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int termID = clsTerm.GetCurrentTermID();
            string currentUser = clsGlobal.CurrentUser.UserName;

            bool allSuccess = true;

            foreach (DataGridViewRow row in dgvScores.Rows)
            {
                if (row.IsNewRow || row.Tag == null) continue;

                int subjectID = Convert.ToInt32(row.Tag);

                decimal? testScore = null, examScore = null;

                if (decimal.TryParse(row.Cells["TestScore"].Value?.ToString(), out decimal t))
                    testScore = Math.Min(t, 30m); // clamp max 30
                if (decimal.TryParse(row.Cells["ExamScore"].Value?.ToString(), out decimal ex))
                    examScore = Math.Min(ex, 70m); // clamp max 70

                var existing = clsScoresDetails.LoadByEnrollmentTermAndSubject(enrollmentID.Value, termID, subjectID);

                clsScoresDetails score;
                if (existing == null)
                {
                    score = new clsScoresDetails
                    {
                        EnrollmentID = enrollmentID.Value,
                        TermID = termID,
                        SubjectID = subjectID,
                        CreatedBy = currentUser
                    };
                }
                else
                {
                    score = existing;
                    if (string.IsNullOrEmpty(score.CreatedBy))
                        score.CreatedBy = currentUser;
                }

                // أهم خطوة ↓↓↓
                score.TestScore = testScore;
                score.ExamScore = examScore;
                score.ModifiedBy = currentUser;

                if (!score.Save())
                    allSuccess = false;

                _UpdateScaledScore(row);

            }

            if (allSuccess)
            {
                // change form mode to Update
                Mode = enMode.Update;
                this.Text = "Update Scores";
                MessageBox.Show("Scores saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Some scores could not be saved.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void _SaveScore(int enrollmentID, int subjectID, bool isTest, decimal rawScore, decimal maxScore)
        {
            if (maxScore <= 0) maxScore = 100m; // default to avoid division by zero

            // Scale raw score to 100%
            decimal scaledScore = (rawScore / maxScore) * 100m;

            // Load existing score if any
            int termID = clsTerm.GetCurrentTermID();
            var existing = clsScoresDetails.LoadByEnrollmentTermAndSubject(enrollmentID, termID, subjectID);

            clsScoresDetails scoreDetail = existing ?? new clsScoresDetails
            {
                EnrollmentID = enrollmentID,
                SubjectID = subjectID,
                TermID = termID,
                CreatedBy = clsGlobal.CurrentUser.UserName
            };

            if (isTest)
                scoreDetail.TestScore = scaledScore;
            else
                scoreDetail.ExamScore = scaledScore;

            scoreDetail.ModifiedBy = clsGlobal.CurrentUser.UserName;

            scoreDetail.Save();
        }


        private void dgvScores_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvScores.CurrentCell == null) return;
            if (dgvScores.CurrentCell.OwningColumn == null) return;

            string colName = dgvScores.CurrentCell.OwningColumn.Name;
            if (colName != "TestScore" && colName != "ExamScore") return;

            if (e.Control is TextBox tb)
            {
                tb.TextChanged -= ScoreCell_TextChanged;
                tb.TextChanged += ScoreCell_TextChanged;
            }
        }
        private void ScoreCell_TextChanged(object sender, EventArgs e)
        {

            if (dgvScores.CurrentCell?.RowIndex < 0) return;

            var row = dgvScores.Rows[dgvScores.CurrentCell.RowIndex];
            _UpdateScaledScore(row);
        }
        private void _UpdateScaledScore(DataGridViewRow row)
        {
            if (row == null) return;

            decimal testScore = 0m, examScore = 0m;
            decimal.TryParse(row.Cells["TestScore"].Value?.ToString(), out testScore);
            decimal.TryParse(row.Cells["ExamScore"].Value?.ToString(), out examScore);

            // Weighted sum: 30 + 70 = 100
            decimal scaled = (testScore / 30m) * 30m + (examScore / 70m) * 70m;
            row.Cells["ScaledScore"].Value = scaled.ToString("0.##");
        }


        private void LoadScores()
        {
            // --- Get current term ---
            clsTerm currentTerm = clsTerm.GetAll()
                ?.AsEnumerable()
                .Where(r => Convert.ToDateTime(r["StartDate"]) <= DateTime.Today
                         && Convert.ToDateTime(r["EndDate"]) >= DateTime.Today)
                .Select(r => new clsTerm
                {
                    TermID = Convert.ToInt32(r["TermID"]),
                    TermName = r["TermName"].ToString()
                })
                .FirstOrDefault();

            if (currentTerm == null)
            {
                MessageBox.Show("No current term found for today!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentTerm.TermName = $"Current Term: {currentTerm.TermName}";

            // --- Load subjects and scores ---
            DataTable subjects = clsSubject.GetAllSubjects();
            dgvScores.Rows.Clear();

            int? enrollmentID = clsEnrollment.GetActiveEnrollmentIDByStudentID(_StudentID);
            foreach (DataRow subj in subjects.Rows)
            {
                int rowIndex = dgvScores.Rows.Add();
                dgvScores.Rows[rowIndex].Cells["SubjectName"].Value = subj["SubjectName"];
                dgvScores.Rows[rowIndex].Tag = subj["SubjectID"];

                if (enrollmentID != null)
                {
                    DataTable dtScores = clsEnrollment.GetScoresByEnrollmentID(enrollmentID.Value);
                    var scoreRow = dtScores.AsEnumerable()
                        .FirstOrDefault(r => Convert.ToInt32(r["SubjectID"]) == Convert.ToInt32(subj["SubjectID"]));

                    if (scoreRow != null)
                    {
                        dgvScores.Rows[rowIndex].Cells["Score"].Value = scoreRow["RawScore"];
                        dgvScores.Rows[rowIndex].Cells["ScaledScore"].Value = scoreRow["ScaledScore"];
                        dgvScores.Rows[rowIndex].Cells["ExamType"].Value = scoreRow["ExamTypeID"];
                    }
                    else
                    {
                        var comboCol = dgvScores.Columns["ExamType"] as DataGridViewComboBoxColumn;
                        if (comboCol != null && comboCol.DataSource != null)
                        {
                            var dt = comboCol.DataSource as DataTable;
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                dgvScores.Rows[rowIndex].Cells["ExamType"].Value = dt.Rows[0]["ExamTypeID"];
                            }
                        }

                    }
                }
            }
        }


        private void dgvScores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // تحقق أن التغيير في عمود Score وليس غيره
            if (e.ColumnIndex == dgvScores.Columns["Score"].Index && e.RowIndex >= 0)
            {
                var scoreCell = dgvScores.Rows[e.RowIndex].Cells["Score"];
                var scaledScoreCell = dgvScores.Rows[e.RowIndex].Cells["ScaledScore"];

                if (float.TryParse(scoreCell.Value?.ToString(), out float rawScore))
                {
                    // مثلاً: نحسب Scaled Score بنسبة 2 (أي 33 * 2 = 66)
                    float scaledScore = rawScore * 2;

                    // أو أي معادلة تحبها:
                    // float scaledScore = (rawScore / maxRawScore) * maxScaledScore;

                    scaledScoreCell.Value = scaledScore.ToString("0.##"); // صيغة عرض رقم عشري
                }
                else
                {
                    scaledScoreCell.Value = null;
                }
            }
        }

        private void dgvScores_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            _UpdateScaledScore(dgvScores.Rows[e.RowIndex]);
        }

        private void cbScaleMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvScores.Rows)
            {
                if (row.IsNewRow) continue;

                float combinedRaw = 0f;
                int examCount = 0;

                // Iterate all columns that start with "Score_" (your exam types)
                foreach (DataGridViewColumn col in dgvScores.Columns)
                {
                    if (!col.Name.StartsWith("Score_")) continue;

                    float value = 0f;
                    if (row.Cells[col.Name].Value != null)
                        float.TryParse(row.Cells[col.Name].Value.ToString(), out value);

                    combinedRaw += value;
                    examCount++;
                }

                if (examCount == 0) continue;

                float totalScaled = 0f;

                foreach (DataGridViewColumn col in dgvScores.Columns)
                {
                    if (!col.Name.StartsWith("Score_")) continue;

                    float raw = 0f;
                    if (!float.TryParse(row.Cells[col.Name].Value?.ToString(), out raw))
                        continue;

                    int examTypeID = int.Parse(col.Name.Replace("Score_", ""));

                    float maxRaw = clsExamType.GetMaxScoreSafe(examTypeID);   // e.g., 50
                    float weight = (float)clsExamType.GetWeight(examTypeID);  // convert double to float


                    if (maxRaw <= 0) continue; // avoid division by zero

                    float scale = 100f;
                    switch (cbScaleMethod.SelectedItem?.ToString())
                    {
                        case "Out of 100": scale = 100f; break;
                        case "Out of 20": scale = 20f; break;
                        case "Out of 10": scale = 10f; break;
                    }

                    float scaled = (raw / maxRaw) * scale * weight;
                    totalScaled += scaled;
                }

                row.Cells["ScaledScore"].Value = totalScaled.ToString("0.##");


            }
        }
       

        private void dgvScores_EditingControlShowing_1(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox tb)
            {
                tb.TextChanged -= ScoreCell_TextChanged;
                tb.TextChanged += ScoreCell_TextChanged;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
