using SchoolProject.People.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.Students
{
    public partial class frmStudentDetail : Form
    {
        public frmStudentDetail(int StudentID)
        {
            InitializeComponent();
            ctrlStudentCard1.LoadInfo(StudentID);
        }
    }
}
