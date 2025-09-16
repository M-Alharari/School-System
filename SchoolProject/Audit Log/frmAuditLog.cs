 
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

namespace SchoolProject.Audit_Log
{
    public partial class frmAuditLog : Form
    {
        private DataTable _dtAllAuditLogs;
        private DataTable _dtAuditLogs;

        public frmAuditLog()
        {
            InitializeComponent();
        }

        private void frmAuditLog_Load(object sender, EventArgs e)
        {
            _RefreshAuditLogs();

            // Optional: setup filter combobox
            cbFilterBy.Items.Clear();
            cbFilterBy.Items.Add("All");
            cbFilterBy.Items.Add("INSERT");
            cbFilterBy.Items.Add("UPDATE");
            cbFilterBy.Items.Add("DELETE");
            cbFilterBy.SelectedIndex = 0;

            dgvAuditLogs.DataSource = _dtAuditLogs;
            lblRecordCount.Text = _dtAuditLogs.Rows.Count.ToString();

            if (_dtAuditLogs.Rows.Count > 0)
                FormatGridColumns();
        }
        private void _RefreshAuditLogs()
        {
            _dtAllAuditLogs = clsAuditLog.GetAll() ?? new DataTable();

            if (_dtAllAuditLogs.Columns.Contains("AuditID"))
            {
                _dtAuditLogs = _dtAllAuditLogs.DefaultView.ToTable(false,
                    "AuditID", "TableName", "RecordID", "Action", "OldValues", "NewValues", "PerformedBy", "PerformedAt");
            }
            else
            {
                _dtAuditLogs = new DataTable();
            }

            dgvAuditLogs.DataSource = _dtAuditLogs;
            lblRecordCount.Text = _dtAuditLogs.Rows.Count.ToString();
        }

        private void FormatGridColumns()
        {
            dgvAuditLogs.Columns[0].HeaderText = "Audit ID";
            dgvAuditLogs.Columns[0].Width = 80;

            dgvAuditLogs.Columns[1].HeaderText = "Table";
            dgvAuditLogs.Columns[1].Width = 100;

            dgvAuditLogs.Columns[2].HeaderText = "Record ID";
            dgvAuditLogs.Columns[2].Width = 80;

            dgvAuditLogs.Columns[3].HeaderText = "Action";
            dgvAuditLogs.Columns[3].Width = 80;

            dgvAuditLogs.Columns[4].HeaderText = "Old Values";
            dgvAuditLogs.Columns[4].Width = 200;

            dgvAuditLogs.Columns[5].HeaderText = "New Values";
            dgvAuditLogs.Columns[5].Width = 200;

            dgvAuditLogs.Columns[6].HeaderText = "Performed By";
            dgvAuditLogs.Columns[6].Width = 100;

            dgvAuditLogs.Columns[7].HeaderText = "Performed Date";
            dgvAuditLogs.Columns[7].Width = 120;
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowAuditDetails showAuditDetails = new ShowAuditDetails((int)dgvAuditLogs.CurrentRow.Cells[0].Value);
            showAuditDetails.ShowDialog();
        }
    }
}
