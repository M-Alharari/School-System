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

namespace SchoolProject.Enrollment_Management
{
    public partial class frmEnrollmentList : Form
    {
        private static DataTable _dtAllEnrollments = clsEnrollment.GetAllEnrollments();



        //only select the columns that you want to show in the grid
        private DataTable _dtEnrollments = _dtAllEnrollments.DefaultView.ToTable(false, "EnrollmentID", "FullName",
                                                         "GradeName", "ClassName", "AcademicYear", "EnrollmentDate",
                                                         "RegisteredBy");

        private void _RefreshPeoplList()
        {
            _dtAllEnrollments = clsEnrollment.GetAllEnrollments();
            _dtEnrollments = _dtAllEnrollments.DefaultView.ToTable(false, "EnrollmentID", "FullName",
                                                         "GradeName", "ClassName", "AcademicYear", "EnrollmentDate",
                                                         "RegisteredBy");
            dgvEnrollment.DataSource = _dtEnrollments;
            lblRecord.Text = dgvEnrollment.Rows.Count.ToString();
        }
        public frmEnrollmentList()
        {
            InitializeComponent();
        }

        private void frmEnrollmentList_Load(object sender, EventArgs e)
        {
            _RefreshPeoplList();

            dgvEnrollment.DataSource = _dtEnrollments;
            cbPeopleFilter.SelectedIndex = 0;
            lblRecord.Text = _dtEnrollments.Rows.Count.ToString();
            if (dgvEnrollment.Rows.Count > 0)
            {
                dgvEnrollment.Columns[0].HeaderText = "Enrollment ID";
                dgvEnrollment.Columns[0].Width = 110;


                dgvEnrollment.Columns[1].HeaderText = "Full Name";
                dgvEnrollment.Columns[1].Width = 160;


                dgvEnrollment.Columns[2].HeaderText = "Grade Name";
                dgvEnrollment.Columns[2].Width = 100;

                dgvEnrollment.Columns[3].HeaderText = "Class Name";
                dgvEnrollment.Columns[3].Width = 100;


                dgvEnrollment.Columns[4].HeaderText = "Academic Year";
                dgvEnrollment.Columns[4].Width = 150;

                dgvEnrollment.Columns[5].HeaderText = "Enrollment Date";
                dgvEnrollment.Columns[5].Width = 150;

                dgvEnrollment.Columns[6].HeaderText = "Registered By";
                dgvEnrollment.Columns[6].Width = 90;
                //dgvEnrollment.Columns[6].DefaultCellStyle.ForeColor = Color.Red;
                




            }
        }
    }
}
