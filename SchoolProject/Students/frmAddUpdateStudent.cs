using SchoolProject.Admissions;
using SchoolProject.Global_Classes;
using SchoolProject.Guardians;
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
 

namespace SchoolProject
{
    public partial class frmAddUpdateStudent : Form
    {
        // Add this property at the top
        public string FeeTypeContext { get; set; } = "Tuition"; // default if needed

        public enum enMode { AddNew = 0, Update = 1 };
        private enMode _Mode;

        public enum enrollMode { AddNew = 0, Update = 1 };
        private enrollMode enroll_Mode;
        private int _StudentID = -1;
        clsStudent _Student;
        clsEnrollment _Enrollment;
        private int GuardianID;
        private int _GuardianID = -1;
        private int _EnrollmentID = -1;
        clsGuardian _Guardian;
        clsPerson _Person;
        private DataTable _dtTerms;
        public frmAddUpdateStudent()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            enroll_Mode = enrollMode.AddNew;
        } 
        public frmAddUpdateStudent(int StudentID)
        {
            InitializeComponent();
            _StudentID = StudentID;
            _Mode = enMode.Update;
            enroll_Mode = enrollMode.Update;
        }
        private void CtrPersonCardWithFilter1_PersonSelected(object sender, int personID)
        {
            MessageBox.Show("This person is an employee and cannot be added as a student.",
                     "Cannot Add", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ctrPersonCardWithFilter1.FilterFocus();
            btnNext.Enabled = false; // disable the Next button
        }
        // Normal selection logic
     private void CtrPersonCardWithFilter1_OnPersonSelected(int personID)
{
    if (clsEmployee.IsPersonEmployee(personID))
    {
        MessageBox.Show("This person is an employee and cannot be added as a student.", "Cannot Add", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        ctrPersonCardWithFilter1.FilterFocus();
        btnNext.Enabled = false;
        return;
    }

    // Save the selected personID in the control
   
    btnNext.Enabled = true;
}

        private DataTable _dtGrades;
        private DataTable _dtClasses;

        private void _LoadGrades()
        {
            cbGrades.Items.Clear();
            _dtGrades = clsGrade.GetAllGrades();

            if (_dtGrades.Rows.Count == 0)
            {
                cbGrades.Items.Add("No items");
                cbGrades.SelectedIndex = 0;
                cbGrades.Enabled = false;
                cbClasses.Items.Add("No items");
                cbClasses.SelectedIndex = 0;
                cbClasses.Enabled = false;
                return;
            }

            foreach (DataRow row in _dtGrades.Rows)
            {
                cbGrades.Items.Add(row["GradeName"].ToString());
            }

            cbGrades.SelectedIndex = 0; // This will trigger SelectedIndexChanged and load classes for Grade 0
            cbGrades.Enabled = true;
            cbClasses.Enabled = true;
        }

        private void _LoadClassesByGradeID(int gradeID)
        {
            cbClasses.Items.Clear();
            _dtClasses = clsClass.GetClassesByGradeID(gradeID); // You must implement this in your class data layer

            if (_dtClasses.Rows.Count == 0)
            {
                cbClasses.Items.Add("No items");
                cbClasses.SelectedIndex = 0;
                cbClasses.Enabled = false;
                return;
            }

            foreach (DataRow row in _dtClasses.Rows)
            {
                cbClasses.Items.Add(row["ClassName"].ToString());
            }

            cbClasses.SelectedIndex = 0;
            cbClasses.Enabled = true;
        }
        private void _FillGradesInComoboBox()
        {
            DataTable dtGrades = clsGrade.GetAllGrades();
            foreach (DataRow row in dtGrades.Rows)
            {
                cbGrades.Items.Add(row["GradeName"]);
            }
            cbGrades.SelectedIndex = 0;
        }

        private void _FillClassesInComoboBox()
        {
            DataTable dtClasses = clsClass.GetAllClasses();

            foreach (DataRow row in dtClasses.Rows)
            {
                cbClasses.Items.Add(row["ClassName"]);
            }
            cbClasses.SelectedIndex = 0;
        }
        private void _ResetDefaultValues()
        {
            _LoadGrades();
            _LoadTerms();  // <-- add this here to populate the Term ComboBox

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Student";
                this.Text = "Add New Student";
                _Student = new clsStudent();

                tpRegisteredInfo.Enabled = false;

                ctrPersonCardWithFilter1.FilterFocus();

                _Enrollment = new clsEnrollment();
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }
            else
            {
                lblTitle.Text = "Update Student";
                this.Text = "Update Student";

                tpRegisteredInfo.Enabled = true;
                btnSave.Enabled = true;
                checkBox1.Checked = false;
                checkBox2.Checked = false;
            }

            lblEnrollmentDate.Text = "[???]";
            lblRegisteredBy.Text = "[???]";
            lblStudentID.Text = "[???]";

            checkBox1.Checked = false;
            checkBox2.Checked = false;
        }

        private void _LoadData()
        {
            _Student = clsStudent.FindStudentByID(_StudentID);
            ctrPersonCardWithFilter1.FilterEnabled = false;

            if (_Student == null)
            {
                MessageBox.Show("No student with ID:" + _StudentID, "Student Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            lblStudentID.Text = _Student.StudentID.ToString();

            _Enrollment = clsEnrollment.FindByStudentID(_StudentID);
            if (_Enrollment != null)
            {
                _EnrollmentID = _Enrollment.EnrollmentID;

                lblEnrollmentDate.Text = _Enrollment.EnrollmentDate.ToString();
                chkIsActive.Checked = _Enrollment.IsActive;
                lblRegisteredBy.Text = clsUser.FindByUserID(_Enrollment.CreatedByUserID).UserName;

                // Load grade
                var grade = clsGrade.FindGradeByID(_Enrollment.GradeID);
                if (grade != null)
                {
                    _LoadGrades();
                    cbGrades.SelectedItem = grade.GradeName;

                    _LoadClassesByGradeID(grade.GradeID);

                    var cls = clsClass.FindClassByID(_Enrollment.ClassID);
                    if (cls != null)
                        cbClasses.SelectedItem = cls.ClassName;
                }

                // Load term
                var term = clsTerm.Find(_Enrollment.TermID);
                if (term != null)
                {
                    _LoadTerms();
                    cbTerms.SelectedItem = term.TermName;
                }
            }

            // ✅ Check if guardian exists for this student
            var dtGuardians = clsGuardianStudents.GetGuardiansForStudent(_Student.StudentID);
            checkBox1.Checked = (dtGuardians.Rows.Count > 0);

            // ✅ Check if student has at least one payment
            var dtPayments = clsTuitionPayment.GetPaymentsByEnrollmentID(_EnrollmentID);
            checkBox2.Checked = (dtPayments.Rows.Count > 0);

            llPaymentForm.Enabled = (_Student != null && _Student.StudentID != -1);

            ctrPersonCardWithFilter1.LoadPersonInfo(_Student.PersonID);
        }


        private void Form4_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            // Assign the check delegate for employees
            ctrPersonCardWithFilter1.PersonCannotBeSelectedCheck = (personID) => clsEmployee.IsPersonEmployee(personID);

            // Subscribe to the event that fires when selection is invalid
            ctrPersonCardWithFilter1.PersonSelectedIsEmp += CtrPersonCardWithFilter1_PersonSelected;

            // Subscribe to the normal person selected event to enable/disable btnNext
            ctrPersonCardWithFilter1.OnPersonSelected += CtrPersonCardWithFilter1_OnPersonSelected;

            if (_Mode == enMode.Update)
                _LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_Mode == enMode.Update)
            {
                btnSave.Enabled = true;
                tpRegisteredInfo.Enabled = true;
                tabControl1.SelectedTab = tabControl1.TabPages["tpRegisteredInfo"];
                return;
            }




            if (ctrPersonCardWithFilter1.PersonID != -1)
            {

                if (clsStudent.DoStudentExists(ctrPersonCardWithFilter1.PersonID))
                {
                    MessageBox.Show("Selected Person is already a Registered Student, choose another one.", "Select another Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ctrPersonCardWithFilter1.FilterFocus();
                }
                else
                {
                    btnSave.Enabled = true;
                    tpRegisteredInfo.Enabled = true;
                    tabControl1.SelectedTab = tabControl1.TabPages["tpRegisteredInfo"];
                }

            }
            else
            {
                MessageBox.Show("Please Select a Person", "Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrPersonCardWithFilter1.FilterFocus();
            }


        }
        private void _LoadTerms()
        {
            cbTerms.Items.Clear();
            _dtTerms = clsTerm.GetAll(); // كل التيرمز من الداتا بيز

            if (_dtTerms.Rows.Count == 0)
            {
                cbTerms.Items.Add("No terms");
                cbTerms.SelectedIndex = 0;
                cbTerms.Enabled = false;
                return;
            }

            DateTime today = DateTime.Today;
            DataRow[] currentTerms = _dtTerms.Select(
                $"'{today:yyyy-MM-dd}' >= StartDate AND '{today:yyyy-MM-dd}' <= EndDate"
            );

            if (currentTerms.Length == 0)
            {
                cbTerms.Items.Add("No active term today");
                cbTerms.SelectedIndex = 0;
                cbTerms.Enabled = false;
                return;
            }

            foreach (DataRow row in currentTerms)
            {
                cbTerms.Items.Add(row["TermName"].ToString());
            }

            cbTerms.SelectedIndex = 0;
            cbTerms.Enabled = true;
        }

        private void llGuardianForm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Student.StudentID == -1)
            {
                MessageBox.Show("Save the student first before assigning a guardian.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var dtGuardians = clsGuardianStudents.GetGuardiansForStudent(_Student.StudentID);

            if (dtGuardians.Rows.Count > 0)
            {
                _GuardianID = Convert.ToInt32(dtGuardians.Rows[0]["GuardianID"]);
                _Guardian = clsGuardian.FindGuardianByID(_GuardianID);
                _Person = clsPerson.Find(_Guardian.PersonID);

                // Ask the user what they want to do
                var result = MessageBox.Show(
                    "This student already has a guardian.\nDo you want to edit the current guardian's info?\nClick 'No' to select a new guardian.",
                    "Guardian Exists",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    // Edit existing guardian
                    frmAddUpdateGuardian frm2 = new frmAddUpdateGuardian(_Guardian.GuardianID);
                    frm2.DataBack += Frm_DataBack;
                    checkBox1.Checked = true;
                    frm2.ShowDialog();
                    return;
                }
                else if (result == DialogResult.No)
                {
                    // Select a new guardian
                    ShowGuardianListForm();
                    return;
                }
                else
                {
                    // Cancel clicked → do nothing
                    return;
                }
            }
            else
            {
                // No guardian yet → select new
                ShowGuardianListForm();
            }
        }
        // Extracted method to show guardian selection
        private void ShowGuardianListForm()
        {
            GuardianList frmList = new GuardianList();

            frmList.GuardianSelected += (s, guardianID) =>
            {
                _GuardianID = guardianID;
                _Guardian = clsGuardian.FindGuardianByID(guardianID);
                _Person = clsPerson.Find(_Guardian.PersonID);

                // Link guardian to student
                clsGuardianStudents.LinkGuardianToStudent(_GuardianID, _Student.StudentID, clsGlobal.CurrentUser.UserID);

                checkBox1.Checked = true;
            };

            frmList.ShowDialog();
        }
        private void Frm_DataBack(object sender, int GuardianID)
        {
            _GuardianID = GuardianID;
            _Student.GuardianID = _GuardianID;
            checkBox1.Checked = (_GuardianID != -1);
        }



        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ctrPersonCardWithFilter1.PersonID == -1)
            {
                MessageBox.Show("Please select a person first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrPersonCardWithFilter1.FilterFocus();
                return;
            }

            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int currentUserID = clsGlobal.CurrentUser.UserID;
            string currentuser = clsGlobal.CurrentUser.UserName;
            DateTime now = DateTime.Now;

            // Make sure a person is selected
            if (ctrPersonCardWithFilter1.PersonID <= 0)
            {
                MessageBox.Show("Please select a person first.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrPersonCardWithFilter1.FilterFocus();
                return;
            }

            // Initialize _Enrollment if somehow null
            if (_Student == null)
                _Student = new clsStudent();

            // Assign the PersonID
            _Student.PersonID = ctrPersonCardWithFilter1.PersonID;
            _Student.ModifiedBy = clsGlobal.CurrentUser.UserID;  // always set ModifiedBy
            _Student.CreatedBy = clsGlobal.CurrentUser.UserID;   // for AddNew

            // Save the student
            if (!_Student.Save())
            {
                MessageBox.Show("Error: Student data could not be saved.", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblStudentID.Text = _Student.StudentID.ToString();
            _Mode = enMode.Update;  // now updating

            lblStudentID.Text = _Student.StudentID.ToString();
            _Mode = enMode.Update;

            string msg = "Student saved successfully.";

            // Handle Enrollment
            if (enroll_Mode == enrollMode.AddNew)
            {
                _Enrollment.StudentID = _Student.StudentID;
                _Enrollment.GradeID = clsGrade.FindGradeByName(cbGrades.Text)?.GradeID ?? -1;
                _Enrollment.ClassID = clsClass.FindClassByName(cbClasses.Text)?.ClassID ?? -1;

                var currentTerm = clsTerm.GetCurrentTerm();
                if (currentTerm == null)
                {
                    MessageBox.Show("No active term found. Please add a term or adjust dates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _Enrollment.TermID = currentTerm.TermID;
                _Enrollment.EnrollmentDate = now;
                _Enrollment.IsActive = chkIsActive.Checked;

                _Enrollment.CreatedByUserID = currentUserID;
                _Enrollment.CreatedAt = now;
                _Enrollment.ModifiedByUser = currentuser; // set ModifiedBy for NOT NULL
                _Enrollment.ModifiedAt = now;

                if (!_Enrollment.Save())
                {
                    MessageBox.Show("Error: Enrollment data could not be saved.", "Save Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _EnrollmentID = _Enrollment.EnrollmentID;
                enroll_Mode = enrollMode.Update;
                msg += "\nEnrollment saved successfully.";
            }
            else
            {
                // Updating existing enrollment
                _Enrollment.GradeID = clsGrade.FindGradeByName(cbGrades.Text)?.GradeID ?? -1;
                _Enrollment.ClassID = clsClass.FindClassByName(cbClasses.Text)?.ClassID ?? -1;
                _Enrollment.IsActive = chkIsActive.Checked;

                _Enrollment.ModifiedByUser = currentuser;
                _Enrollment.ModifiedAt = now;

                if (!_Enrollment.Save())
                {
                    MessageBox.Show("Error: Enrollment data could not be updated.", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                msg += "\nEnrollment updated successfully.";
            }

            // Handle Guardian linking
            int guardianIdToUse = _GuardianID != -1 ? _GuardianID : _Student.GuardianID;

            if (guardianIdToUse != -1 && _Student.StudentID != -1)
            {
                var dtExisting = clsGuardianStudents.GetStudentsForGuardian(guardianIdToUse);
                DataRow existingRow = dtExisting?.AsEnumerable()
                    .FirstOrDefault(r => Convert.ToInt32(r["EnrollmentID"]) == _Student.StudentID);

                clsGuardianStudents gs;
                if (existingRow != null)
                {
                    // Update existing link
                    gs = new clsGuardianStudents
                    {
                        GuardianStudentID = Convert.ToInt32(existingRow["GuardianStudentID"]),
                        GuardianID = guardianIdToUse,
                        StudentID = _Student.StudentID,
                        Relationship = _Guardian?.Relationship ?? "",
                        ModifiedBy = currentUserID,
                      
                    };
                }
                else
                {
                    // New link
                    gs = new clsGuardianStudents
                    {
                        GuardianID = guardianIdToUse,
                        StudentID = _Student.StudentID,
                        Relationship = _Guardian?.Relationship ?? "",
                        CreatedBy = currentUserID,
                       
                        ModifiedBy = currentUserID, // required for NOT NULL
                        
                    };
                }

                if (!gs.Save(currentUserID))
                {
                    MessageBox.Show("Warning: Unable to link Guardian to Student.", "Link Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    msg += "\nGuardian linked successfully.";
                }
            }

            // Show final success message
            MessageBox.Show(msg, "Save Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Update UI
            lblEnrollmentDate.Text = _Enrollment.EnrollmentDate.ToString();
            lblRegisteredBy.Text = clsUser.FindByUserID(_Enrollment.CreatedByUserID).UserName;
            llPaymentForm.Enabled = true;
        }
        private void llPaymentForm_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmTuitionPayment frm = new frmTuitionPayment(_EnrollmentID);
            

           

            frm.ShowDialog();
        }
        private void Frm_PaymentSaved(object sender, EventArgs e)
        {
            // Automatically check the checkbox when payment is saved
            checkBox2.Checked = true;
        }
        private void cbGrades_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dtGrades == null || _dtGrades.Rows.Count == 0 || cbGrades.SelectedIndex == -1)
                return;

            int selectedGradeID = Convert.ToInt32(_dtGrades.Rows[cbGrades.SelectedIndex]["GradeID"]);
            _LoadClassesByGradeID(selectedGradeID);
        }
    }
    }
