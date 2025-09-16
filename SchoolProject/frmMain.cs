using SchoolProject.Assessment_and_Exams;
using SchoolProject.Assessment_and_Exams.Exam_Types;
using SchoolProject.Assigning_Forms;
using SchoolProject.Assigning_Forms.Assign_Subjects_to_Grades;
using SchoolProject.Attendance;
using SchoolProject.Audit_Log;
using SchoolProject.Behaviours;
using SchoolProject.Classes;
using SchoolProject.Comparisons;
using SchoolProject.Comparisons.Trends;
using SchoolProject.Employees;
using SchoolProject.Enrollment_Management;
using SchoolProject.Fees_and_Payments;
using SchoolProject.Fees_and_Payments.Fee_Types;
using SchoolProject.Global_Classes;
using SchoolProject.Graduation;
using SchoolProject.Guardians;
using SchoolProject.People;
using SchoolProject.Positions;
using SchoolProject.Receipts;
using SchoolProject.Salary_Deduction;
using SchoolProject.School_Info;
using SchoolProject.Students;
using SchoolProject.Subjects;
using SchoolProject.Teachers;
using SchoolProject.Terms;
using SchoolProject.Users;
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
    public partial class frmMain : Form
    {
        frmLogin _frmLogin;
        private Timer notificationTimer;
        private bool notificationShown = false;
        public frmMain(frmLogin frmLogin)
        {
            InitializeComponent();
            _frmLogin = frmLogin;
            //SetupNotificationTimer();
        }

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPeopleManage frm = new frmPeopleManage();
            frm.ShowDialog();


        }
        //private void SetupNotificationTimer()
        //{
        //    notificationTimer = new Timer();
        //    notificationTimer.Interval = 60000; // تفحص كل 60 ثانية (يمكن التعديل حسب الحاجة)
        //    notificationTimer.Tick += NotificationTimer_Tick;
        //    notificationTimer.Start();
        //}

        //private void NotificationTimer_Tick(object sender, EventArgs e)
        //{
        //    if (notificationShown)
        //        return;

        //    System.Diagnostics.Debug.WriteLine("NotificationTimer_Tick fired at " + DateTime.Now);

        //    DataTable allInstallments = clsInstallment.GetAllInstallmentsAsDataTable();

        //    HashSet<int> dueTuitionFeeIDs = new HashSet<int>();

        //    foreach (DataRow row in allInstallments.Rows)
        //    {
        //        DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
        //        bool isPaid = Convert.ToBoolean(row["IsPaid"]);
        //        int tuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]);

        //        if (!isPaid && dueDate <= DateTime.Today)
        //        {
        //            dueTuitionFeeIDs.Add(tuitionFeeID);
        //        }
        //    }

        //    System.Diagnostics.Debug.WriteLine("Found due TuitionFeeIDs count: " + dueTuitionFeeIDs.Count);

        //    if (dueTuitionFeeIDs.Count > 0)
        //    {
        //        notificationShown = true;

        //        //var notificationForm = new frmNotification(dueTuitionFeeIDs.ToList());
        //        notificationForm.FormClosed += (s, args) =>
        //        {
        //            notificationShown = false;
        //        };
        //        notificationForm.ShowDialog();  // جرب ShowDialog بدل Show
        //    }
        //}

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmStudentList frm  = new frmStudentList();
            //frm.ShowDialog();
        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEnrollmentList frm = new frmEnrollmentList();    
            frm.ShowDialog();
        }

        private void addClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClasseslist frm = new frmClasseslist();
            frm.ShowDialog();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmManageUsers frm = new frmManageUsers();
            frm.ShowDialog();
        }

        private void currentUserInfoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           frmUserInfo frm = new frmUserInfo(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void changePasswordToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           frmChangePassword frm = new frmChangePassword(clsGlobal.CurrentUser.UserID);
            frm.ShowDialog();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!LicenseManager.IsLicenseValid())
            {
                // Show license activation form
                using (var activateForm = new frmActivateLicense())
                {
                    var result = activateForm.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        // Instead of exiting immediately, just show a message
                        MessageBox.Show("License expired! You can only view notifications.");
                        // optionally: disable menus or certain features
                    }
                }
            }

            this.BackColor = Color.White;
            //ShowDueInstallmentsNotificationIfAny();
            lblUsername.Text = $"Welcome, User:{clsGlobal.CurrentUser.UserName}";
        }
        //private void ShowDueInstallmentsNotificationIfAny()
        //{
        //    DataTable allInstallments = clsInstallment.GetAllInstallmentsAsDataTable();

        //    HashSet<int> dueTuitionFeeIDs = new HashSet<int>();

        //    foreach (DataRow row in allInstallments.Rows)
        //    {
        //        DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
               
        //        int tuitionFeeID = Convert.ToInt32(row["TuitionFeeID"]);

        //        if ( dueDate <= DateTime.Today)
        //        {
        //            dueTuitionFeeIDs.Add(tuitionFeeID);
        //        }
        //    }

        //    if (dueTuitionFeeIDs.Count > 0)
        //    {
        //        notificationShown = true;
        //        this.BackColor = Color.White;
        //        var notificationForm = new frmNotification(dueTuitionFeeIDs.ToList());
        //        notificationForm.FormClosed += (s, args) =>
        //        {
        //            notificationShown = false; // إعادة تفعيل عرض التنبيه بعد إغلاق النافذة
        //        };
        //        notificationForm.ShowDialog(); // Show() ليست ShowDialog() حتى لا توقف frmMain
              
        //    }
        //}
        private void teachersLsitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTeacherList frm = new frmTeacherList();
            frm.ShowDialog();
        }

        private void studentsListToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmStudentList frm = new frmStudentList();
            frm.ShowDialog();
        }

        private void gradesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGradeslist frm = new frmGradeslist();
            frm.ShowDialog();
        }

        private void classesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmClasseslist frm = new frmClasseslist();
            frm.ShowDialog();
        }

        private void subjectsListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSubjectList_cs frm = new frmSubjectList_cs();
            frm.ShowDialog();
        }

        private void employeesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmEmployeelist frm = new frmEmployeelist();

            frm.ShowDialog();
        }

        private void positionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPositionManage frm = new frmPositionManage();
            frm.ShowDialog();
        }

        private void peopleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPeopleManage frm = new frmPeopleManage();    
            frm.ShowDialog();
        }

        private void feesToolStripMenuItem_Click(object sender, EventArgs e)
        {
           frmPaymentManage frm = new frmPaymentManage();
            frm.ShowDialog();
        }

        private void examScoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GradeClassStudentsExam frmStudentMarksEntry = new GradeClassStudentsExam();
             frmStudentMarksEntry.ShowDialog();
        }

        private void frmAssignSubjectsToGradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAssignSubjectsToGrades frmAssignSubjectsToGrades = new frmAssignSubjectsToGrades();
            frmAssignSubjectsToGrades.ShowDialog();
        }

        private void managePaymentTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPaymentManage frm = new frmPaymentManage();
            frm.ShowDialog();
        }

        private void feesListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPaymentManage frmPaymentManage = new frmPaymentManage();
            frmPaymentManage.ShowDialog();
        }

        private void assignClassesToTeachersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssignClassesToTeachers assignClassesToTeachers = new AssignClassesToTeachers();
            assignClassesToTeachers.ShowDialog();
        }

        private void assignSubjectsToTeachersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //AssignTeachersToSubjecs assignTeachersToSubjecs     = new AssignTeachersToSubjecs(_);
            //assignTeachersToSubjecs.ShowDialog();
        }

        private void changeSchoolInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSchoolInfo frmSchoolInfo = new frmSchoolInfo();
            frmSchoolInfo.ShowDialog();
        }

        private void receiptDemoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReceiptDemo receiptDemo = new ReceiptDemo();
            receiptDemo.ShowDialog();
        }

        private void feeTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddUpdateFeeTypes addUpdateFeeTypes = new AddUpdateFeeTypes();
            addUpdateFeeTypes.ShowDialog();
        }

        private void addStudentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAttendaceManagement frmAttendaceManagement = new frmAttendaceManagement();
            frmAttendaceManagement.ShowDialog();
        }

        private void monthlyAttendanceToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void salaryDeductionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSalaryDeductionSummary frmSalaryDeductionSummary = new frmSalaryDeductionSummary();
            frmSalaryDeductionSummary.ShowDialog();
        }

        private void termsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTermslist  Termlist = new frmTermslist();
            Termlist.ShowDialog();
        }

        private void examTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmExamTypeslist frmExamTypeslist = new frmExamTypeslist();
            frmExamTypeslist.ShowDialog();
        }

        private void gradeClassStudentsExamToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GradeClassStudentsExam gradeClassStudentsExam = new GradeClassStudentsExam();
            gradeClassStudentsExam.ShowDialog();
        }

        private void frmGraduateStudentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGraduateStudents frmGraduateStudents = new frmGraduateStudents();
            frmGraduateStudents.ShowDialog();
        }

        private void typeScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void compareGradesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCompareGradesScores frmCompareGradesScores = new frmCompareGradesScores();
            frmCompareGradesScores.ShowDialog();
        }

        private void frmCompareStudentsInAClassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCompareStudentsInAClass frmCompareStudentsInAClass = new frmCompareStudentsInAClass();
            frmCompareStudentsInAClass.ShowDialog();
        }

        private void frmStudentBehaviorslistToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void frmAuditLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAuditLog frmAuditLog = new frmAuditLog();
            frmAuditLog.ShowDialog();   
        }

        private void gurdiansToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuardianList guardianList = new GuardianList();
            guardianList.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void oNewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            frmAttendaceManagement frmAttendaceManagement = new frmAttendaceManagement();
            frmAttendaceManagement.ShowDialog();
        }

        private void ManageDetainedLicensestoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmPaymentManage frm = new frmPaymentManage();
            frm.ShowDialog();
        }

        private void studentResultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GradeClassStudentsExam frmStudentMarksEntry = new GradeClassStudentsExam();
            frmStudentMarksEntry.ShowDialog();
        }

        private void assignSubjectsToClassesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAssignSubjectsToGrades frmAssignSubjectsToGrades = new frmAssignSubjectsToGrades();
            frmAssignSubjectsToGrades.ShowDialog();
        }

        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
        }

        private void ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSalaryDeductionSummary frmSalaryDeductionSummary = new frmSalaryDeductionSummary();
            frmSalaryDeductionSummary.ShowDialog();
        }

        private void graduateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmGraduateStudents frmGraduateStudents = new frmGraduateStudents();
            frmGraduateStudents.ShowDialog();
        }

        private void feeManagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCompareGradesScores frmCompareGradesScores = new frmCompareGradesScores();
            frmCompareGradesScores.ShowDialog();
        }

        private void behavioursListToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void compareClassesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCompareStudentsInAClass frmCompareStudentsInAClass = new frmCompareStudentsInAClass();
            frmCompareStudentsInAClass.ShowDialog();
        }

        private void auditLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAuditLog frmAuditLog = new frmAuditLog();
            frmAuditLog.ShowDialog();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            frmTermslist Termlist = new frmTermslist();
            Termlist.ShowDialog();
        }

        private void frmStudentPerformanceTrendsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStudentPerformanceTrends frmStudentPerformanceTrends = new frmStudentPerformanceTrends();
            frmStudentPerformanceTrends.ShowDialog();
        }

        private void enrollmentDashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnrollmnetsDashboard enrollmnetsDashboard    = new EnrollmnetsDashboard();
            enrollmnetsDashboard.ShowDialog();
        }

        private void frmfinancialdashboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmfinancialdashboard frmfinancialdashboard 
                = new frmfinancialdashboard();
            frmfinancialdashboard.ShowDialog();
        }

        private void detainLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEmployeesPayroll frmEmployeesPayroll = new frmEmployeesPayroll();
            frmEmployeesPayroll.ShowDialog();
        }

        private void timeTableGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeTableGenerator timeTableGenerator = new TimeTableGenerator();
            timeTableGenerator.ShowDialog();
        }
    }
}
//public class frmNotification : Form
//{
//    private FlowLayoutPanel panel;
//    private List<int> _tuitionFeeIDs;
//    private Button btnClose;

