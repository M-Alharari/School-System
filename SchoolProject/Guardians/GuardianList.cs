 
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

namespace SchoolProject.Guardians
{
    public partial class GuardianList : Form
    {
        //private static DataTable _dtAllGuardians = clsGuardian.GetAllGuardians() ?? new DataTable();
        private DataTable _dtGuardians;
        // Delegate & Event for sending GuardianID back
        public delegate void GuardianSelectedEventHandler(object sender, int guardianID);
        public event GuardianSelectedEventHandler GuardianSelected;

        private void _RefreshGuardianList()
        {
            DataTable dtDisplay = clsGuardianStudents.GetAllGuardiansSummary();
            dgvGuardians.DataSource = dtDisplay;
            dgvGuardians.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            lblRecordCount.Text = dgvGuardians.Rows.Count.ToString();

            // Optional: hide ID column
            dgvGuardians.Columns["GuardianID"] .HeaderText = "Guardian ID";

            dgvGuardians.Columns["GuardianName"].HeaderText = "Guardian Name";
            dgvGuardians.Columns["StudentCount"].HeaderText = "Number of Students";
        }


        public GuardianList()
        {
            InitializeComponent();
        }

        private void GuardianList_Load(object sender, EventArgs e)
        {
            _RefreshGuardianList();
            cbFilterBy.SelectedIndex = 0;

            if (dgvGuardians.Columns.Contains("GuardianID"))
            {
                dgvGuardians.Columns["GuardianID"].HeaderText = "Guardian ID";
                dgvGuardians.Columns["GuardianID"].Width = 100;
            }

            if (dgvGuardians.Columns.Contains("GuardianName"))
            {
                dgvGuardians.Columns["GuardianName"].HeaderText = "Guardian Name";
                dgvGuardians.Columns["GuardianName"].Width = 200;
            }

            if (dgvGuardians.Columns.Contains("StudentCount"))
            {
                dgvGuardians.Columns["StudentCount"].HeaderText = "Number of Students";
                dgvGuardians.Columns["StudentCount"].Width = 120;
            }

        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "None";

            if (cbFilterBy.Text == "Guardian Name")
                FilterColumn = "GuardianName";
            
            else
                FilterColumn = "None";


            if (string.IsNullOrWhiteSpace(txtFilterValue.Text) || FilterColumn == "None")
            {
                _dtGuardians.DefaultView.RowFilter = "";
                lblRecordCount.Text = dgvGuardians.Rows.Count.ToString();
                return;
            }

            _dtGuardians.DefaultView.RowFilter = $"[{FilterColumn}] LIKE '{txtFilterValue.Text.Trim()}%'";
            lblRecordCount.Text = dgvGuardians.Rows.Count.ToString();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = cbFilterBy.Text != "None";
            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }


        private void selectGuardianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvGuardians.CurrentRow != null)
            {
                int guardianID = Convert.ToInt32(dgvGuardians.CurrentRow.Cells["GuardianID"].Value);
                GuardianSelected?.Invoke(this, guardianID);
                this.Close();
            }
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuardianStudentsDetails guardianStudentsDetails = new GuardianStudentsDetails((int)dgvGuardians.CurrentRow.Cells[0].Value);
            guardianStudentsDetails.ShowDialog();
        }
    }
}
