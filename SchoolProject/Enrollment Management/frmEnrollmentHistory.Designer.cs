namespace SchoolProject.Enrollment_Management
{
    partial class frmEnrollmentHistory
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ctrlStudentCard1 = new SchoolProject.Students.ctrlStudentCard();
            this.cmsbehaviourhistory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.InternationalLicenseHistorytoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.lblBehaviourCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dgvBehaviour = new System.Windows.Forms.DataGridView();
            this.tbStudentBehaviours = new System.Windows.Forms.TabPage();
            this.tcEnrollHistory = new System.Windows.Forms.TabControl();
            this.tpStudentInfo = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            this.cmsLocalLicenseHistory = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showLicenseInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmsbehaviourhistory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBehaviour)).BeginInit();
            this.tbStudentBehaviours.SuspendLayout();
            this.tcEnrollHistory.SuspendLayout();
            this.tpStudentInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.cmsLocalLicenseHistory.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctrlStudentCard1
            // 
            this.ctrlStudentCard1.BackColor = System.Drawing.Color.White;
            this.ctrlStudentCard1.Location = new System.Drawing.Point(12, 47);
            this.ctrlStudentCard1.Name = "ctrlStudentCard1";
            this.ctrlStudentCard1.Size = new System.Drawing.Size(755, 277);
            this.ctrlStudentCard1.TabIndex = 141;
            // 
            // cmsbehaviourhistory
            // 
            this.cmsbehaviourhistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InternationalLicenseHistorytoolStripMenuItem});
            this.cmsbehaviourhistory.Name = "cmsLocalLicenseHistory";
            this.cmsbehaviourhistory.Size = new System.Drawing.Size(181, 48);
            // 
            // InternationalLicenseHistorytoolStripMenuItem
            // 
            this.InternationalLicenseHistorytoolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.InternationalLicenseHistorytoolStripMenuItem.Name = "InternationalLicenseHistorytoolStripMenuItem";
            this.InternationalLicenseHistorytoolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.InternationalLicenseHistorytoolStripMenuItem.Text = "Show License Info";
            this.InternationalLicenseHistorytoolStripMenuItem.Click += new System.EventHandler(this.InternationalLicenseHistorytoolStripMenuItem_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(233, 20);
            this.label3.TabIndex = 139;
            this.label3.Text = "Student Behaviours History:";
            // 
            // lblBehaviourCount
            // 
            this.lblBehaviourCount.AutoSize = true;
            this.lblBehaviourCount.Location = new System.Drawing.Point(108, 218);
            this.lblBehaviourCount.Name = "lblBehaviourCount";
            this.lblBehaviourCount.Size = new System.Drawing.Size(19, 13);
            this.lblBehaviourCount.TabIndex = 138;
            this.lblBehaviourCount.Text = "??";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(14, 218);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 20);
            this.label5.TabIndex = 137;
            this.label5.Text = "# Records:";
            // 
            // dgvBehaviour
            // 
            this.dgvBehaviour.AllowUserToAddRows = false;
            this.dgvBehaviour.AllowUserToDeleteRows = false;
            this.dgvBehaviour.AllowUserToResizeRows = false;
            this.dgvBehaviour.BackgroundColor = System.Drawing.Color.White;
            this.dgvBehaviour.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBehaviour.ContextMenuStrip = this.cmsbehaviourhistory;
            this.dgvBehaviour.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvBehaviour.Location = new System.Drawing.Point(18, 42);
            this.dgvBehaviour.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvBehaviour.MultiSelect = false;
            this.dgvBehaviour.Name = "dgvBehaviour";
            this.dgvBehaviour.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvBehaviour.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvBehaviour.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvBehaviour.Size = new System.Drawing.Size(710, 146);
            this.dgvBehaviour.TabIndex = 136;
            this.dgvBehaviour.TabStop = false;
            // 
            // tbStudentBehaviours
            // 
            this.tbStudentBehaviours.Controls.Add(this.label3);
            this.tbStudentBehaviours.Controls.Add(this.lblBehaviourCount);
            this.tbStudentBehaviours.Controls.Add(this.label5);
            this.tbStudentBehaviours.Controls.Add(this.dgvBehaviour);
            this.tbStudentBehaviours.Location = new System.Drawing.Point(4, 22);
            this.tbStudentBehaviours.Name = "tbStudentBehaviours";
            this.tbStudentBehaviours.Padding = new System.Windows.Forms.Padding(3);
            this.tbStudentBehaviours.Size = new System.Drawing.Size(735, 240);
            this.tbStudentBehaviours.TabIndex = 1;
            this.tbStudentBehaviours.Text = "Behaviours";
            this.tbStudentBehaviours.UseVisualStyleBackColor = true;
            // 
            // tcEnrollHistory
            // 
            this.tcEnrollHistory.Controls.Add(this.tpStudentInfo);
            this.tcEnrollHistory.Controls.Add(this.tbStudentBehaviours);
            this.tcEnrollHistory.Location = new System.Drawing.Point(12, 30);
            this.tcEnrollHistory.Name = "tcEnrollHistory";
            this.tcEnrollHistory.SelectedIndex = 0;
            this.tcEnrollHistory.Size = new System.Drawing.Size(743, 266);
            this.tcEnrollHistory.TabIndex = 131;
            // 
            // tpStudentInfo
            // 
            this.tpStudentInfo.Controls.Add(this.label1);
            this.tpStudentInfo.Controls.Add(this.lblRecordCount);
            this.tpStudentInfo.Controls.Add(this.label2);
            this.tpStudentInfo.Controls.Add(this.dgvHistory);
            this.tpStudentInfo.Location = new System.Drawing.Point(4, 22);
            this.tpStudentInfo.Name = "tpStudentInfo";
            this.tpStudentInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tpStudentInfo.Size = new System.Drawing.Size(735, 240);
            this.tpStudentInfo.TabIndex = 0;
            this.tpStudentInfo.Text = "StudentInfo";
            this.tpStudentInfo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 20);
            this.label1.TabIndex = 135;
            this.label1.Text = "Student History:";
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecordCount.Location = new System.Drawing.Point(97, 216);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(24, 18);
            this.lblRecordCount.TabIndex = 134;
            this.lblRecordCount.Text = "??";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 133;
            this.label2.Text = "# Records:";
            // 
            // dgvHistory
            // 
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.AllowUserToResizeRows = false;
            this.dgvHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.ContextMenuStrip = this.cmsLocalLicenseHistory;
            this.dgvHistory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvHistory.Location = new System.Drawing.Point(7, 43);
            this.dgvHistory.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvHistory.MultiSelect = false;
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.ReadOnly = true;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvHistory.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHistory.Size = new System.Drawing.Size(699, 160);
            this.dgvHistory.TabIndex = 132;
            this.dgvHistory.TabStop = false;
            // 
            // cmsLocalLicenseHistory
            // 
            this.cmsLocalLicenseHistory.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showLicenseInfoToolStripMenuItem});
            this.cmsLocalLicenseHistory.Name = "cmsLocalLicenseHistory";
            this.cmsLocalLicenseHistory.Size = new System.Drawing.Size(170, 26);
            // 
            // showLicenseInfoToolStripMenuItem
            // 
            this.showLicenseInfoToolStripMenuItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.showLicenseInfoToolStripMenuItem.Name = "showLicenseInfoToolStripMenuItem";
            this.showLicenseInfoToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.showLicenseInfoToolStripMenuItem.Text = "Show License Info";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tcEnrollHistory);
            this.groupBox1.Location = new System.Drawing.Point(12, 330);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(755, 302);
            this.groupBox1.TabIndex = 140;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Driver Licenses";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(45, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(702, 37);
            this.lblTitle.TabIndex = 139;
            this.lblTitle.Text = "Enrollment History";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmEnrollmentHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(775, 644);
            this.Controls.Add(this.ctrlStudentCard1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmEnrollmentHistory";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmEnrollmentHistorys";
            this.Load += new System.EventHandler(this.frmEnrollmentHistorys_Load);
            this.cmsbehaviourhistory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBehaviour)).EndInit();
            this.tbStudentBehaviours.ResumeLayout(false);
            this.tbStudentBehaviours.PerformLayout();
            this.tcEnrollHistory.ResumeLayout(false);
            this.tpStudentInfo.ResumeLayout(false);
            this.tpStudentInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.cmsLocalLicenseHistory.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Students.ctrlStudentCard ctrlStudentCard1;
        private System.Windows.Forms.ContextMenuStrip cmsbehaviourhistory;
        private System.Windows.Forms.ToolStripMenuItem InternationalLicenseHistorytoolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBehaviourCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridView dgvBehaviour;
        private System.Windows.Forms.TabPage tbStudentBehaviours;
        private System.Windows.Forms.TabControl tcEnrollHistory;
        private System.Windows.Forms.TabPage tpStudentInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvHistory;
        private System.Windows.Forms.ContextMenuStrip cmsLocalLicenseHistory;
        private System.Windows.Forms.ToolStripMenuItem showLicenseInfoToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblTitle;
    }
}