//    public frmNotification(List<int> tuitionFeeIDs)
//    {
//        _tuitionFeeIDs = tuitionFeeIDs;

//        Text = "Installments Due Notification";
//        Size = new Size(700, 450);
//        StartPosition = FormStartPosition.CenterScreen;
//        FormBorderStyle = FormBorderStyle.FixedToolWindow;
//        BackColor = Color.White;
//        KeyPreview = true; // Enable keyboard shortcuts

//        panel = new FlowLayoutPanel
//        {
//            Dock = DockStyle.Top,
//            AutoScroll = true,
//            Padding = new Padding(10),
//            WrapContents = false,
//            FlowDirection = FlowDirection.TopDown,
//            Height = 350
//        };

//        btnClose = new Button
//        {
//            Text = "Close",
//            AutoSize = true,
//            Anchor = AnchorStyles.Right,
//            DialogResult = DialogResult.OK
//        };
//        btnClose.Click += (s, e) => this.Close();
//        btnClose.TabIndex = 0;

//        Controls.Add(panel);
//        Controls.Add(btnClose);

//        // Place close button correctly after panel
//        btnClose.Top = panel.Bottom + 10;
//        btnClose.Left = this.ClientSize.Width - btnClose.Width - 20;
//    }

//    // Load data after form is fully shown
//    protected override void OnShown(EventArgs e)
//    {
//        base.OnShown(e);
//        LoadDueInstallments();
//    }

