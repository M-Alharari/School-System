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
    public partial class frmActivateLicense : Form
    {
        public frmActivateLicense()
        {
            InitializeComponent();
        }

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            string key = txtLicenseKey.Text.Trim();

            if (LicenseManager.ValidateLicense(key))
            {
                LicenseManager.SaveLicense(key); // Save to license.dat
                MessageBox.Show("License activated successfully!");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid license key!");
            }

        }
    }
}
