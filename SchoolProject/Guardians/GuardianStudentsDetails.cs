using SchoolProject.Students;
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
    public partial class GuardianStudentsDetails : Form
    {
        private int _guardianID;
        public GuardianStudentsDetails(int guardianID)
        {
            InitializeComponent(); _guardianID = guardianID;
            LoadGuardianStudents();
        }

        private void LoadGuardianStudents()
        {
            DataTable dt = clsGuardianStudents.GetGuardianStudents(_guardianID);


            dgvStudents.DataSource = dt;
            lblRecordCount.Text = dt.Rows.Count.ToString();
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.Columns["GuardianID"].Visible = false;

            dgvStudents.Columns["StudentID"].HeaderText = "Student ID";

            dgvStudents.Columns["StudentName"].HeaderText = "Student";
            dgvStudents.Columns["Relationship"].HeaderText = "Relationship";

            // Add button column (if not already added)
            if (!dgvStudents.Columns.Contains("Details"))
            {
                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
                btnCol.Name = "Details";
                btnCol.HeaderText = "Actions";
                btnCol.Text = "View Details";
                btnCol.UseColumnTextForButtonValue = true;
                dgvStudents.Columns.Add(btnCol);
            }
        }
        private void AddDetailsColumn()
        {
            if (!dgvStudents.Columns.Contains("Details"))
            {
                DataGridViewButtonColumn btnDetails = new DataGridViewButtonColumn();
                btnDetails.Name = "Details";
                btnDetails.HeaderText = "Details";
                btnDetails.Text = "View";
                btnDetails.UseColumnTextForButtonValue = true;

                dgvStudents.Columns.Add(btnDetails);
            }
        }



        private void btnShowDetails_Click(object sender, EventArgs e)
        { 
        }

        private void dgvStudents_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks on header or invalid rows
            if (e.RowIndex < 0) return;

            // Only handle if the "Details" column was clicked
            if (dgvStudents.Columns[e.ColumnIndex].Name == "Details")
            {
                int studentID = Convert.ToInt32(dgvStudents.Rows[e.RowIndex].Cells["StudentID"].Value);

                frmStudentDetail frm = new frmStudentDetail(studentID);
                frm.ShowDialog();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
