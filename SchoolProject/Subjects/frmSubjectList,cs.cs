 
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

namespace SchoolProject.Subjects
{
    public partial class frmSubjectList_cs : Form
    {
        clsSubject _Subject;
        private static DataTable _dtAllSubjects;
        private DataTable _dtSubjects;


        private void _Refreshment()
        {
            _dtAllSubjects = clsSubject.GetAllSubjects() ?? new DataTable();

            if (_dtAllSubjects.Columns.Count == 0)
            {
                _dtAllSubjects.Columns.Add("SubjectID", typeof(int));
                _dtAllSubjects.Columns.Add("SubjectName", typeof(string));
            }

            _dtSubjects = _dtAllSubjects.DefaultView.ToTable(false, "SubjectID", "SubjectName");

            dgvSubjects.DataSource = _dtSubjects;
            lblRecordCount.Text = dgvSubjects.Rows.Count.ToString();
        }




        public frmSubjectList_cs()
        {
            InitializeComponent();
        }

        private void frmSubjectList_cs_Load(object sender, EventArgs e)
        {
            _dtAllSubjects = clsSubject.GetAllSubjects() ?? new DataTable();
            if (_dtAllSubjects.Columns.Count == 0)
            {
                // If DB returned empty, create default columns
                _dtAllSubjects.Columns.Add("SubjectID", typeof(int));
                _dtAllSubjects.Columns.Add("SubjectName", typeof(string));
            }

            _dtSubjects = _dtAllSubjects.DefaultView.ToTable(false, "SubjectID", "SubjectName");

            dgvSubjects.DataSource = _dtSubjects;
            lblRecordCount.Text = dgvSubjects.Rows.Count.ToString();

        }

        private void btnAddNewSubject_Click(object sender, EventArgs e)
        {
            frmAddUpdateSubject frm = new frmAddUpdateSubject();
            frm.ShowDialog();
            _Refreshment();
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateSubject frm = new frmAddUpdateSubject((int)dgvSubjects.CurrentRow.Cells[0].Value);
            frm.ShowDialog();
            _Refreshment();
        }
    }
}
