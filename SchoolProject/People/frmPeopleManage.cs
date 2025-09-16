using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace SchoolProject.People
{
    public partial class frmPeopleManage : Form
    {
        private static DataTable _dtAllPeople = clsPerson.GetAllPeople() ?? new DataTable();

        private DataTable _dtPeople;

        private void _RefreshPeoplList()
        {
            // 1️⃣ Get raw data
            _dtAllPeople = clsPerson.GetAllPeople() ?? new DataTable();

            if (_dtAllPeople.Rows.Count == 0)
            {
                _dtPeople = new DataTable();
                dgvPeople.DataSource = _dtPeople;
                lblRecord.Text = "0";
                return;
            }

            // 2️⃣ Create display table including all columns
            _dtPeople = _dtAllPeople.DefaultView.ToTable(false,
                "PersonID",
                "NationalNo",
                "FirstName",
                "SecondName",
                "ThirdName",
                "LastName",
                "GendorCaption",
                "CountryName"
            );

            // 3️⃣ Add FullName column
            if (!_dtPeople.Columns.Contains("FullName"))
                _dtPeople.Columns.Add("FullName", typeof(string));

            foreach (DataRow row in _dtPeople.Rows)
            {
                row["FullName"] = $"{row["FirstName"]} {row["SecondName"]} {row["ThirdName"]} {row["LastName"]}"
                                  .Replace("  ", " ").Trim();
            }

            // 4️⃣ Reorder columns: PersonID, NationalNo, FullName, Gender, Nationality
            dgvPeople.DataSource = _dtPeople;
            dgvPeople.Columns["PersonID"].DisplayIndex = 0;
            dgvPeople.Columns["NationalNo"].DisplayIndex = 1;
            dgvPeople.Columns["FullName"].DisplayIndex = 2;
            dgvPeople.Columns["GendorCaption"].DisplayIndex = 3;
            dgvPeople.Columns["CountryName"].DisplayIndex = 4;

            // 5️⃣ Set headers
            dgvPeople.Columns["PersonID"].HeaderText = "Person ID";
            dgvPeople.Columns["NationalNo"].HeaderText = "National No.";
            dgvPeople.Columns["FullName"].HeaderText = "Full Name";
            dgvPeople.Columns["GendorCaption"].HeaderText = "Gender";
            dgvPeople.Columns["CountryName"].HeaderText = "Nationality";

            // 6️⃣ Hide individual name columns
            dgvPeople.Columns["FirstName"].Visible = false;
            dgvPeople.Columns["SecondName"].Visible = false;
            dgvPeople.Columns["ThirdName"].Visible = false;
            dgvPeople.Columns["LastName"].Visible = false;

            // 7️⃣ Update record count
            lblRecord.Text = dgvPeople.Rows.Count.ToString();
        }

        public frmPeopleManage()
        {
            InitializeComponent();
        }

        private void frmPeopleManage_Load(object sender, EventArgs e)
        {
            _RefreshPeoplList();
            cbPeopleFilter.SelectedIndex = 0;
            txtFilter.Visible = (cbPeopleFilter.Text != "None");

        }

        private void cbPeopleFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilter.Visible = (cbPeopleFilter.Text != "None");

            if (txtFilter.Visible)
            {
                txtFilter.Text = "";
                txtFilter.Focus();
            }

        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        { 
            string filterColumn = "";

            // Map combobox selection to real column
            switch (cbPeopleFilter.Text)
            {
                case "Person ID":
                    filterColumn = "PersonID";
                    break;
                case "National No.":
                    filterColumn = "NationalNo";
                    break;
                case "Full Name":
                    filterColumn = "FullName";
                    break;
                case "Gender":
                    filterColumn = "GendorCaption";
                    break;
                case "Nationality":
                    filterColumn = "CountryName";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            // Reset filter if empty or invalid
            if (string.IsNullOrWhiteSpace(txtFilter.Text) || filterColumn == "None")
            {
                _dtPeople.DefaultView.RowFilter = "";
                lblRecord.Text = dgvPeople.Rows.Count.ToString();
                return;
            }

            string filterText = txtFilter.Text.Trim().Replace("'", "''");

            // Use exact match for numeric ID, LIKE for others
            if (filterColumn == "PersonID")
                _dtPeople.DefaultView.RowFilter = $"[{filterColumn}] = {filterText}";
            else
                _dtPeople.DefaultView.RowFilter = $"[{filterColumn}] LIKE '{filterText}%'";

            lblRecord.Text = dgvPeople.Rows.Count.ToString();

        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id is selected.
            if (cbPeopleFilter.Text == "Person ID")
               
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void showDeatilsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmShowPersonDetails frm = new frmShowPersonDetails((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _RefreshPeoplList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ID = dgvPeople.Rows[0].Index;
            frmAddUpdatePerson frm = new frmAddUpdatePerson((int)dgvPeople.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _RefreshPeoplList();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete Person [" + dgvPeople.CurrentRow.Cells[0].Value + "]", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)

            {

                //Perform Delele and refresh
                if (clsPerson.DeletePerson((int)dgvPeople.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeoplList();
                }

                else
                    MessageBox.Show("Person was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnAddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
            _RefreshPeoplList(); 
        }
    }
}
