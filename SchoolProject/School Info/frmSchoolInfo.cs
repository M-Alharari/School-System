using SchoolProject.Global_Classes;
using SchoolProject.Properties;
using SchoolProjectBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolProject.School_Info
{
    public partial class frmSchoolInfo : Form
    {
        public enum enMode { AddNew, Update }

        private enMode _Mode;
        private clsSchoolInfo _SchoolInfo;
        public frmSchoolInfo()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
        }

        private void frmSchoolInfo_Load(object sender, EventArgs e)
        {
            _LoadSchoolInfo();
        }
        private void _LoadSchoolInfo()
        {
            _SchoolInfo = clsSchoolInfo.GetSchoolInfo();

            if (_SchoolInfo == null)
            {
                // No record exists → Add mode
                _Mode = enMode.AddNew;
                _SchoolInfo = new clsSchoolInfo();
            }
            else
            {
                // Record exists → Update mode
                _Mode = enMode.Update;

                txtSchoolName.Text = _SchoolInfo.SchoolName;
                txtAddress.Text = _SchoolInfo.Address;
                txtPhone.Text = _SchoolInfo.Phone;
                txtEmail.Text = _SchoolInfo.Email;
                txtWebsite.Text = _SchoolInfo.Website;

                if (!string.IsNullOrEmpty(_SchoolInfo.LogoPath) && File.Exists(_SchoolInfo.LogoPath))
                {
                    pbLogo.ImageLocation = _SchoolInfo.LogoPath;
                    llRemoveLogo.Visible = true;
                }
            }
        }

        private bool _HandleLogo()
        {
            if (_SchoolInfo.LogoPath != pbLogo.ImageLocation)
            {
                // Delete old logo if exists
                if (!string.IsNullOrEmpty(_SchoolInfo.LogoPath))
                {
                    try { File.Delete(_SchoolInfo.LogoPath); } catch { }
                }

                if (!string.IsNullOrEmpty(pbLogo.ImageLocation))
                {
                    string destFile = pbLogo.ImageLocation;
                    if (!clsUtil.CopyImageToProjectImagesFolder(ref destFile))
                    {
                        MessageBox.Show("Error copying logo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    _SchoolInfo.LogoPath = destFile;
                }
                else
                {
                    _SchoolInfo.LogoPath = null; // user removed logo
                }
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!_HandleLogo()) return;

            _SchoolInfo.SchoolName = txtSchoolName.Text.Trim();
            _SchoolInfo.Address = txtAddress.Text.Trim();
            _SchoolInfo.Phone = txtPhone.Text.Trim();
            _SchoolInfo.Email = txtEmail.Text.Trim();
            _SchoolInfo.Website = txtWebsite.Text.Trim();

            if (_SchoolInfo.Save(clsGlobal.CurrentUser.UserID))
            {
                MessageBox.Show("School info saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Error saving school info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void llSetLogo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string destFile = openFileDialog1.FileName;
                if (clsUtil.CopyImageToProjectImagesFolder(ref destFile))
                {
                    pbLogo.ImageLocation = destFile;
                    _SchoolInfo.LogoPath = destFile;
                }
            }
        }

   

        private void llSetImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            openFileDialog1.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                pbLogo.Load(file);
                llRemoveLogo.Visible = true;
            }
        }

        private void llRemoveLogo_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
          
            pbLogo.ImageLocation = null;
            _SchoolInfo.LogoPath = null;
            llRemoveLogo.Visible = false;
        }
    }
}
