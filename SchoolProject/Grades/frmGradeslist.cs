using SchoolProject.Grades;
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

namespace SchoolProject
{
    public partial class frmGradeslist : Form
    {

        private static DataTable _dtAllGrades;
        private DataTable _dtGrades;

        private void _RefreshClassesList()
        {
            _dtAllGrades = clsGrade.GetAllGrades() ?? new DataTable(); // ensure not null

            if (_dtAllGrades.Rows.Count == 0)
            {
                MessageBox.Show("No grades found in the database.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _dtGrades = _dtAllGrades.Clone(); // keep structure but empty
            }
            else
            {
                _dtGrades = _dtAllGrades.DefaultView.ToTable(false, "GradeID", "GradeName");
            }

            dgvGrades.DataSource = _dtGrades;
            lblRecordCount.Text = _dtGrades.Rows.Count.ToString();

            // Optional: auto-size columns to content
            foreach (DataGridViewColumn col in dgvGrades.Columns)
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
      
        public frmGradeslist()
        {
            InitializeComponent();
        }

        private void frmGradeslist_Load(object sender, EventArgs e)
        {
            _RefreshClassesList();
            cbFilterBy.SelectedIndex = 0;
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";
            switch (cbFilterBy.Text)
            {
                case "Grade ID":
                    FilterColumn = "GradeID";
                    break;
                case "Grade Name":
                    FilterColumn = "GradeName";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtGrades.DefaultView.RowFilter = "";
                lblRecordCount.Text = dgvGrades.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "GradeID")
                //in this case we deal with integer not string.

                _dtGrades.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
            else
                _dtGrades.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());

            lblRecordCount.Text = dgvGrades.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id is selected.
            if (cbFilterBy.Text == "Class ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnAddNewClass_Click(object sender, EventArgs e)
        {
           frmAddUpdateGrade frm = new frmAddUpdateGrade();
            frm.ShowDialog();
            _RefreshClassesList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateGrade frm = new frmAddUpdateGrade(
               (int)dgvGrades.CurrentRow.Cells[0].Value
            );
            frm.ShowDialog();
            _RefreshClassesList();

        }
    }
}
