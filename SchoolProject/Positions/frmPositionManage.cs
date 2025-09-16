 
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

namespace SchoolProject.Positions
{
    public partial class frmPositionManage : Form
    {

        clsPosition _Position;
        private static DataTable _dtAllPositions = clsPosition.GetAllPositions();
        private DataTable _dtPositions = _dtAllPositions.DefaultView.ToTable(false, "PositionID", "PositionName");


        private void _Refreshment()
        {
            _dtAllPositions = clsPosition.GetAllPositions();
            _dtPositions = _dtAllPositions.DefaultView.ToTable(false, "PositionID", "PositionName");

            dgvPosition.DataSource = _dtAllPositions;
            lblRecordCount.Text = dgvPosition.RowCount.ToString();
        }



        public frmPositionManage()
        {
            InitializeComponent();
            _dtAllPositions = clsPosition.GetAllPositions() ?? new DataTable();

            if (_dtAllPositions.Columns.Contains("PositionID") && _dtAllPositions.Columns.Contains("PositionName"))
                _dtPositions = _dtAllPositions.DefaultView.ToTable(false, "PositionID", "PositionName");
            else
                _dtPositions = new DataTable(); // fallback
        }

        private void frmPositionManage_Load(object sender, EventArgs e)
        {
            dgvPosition.DataSource = _dtAllPositions;
            lblRecordCount.Text = dgvPosition.RowCount.ToString();
            lblRecordCount.Text = dgvPosition.Rows.Count.ToString();
            if (dgvPosition.Rows.Count > 0)
            {
                dgvPosition.Columns[0].HeaderText = "Position ID";
                dgvPosition.Columns[0].Width = 100;

                dgvPosition.Columns[1].HeaderText = "Position Name";
                dgvPosition.Columns[1].Width = 300;
            }
        }

        private void btnAddNewSubject_Click(object sender, EventArgs e)
        {
            frmAddUpdatePosition frm = new frmAddUpdatePosition();
            frm.ShowDialog();
            _Refreshment();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete Position [" + dgvPosition.CurrentRow.Cells[0].Value + "]", "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)

            {

                //Perform Delele and refresh
                if (clsPosition.DeletePosition((int)dgvPosition.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("Position Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _Refreshment();
                }

                else
                    MessageBox.Show("Position was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }
    }
}