//    private void LoadDueInstallments()
//    {
//        panel.Controls.Clear();

//        foreach (int tuitionFeeID in _tuitionFeeIDs)
//        {
//            DataTable dt = clsInstallment.GetInstallmentSummaryByTuitionFeeID(tuitionFeeID);

//            // Use LINQ to filter due and unpaid rows
//            var dueRows = dt.AsEnumerable()
//                            .Where(r => !Convert.ToBoolean(r["IsPaid"]) &&
//                                        Convert.ToDateTime(r["DueDate"]) <= DateTime.Today)
//                            .ToList();

//            foreach (var row in dueRows)
//            {
//                int installmentID = Convert.ToInt32(row["InstallmentID"]);
//                int installmentNumber = Convert.ToInt32(row["InstallmentNumber"]);
//                DateTime dueDate = Convert.ToDateTime(row["DueDate"]);
//                decimal amount = Convert.ToDecimal(row["Amount"]);
//                string fullName = row["FullName"].ToString();

//                FlowLayoutPanel container = new FlowLayoutPanel
//                {
//                    AutoSize = true,
//                    FlowDirection = FlowDirection.LeftToRight,
//                    Margin = new Padding(5)
//                };

//                Button btnPay = new Button
//                {
//                    Text = "Pay",
//                    Tag = installmentID,
//                    AutoSize = true
//                };
//                btnPay.Click += BtnPay_Click;

