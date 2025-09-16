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

namespace SchoolProject.Assessment_and_Exams.Exam_Types
{
    public partial class frmExamTypeslist : Form
    {
        private static DataTable _dtAllExamTypes;
        private DataTable _dtExamTypes;

        private void LoadExamTypes()
        {
            // Get all exam types
            _dtExamTypes = clsExamType.GetAllExamTypes() ?? new DataTable();

            if (_dtExamTypes.Columns.Count == 0)
            {
                MessageBox.Show("No Exam Types found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dgvExamTypes.DataSource = null;
                return;
            }

            // Determine the correct title column
            string titleColumn = _dtExamTypes.Columns.Contains("Title") ? "Title" : "ExamTypeName";

            // Bind to DataGridView
            dgvExamTypes.DataSource = _dtExamTypes;

            // Hide ID column
            if (_dtExamTypes.Columns.Contains("ExamTypeID"))
                dgvExamTypes.Columns["ExamTypeID"].Visible = false;

            // Set headers
            if (_dtExamTypes.Columns.Contains(titleColumn))
            {
                dgvExamTypes.Columns[titleColumn].HeaderText = "Exam Title";
                dgvExamTypes.Columns[titleColumn].Width = 200;
            }

            if (_dtExamTypes.Columns.Contains("Weight"))
            {
                dgvExamTypes.Columns["Weight"].HeaderText = "Weight";
                dgvExamTypes.Columns["Weight"].Width = 80;
            }

            if (_dtExamTypes.Columns.Contains("TermID"))
            {
                dgvExamTypes.Columns["TermID"].HeaderText = "Term";
                dgvExamTypes.Columns["TermID"].Width = 100;
            }

            if (_dtExamTypes.Columns.Contains("IsActive"))
            {
                dgvExamTypes.Columns["IsActive"].HeaderText = "Active";
                dgvExamTypes.Columns["IsActive"].Width = 70;
            }

            lblRecordCount.Text = dgvExamTypes.Rows.Count.ToString();
        }
        private void SetupGridHeaders(string colID, string colTitle, string colWeight, string colTermID, string colActive)
        {
            if (dgvExamTypes.Rows.Count == 0) return;

            dgvExamTypes.Columns[colID].Visible = false;

            dgvExamTypes.Columns[colTitle].HeaderText = "Exam Title";
            dgvExamTypes.Columns[colTitle].Width = 200;

            dgvExamTypes.Columns[colWeight].HeaderText = "Weight";
            dgvExamTypes.Columns[colWeight].Width = 100;

            dgvExamTypes.Columns[colTermID].HeaderText = "Term";
            dgvExamTypes.Columns[colTermID].Width = 150;

            dgvExamTypes.Columns[colActive].HeaderText = "Active";
            dgvExamTypes.Columns[colActive].Width = 70;
        }

        private void _RefreshData()
        {
            LoadExamTypes();
        }
        public frmExamTypeslist()
        {
            InitializeComponent();
            _dtAllExamTypes = clsExamType.GetAllExamTypes() ?? new DataTable();

            if (_dtAllExamTypes.Columns.Contains("ExamTypeID") && _dtAllExamTypes.Columns.Contains("Title"))
                _dtExamTypes = _dtAllExamTypes.DefaultView.ToTable(false, "ExamTypeID", "Title", "Weight", "TermID", "IsActive");
            else
                _dtExamTypes = new DataTable();
        }

       

        private void frmExamTypeslist_Load(object sender, EventArgs e)
        {
            LoadExamTypes();
        }

        private void btnAddNewExamType_Click(object sender, EventArgs e)
        {
            AddUpdateExamType frm = new AddUpdateExamType(); // Add new mode
            frm.ShowDialog();
            _RefreshData();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvExamTypes.CurrentRow != null)
            {
                int examTypeID = Convert.ToInt32(dgvExamTypes.CurrentRow.Cells["ExamTypeID"].Value);
                AddUpdateExamType frm = new AddUpdateExamType(examTypeID); // Edit mode
                frm.ShowDialog();
                _RefreshData();
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvExamTypes.CurrentRow == null) return;

            int examTypeID = Convert.ToInt32(dgvExamTypes.CurrentRow.Cells["ExamTypeID"].Value);
            if (MessageBox.Show($"Are you sure you want to delete Exam Type [{dgvExamTypes.CurrentRow.Cells["Title"].Value}]?",
                "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                var exam = clsExamType.FindByID(examTypeID);
                if (exam != null && exam.Delete())
                {
                    MessageBox.Show("Exam Type deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshData();
                }
                else
                {
                    MessageBox.Show("Cannot delete this Exam Type. It may have linked data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
