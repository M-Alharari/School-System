using SchoolProject.Employees;
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

namespace SchoolProject.Behaviours
{
    public partial class frmStudentBehaviorslist : Form
    {
        private static DataTable _dtBehaviours;
        private clsStudentBehaviour BehaviourLogic;
        private int _studentID; // Or EnrolledID if you prefer

        public frmStudentBehaviorslist(int studentID)
        {
            InitializeComponent(); _studentID = studentID;
            BehaviourLogic = new clsStudentBehaviour();
        }


        private void _RefreshBehavioursList()
        {
            // Get behaviours for this student safely
            _dtBehaviours = clsStudentBehaviour.GetByEnrolledID(_studentID) ?? new DataTable();

            dgvBehaviours.DataSource = _dtBehaviours;

            if (_dtBehaviours.Rows.Count > 0)
            {
                dgvBehaviours.Columns[0].HeaderText = "Behaviour ID";
                dgvBehaviours.Columns[0].Width = 80;

                dgvBehaviours.Columns[1].HeaderText = "Date Recorded";
                dgvBehaviours.Columns[1].Width = 120;

                dgvBehaviours.Columns[2].HeaderText = "Type";
                dgvBehaviours.Columns[2].Width = 100;

                dgvBehaviours.Columns[3].HeaderText = "Category";
                dgvBehaviours.Columns[3].Width = 120;

                dgvBehaviours.Columns[4].HeaderText = "Severity";
                dgvBehaviours.Columns[4].Width = 80;

                dgvBehaviours.Columns[5].HeaderText = "Action Taken";
                dgvBehaviours.Columns[5].Width = 120;

                dgvBehaviours.Columns[6].HeaderText = "Description";
                dgvBehaviours.Columns[6].Width = 250;

                dgvBehaviours.Columns[7].HeaderText = "Recorded By";
                dgvBehaviours.Columns[7].Width = 120;
            }

            lblRecordsCount.Text = dgvBehaviours.Rows.Count.ToString();
        }

        private void frmStudentBehaviorslist_Load(object sender, EventArgs e)
        {
            _RefreshBehavioursList();
        }

        private void btnAddBehaviour_Click(object sender, EventArgs e)
        {
            frmAddUpdateBehaviour frm = new frmAddUpdateBehaviour(_studentID);
            frm.ShowDialog();
            _RefreshBehavioursList();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvBehaviours.CurrentRow == null) return;

            int behaviourID = (int)dgvBehaviours.CurrentRow.Cells[0].Value;
            frmAddUpdateBehaviour frm = new frmAddUpdateBehaviour(_studentID, behaviourID);
            frm.ShowDialog();
            _RefreshBehavioursList();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            ShowBehavioursDetails frmStudentBehaviors = new ShowBehavioursDetails((int)dgvBehaviours.CurrentRow.Cells["behaviourID"].Value);
            frmStudentBehaviors.ShowDialog();
        }
    }
}
