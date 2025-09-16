 
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
using static SchoolProjectBusiness.clsStudentBehaviour;

namespace SchoolProject.Behaviours
{
    public partial class ShowBehavioursDetails : Form
    {
        int _behaviourID = -1;
        public ShowBehavioursDetails(int behaviourID)
        {
            InitializeComponent();
            _behaviourID = behaviourID;
        }

        private void BehavioursList_Load(object sender, EventArgs e)
        {
            // Load grades
            cmbGrade.DataSource = clsGrade.GetAllGrades(); // assuming returns DataTable/List with GradeID, Name
            cmbGrade.DisplayMember = "Name";
            cmbGrade.ValueMember = "GradeID";

            // Load classes
            cmbClass.DataSource = clsClass.GetAllClasses(); // assuming returns DataTable/List with ClassID, Name
            cmbClass.DisplayMember = "Name";
            cmbClass.ValueMember = "ClassID";

            cmbGrade.SelectedIndexChanged += ComboBox_SelectionChanged;
            cmbClass.SelectedIndexChanged += ComboBox_SelectionChanged;

            // Trigger initial load
            LoadBehaviourDetails(_behaviourID);
        }
        private void ComboBox_SelectionChanged(object sender, EventArgs e)
        {
            LoadBehaviourDetails(_behaviourID);
        }

        private void LoadBehaviourDetails(int behaviourID)
        {
            var dt = clsStudentBehaviour.GetBehavioursByEnrollmentWithEnums(behaviourID);

            if (dt == null || dt.Rows.Count == 0)
            {
                lblBehaviourType.Text = "-";
                lblCategory.Text = "-";
                lblSeverity.Text = "-";
                lblAction.Text = "-";
                lblDescription.Text = "-";
                lblCreatedDate.Text = "-";
                return;
            }

            // If you want to show only the latest behaviour, take the first row
            var row = dt.Rows[0];

            lblFullName.Text = row["FullName"].ToString();
            lblCreatedDate.Text = row["CreatedDateStr"].ToString();
            lblBehaviourType.Text = row["BehaviourTypeName"].ToString();
            lblCategory.Text = row["CategoryName"].ToString();
            lblSeverity.Text = row["SeverityName"].ToString();
            lblAction.Text = row["ActionName"].ToString();
            lblDescription.Text = row["Description"]?.ToString() ?? "-";
            lblCreatedDate.Text = row["DateRecorded"] != DBNull.Value
                ? Convert.ToDateTime(row["DateRecorded"]).ToString("yyyy-MM-dd")
                : "-";


            // If you want to **show all behaviours** in the labels, you can loop:
            /*
            lblBehaviourType.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r["BehaviourTypeName"]));
            lblCategory.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r["CategoryName"]));
            lblSeverity.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r["SeverityName"]));
            lblAction.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r["ActionName"]));
            lblDescription.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r["Description"]));
            lblDateRecorded.Text = string.Join(Environment.NewLine, dt.AsEnumerable().Select(r => r.Field<DateTime?>("DateRecorded")?.ToString("yyyy-MM-dd") ?? "-"));
            */
        }




















    }
}
