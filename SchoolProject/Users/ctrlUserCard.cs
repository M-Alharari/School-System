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

namespace SchoolProject.Users
{
    public partial class ctrlUserCard : UserControl
    {
        private clsUser _User;
        private int _UserID = -1;
     

        public int UserID
        {
            get { return _UserID; }
        }

     
        public ctrlUserCard()
        {
            InitializeComponent();
        }
        public void LoadUserInfo(int UserID)
        {
            _User = clsUser.FindByUserID(UserID);

            if (_User == null)
            {
                _ResetPersonInfo();
                MessageBox.Show("No User with UserID: " + UserID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _UserID = _User.UserID; // ✅ FIX: Store correct ID
            _FillUserInfo(); 
        }

        private void _FillUserInfo()
        {
            personcard1.LoadPersonInfo(_User.PersonID);
            lblUserID.Text = _User.UserID.ToString();
            lblUserName.Text = _User.UserName.ToString();

            if (_User.IsActive)
                lblIsActive.Text = "Yes";
            else
                lblIsActive.Text = "No";
        }



        private void _ResetPersonInfo()
        {
            personcard1.ResetPersonInfo();
            lblUserID.Text = "[???]";
            lblUserID.Text = "[???]";
            lblIsActive.Text = "[???}";
        }

      
    }
}
