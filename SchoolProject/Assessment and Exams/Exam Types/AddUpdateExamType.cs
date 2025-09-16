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
    public partial class AddUpdateExamType : Form
    {
        private int? _ExamTypeID = null; // null = AddNew, otherwise Update
        
        public AddUpdateExamType()
        {
            InitializeComponent();
        }
        public AddUpdateExamType(int examTypeID)
        {
            InitializeComponent(); _ExamTypeID = examTypeID;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a title for the exam type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtWeight.Text, out double weight) || weight < 0 || weight > 1)
            {
                MessageBox.Show("Weight must be a number between 0 and 1.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            clsTerm currentTerm = clsTerm.GetCurrentTerm();
            if (currentTerm == null)
            {
                MessageBox.Show("Cannot determine current term!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            clsExamType examType = new clsExamType
            {
                Title = txtTitle.Text.Trim(),
                Weight = weight,
                TermID = currentTerm.TermID,
                IsActive = chkIsActive.Checked,
                Mode = _ExamTypeID.HasValue ? clsExamType.enMode.Update : clsExamType.enMode.AddNew
            };

            if (_ExamTypeID.HasValue)
                examType.ExamTypeID = _ExamTypeID.Value;

            try
            {
                if (examType.Save())
                    MessageBox.Show("Exam type saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show("Error saving exam type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmExamSetup_Load(object sender, EventArgs e)
        {
            if (_ExamTypeID.HasValue)
                LoadExamType(_ExamTypeID.Value);
        }
        private void LoadExamType(int examTypeID)
        {
            clsExamType exam = clsExamType.FindByID(examTypeID);
            if (exam == null)
            {
                MessageBox.Show("Exam type not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtTitle.Text = exam.Title;
            txtWeight.Text = exam.Weight.ToString("0.##");
            chkIsActive.Checked = exam.IsActive;
        }
        private int GetSelectedTermID()
        {
            // TODO: Replace with actual logic to get TermID
            // Could be a combobox on the form or default term
            return 1;
        }
    }
}
