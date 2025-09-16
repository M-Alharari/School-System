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
    public partial class GuardianStudentsList : Form
    {
        public GuardianStudentsList()
        {
            InitializeComponent();
        }

        private void showPersonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GuardianStudentsDetails guardianStudentsDetails = new GuardianStudentsDetails((int)dgvGuardians.CurrentRow.Cells[0].Value);

        }
    }
}
