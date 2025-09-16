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
    public partial class frmAddUdateClass : Form
    {
        private enum enMode { AddNew = 0, Update = 1 }
        enMode Mode = enMode.AddNew;
        private int _ClassID;
        clsClass _Class;

        private int _gradeID = -1;
         
        public frmAddUdateClass(int gradeID)
        {
            InitializeComponent();
            _gradeID = gradeID;
        }

        public frmAddUdateClass()
        {
            InitializeComponent();
            Mode = enMode.AddNew;
        }

        public frmAddUdateClass(int ClassID, int gradeID)
        {
            InitializeComponent();

            _ClassID = ClassID;
            _gradeID = gradeID;
            Mode = enMode.Update;
        }
        private void _ResetDefaultValues()
        {

            if (Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Class";
                this.lblTitle.Text = "Add New Class";
                _Class = new clsClass();

            }
            else
            {
                lblTitle.Text = "Update Class";
                this.lblTitle.Text = "Update Class";

            }


            lblClassID.Text = "[????]";
            txtClassName.Text = "";
           
        }



        private bool _LoadData()
        {
            _Class = clsClass.FindClassByID(_ClassID);

            if (_Class == null)
            {
                MessageBox.Show($"No Class found with ID: {_ClassID}", "Class Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return false;
            }

            txtClassName.Text = _Class.ClassName;
            lblClassID.Text = _Class.ClassID.ToString();

            return true;
        }


        private void frmAddUdateClass_Load(object sender, EventArgs e)
        {
            if (Mode == enMode.Update)
            {
                if (!_LoadData())
                {
                    return; // الكائن فشل تحميله وتم إغلاق الفورم بالفعل
                }
                lblTitle.Text = "Update Class";
            }
            else
            {
                _Class = new clsClass(); // إنشاء كائن جديد
                _Class.GradeID = _gradeID;
                lblTitle.Text = "Add New Class";
                lblClassID.Text = "[New]";
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtClassName.Text.Trim() == "")
                if (txtClassName.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter a class name.");
                    return;
                }

            if (_Class == null)
            {
                MessageBox.Show("Internal error: Class object is not initialized.");
                return;
            }

            _Class.ClassName = txtClassName.Text.Trim();
            _Class.GradeID = _gradeID;

            if (_Class.Save())
            {
                MessageBox.Show(Mode == enMode.AddNew ? "Class added successfully." : "Class updated successfully.");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save class.");
            }
        }
    }
}
