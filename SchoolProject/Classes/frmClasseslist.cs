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

namespace SchoolProject.Classes
{
    public partial class frmClasseslist : Form
    {

        clsEmployee employee;
        private static DataTable _dtAllClasses = clsClass.GetAllClasses();
        private DataTable _dtClasses = _dtAllClasses.DefaultView.ToTable(false, "ClassID", "ClassName");



        private void _RefreshClassesList()
        {
            if (cbGrades.SelectedValue == null)
                return;

            int selectedGradeID;

            if (cbGrades.SelectedValue is DataRowView drv)
                selectedGradeID = Convert.ToInt32(drv["GradeID"]);
            else
                selectedGradeID = Convert.ToInt32(cbGrades.SelectedValue);

            _LoadClassesByGrade(selectedGradeID);
        }

        public frmClasseslist()
        {
            InitializeComponent();
        }

        private void frmClasseslist_Load(object sender, EventArgs e)
        {
            _FillGradesComboBox();
            cbFilterBy.SelectedIndex = 0;

            // أول تحميل يكون على أول Grade
            if (cbGrades.Items.Count > 0)
            {
                int gradeID = Convert.ToInt32(cbGrades.SelectedValue);
                _LoadClassesByGrade(gradeID);
            }
        }
        private void _LoadClassesByGrade(int gradeID)
        {
            _dtAllClasses = clsClass.GetClassesByGradeID(gradeID); // استرجاع الصفوف حسب Grade
            _dtClasses = _dtAllClasses.DefaultView.ToTable(false, "ClassID", "ClassName");
            dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvClasses.DataSource = _dtClasses;

            lblRecordCount.Text = _dtClasses.Rows.Count.ToString();

            if (dgvClasses.Rows.Count > 0)
            {
                dgvClasses.Columns[0].HeaderText = "Class ID";
                dgvClasses.Columns[0].Width = 100;

                dgvClasses.Columns[1].HeaderText = "Class Name";
                dgvClasses.Columns[1].Width = 200;
            }
        }

        private void _FillGradesComboBox()
        {
            DataTable dtGrades = clsGrade.GetAllGrades(); // تأكد أنك عامل Data Layer و Business Layer للـ Grades

            cbGrades.DataSource = dtGrades;
            cbGrades.DisplayMember = "GradeName";
            cbGrades.ValueMember = "GradeID";
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
                case "Class ID":
                    FilterColumn = "ClassID";
                    break;
                case "Class Name":
                    FilterColumn = "ClassName";
                    break;
                default:
                    FilterColumn = "None";
                    break;
            }

            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtClasses.DefaultView.RowFilter = "";
                lblRecordCount.Text = dgvClasses.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "ClassID")
                //in this case we deal with integer not string.

                _dtClasses.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
            else
                _dtClasses.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());

            lblRecordCount.Text = dgvClasses.Rows.Count.ToString();

        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            //we allow number incase person id is selected.
            if (cbFilterBy.Text == "Class ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnAddNewClass_Click(object sender, EventArgs e)
        {
            if (cbGrades.SelectedValue == null)
                return;

            int selectedGradeID;

            if (cbGrades.SelectedValue is DataRowView drv)
                selectedGradeID = Convert.ToInt32(drv["GradeID"]);
            else
                selectedGradeID = Convert.ToInt32(cbGrades.SelectedValue);

            frmAddUdateClass frm = new frmAddUdateClass(selectedGradeID);
            frm.ShowDialog();
            _LoadClassesByGrade(selectedGradeID); // لتحديث البيانات فورًا
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        
        {
            if (dgvClasses.CurrentRow == null)
            {
                MessageBox.Show("Please select a class to edit.");
                return;
            }

            int classID = Convert.ToInt32(dgvClasses.CurrentRow.Cells["ClassID"].Value);
            int gradeID = Convert.ToInt32(cbGrades.SelectedValue);  // تأكد أن الـ ComboBox يحتوي على القيم الصحيحة

            frmAddUdateClass editForm = new frmAddUdateClass(classID, gradeID);
            editForm.ShowDialog();

            _RefreshClassesList();
        }


            

        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGrades.SelectedValue == null)
                return;

            int selectedGradeID;

            // حاول قراءة القيمة بطريقة آمنة
            if (cbGrades.SelectedValue is DataRowView drv)
            {
                selectedGradeID = Convert.ToInt32(drv["GradeID"]);
            }
            else
            {
                selectedGradeID = Convert.ToInt32(cbGrades.SelectedValue);
            }

            _LoadClassesByGrade(selectedGradeID);
        }
    }
}