//                Label lbl = new Label
//                {
//                    Text = $"Student: {fullName} | TuitionFeeID: {tuitionFeeID} | " +
//                           $"Installment #{installmentNumber} | Due: {dueDate:dd/MM/yyyy} | Amount: {amount:0.00}",
//                    AutoSize = true,
//                    Padding = new Padding(5, 8, 5, 5)
//                };

//                container.Controls.Add(btnPay);
//                container.Controls.Add(lbl);

//                panel.Controls.Add(container);
//            }
//        }

//        // Debug if panel is empty
//        if (panel.Controls.Count == 0)
//            MessageBox.Show("No due installments found.", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
//    }

//    private void BtnPay_Click(object sender, EventArgs e)
//    {
//        if (sender is Button btn)
//        {
//            int installmentID = (int)btn.Tag;
//            var payForm = new frmPayInstallment(installmentID);
//            var result = payForm.ShowDialog();

//            if (result == DialogResult.OK)
//            {
//                MessageBox.Show("Payment completed successfully.");
//                LoadDueInstallments(); // Refresh the list after payment
//            }
//        }
//    }

//    // Allow Esc to close
//    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
//    {
//        if (keyData == Keys.Escape)
//        {
//            Close();
//            return true;
//        }
//        return base.ProcessCmdKey(ref msg, keyData);
//    }
//}

