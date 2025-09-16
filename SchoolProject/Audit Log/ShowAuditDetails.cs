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

namespace SchoolProject.Audit_Log
{
    public partial class ShowAuditDetails : Form
    {
        private clsAuditLog _Audit; // instance variable, NOT static
        public ShowAuditDetails(int auditID)
        {
            InitializeComponent(); LoadAuditDetails(auditID);
        }




        private void LoadAuditDetails(int auditID)
        {
            _Audit = clsAuditLog.Find(auditID); // fetch audit entry

            if (_Audit == null)
            {
                MessageBox.Show("No audit found with ID " + auditID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // Fill labels
            lblAction.Text = _Audit.Action;
            lblTableName.Text = _Audit.TableName;
            lblAuditID.Text = _Audit.AuditID.ToString();

            // Convert PerformedBy ID to username if needed
            var user = clsUser.FindByUserID(_Audit.PerformedBy);
            lblPerformedBy.Text = user != null ? user.UserName : _Audit.PerformedBy.ToString();

            lblPerformedAt.Text = _Audit.PerformedAt.ToString("yyyy-MM-dd HH:mm:ss");
            lblOldValues.Text = string.IsNullOrEmpty(_Audit.OldValues) ? "N/A" : _Audit.OldValues;
            lblNewValues.Text = string.IsNullOrEmpty(_Audit.NewValues) ? "N/A" : _Audit.NewValues;
        }



























    }
}
