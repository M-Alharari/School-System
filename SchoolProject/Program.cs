 
using SchoolProject.Assessment_and_Exams;
using SchoolProject.Attendance;
using SchoolProject.Classes;
using SchoolProject.Employees;
using SchoolProject.Enrollment_Management;
using SchoolProject.Fees_and_Payments;
using SchoolProject.Guardians;
using SchoolProject.People;
using SchoolProject.Positions;
using SchoolProject.Receipts;
using SchoolProject.Subjects;
using SchoolProject.Teachers;
using SchoolProject.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace SchoolProject
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // License check BEFORE login
            if (!LicenseManager.IsLicenseValid())
            {
                using (var activateForm = new frmActivateLicense())
                {
                    if (activateForm.ShowDialog() != DialogResult.OK)
                    {
                        MessageBox.Show("License is required. Application will exit.");
                        return; // Exit application
                    }
                }
            }

            // Show login form
            using (var loginForm = new frmLogin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    // Only run main form if login succeeds
                    Application.Run(new frmMain(loginForm));
                }
            }
        }
    }

}
