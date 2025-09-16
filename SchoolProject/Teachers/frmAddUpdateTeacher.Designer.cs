namespace SchoolProject.Teachers
{
    partial class frmAddUpdateTeacher
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
            this.errorProvider3 = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTeacherID = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.gbTeacher = new System.Windows.Forms.GroupBox();
            this.chkHasSubjects = new System.Windows.Forms.CheckBox();
            this.chkHasClasses = new System.Windows.Forms.CheckBox();
            this.llAssigntoSubjects = new System.Windows.Forms.LinkLabel();
            this.llAssigntoClasses = new System.Windows.Forms.LinkLabel();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.ctrlEmployeeCardWithFilter1 = new SchoolProject.Employees.ctrlEmployeeCardWithFilter();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.gbTeacher.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider3
            // 
            this.errorProvider3.ContainerControl = this;
            // 
            // errorProvider2
            // 
            this.errorProvider2.ContainerControl = this;
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.Location = new System.Drawing.Point(701, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(113, 36);
            this.btnSave.TabIndex = 17;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.Color.White;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Red;
            this.lblTitle.Location = new System.Drawing.Point(5, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(827, 37);
            this.lblTitle.TabIndex = 159;
            this.lblTitle.Text = "Add New Teacher";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTeacherID
            // 
            this.lblTeacherID.AutoSize = true;
            this.lblTeacherID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTeacherID.Location = new System.Drawing.Point(169, 35);
            this.lblTeacherID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTeacherID.Name = "lblTeacherID";
            this.lblTeacherID.Size = new System.Drawing.Size(49, 20);
            this.lblTeacherID.TabIndex = 150;
            this.lblTeacherID.Text = "[???]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 35);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 20);
            this.label3.TabIndex = 148;
            this.label3.Text = "Teacher ID: ";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog1";
            // 
            // gbTeacher
            // 
            this.gbTeacher.Controls.Add(this.chkHasSubjects);
            this.gbTeacher.Controls.Add(this.chkHasClasses);
            this.gbTeacher.Controls.Add(this.llAssigntoSubjects);
            this.gbTeacher.Controls.Add(this.llAssigntoClasses);
            this.gbTeacher.Controls.Add(this.btnSave);
            this.gbTeacher.Controls.Add(this.lblTeacherID);
            this.gbTeacher.Controls.Add(this.label3);
            this.gbTeacher.Controls.Add(this.pictureBox3);
            this.gbTeacher.Location = new System.Drawing.Point(12, 452);
            this.gbTeacher.Name = "gbTeacher";
            this.gbTeacher.Size = new System.Drawing.Size(820, 78);
            this.gbTeacher.TabIndex = 160;
            this.gbTeacher.TabStop = false;
            this.gbTeacher.Text = "Teacher Details";
            // 
            // chkHasSubjects
            // 
            this.chkHasSubjects.AutoSize = true;
            this.chkHasSubjects.Enabled = false;
            this.chkHasSubjects.Location = new System.Drawing.Point(550, 39);
            this.chkHasSubjects.Name = "chkHasSubjects";
            this.chkHasSubjects.Size = new System.Drawing.Size(15, 14);
            this.chkHasSubjects.TabIndex = 154;
            this.chkHasSubjects.UseVisualStyleBackColor = true;
            // 
            // chkHasClasses
            // 
            this.chkHasClasses.AutoSize = true;
            this.chkHasClasses.Enabled = false;
            this.chkHasClasses.Location = new System.Drawing.Point(374, 39);
            this.chkHasClasses.Name = "chkHasClasses";
            this.chkHasClasses.Size = new System.Drawing.Size(15, 14);
            this.chkHasClasses.TabIndex = 153;
            this.chkHasClasses.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.chkHasClasses.UseVisualStyleBackColor = true;
            // 
            // llAssigntoSubjects
            // 
            this.llAssigntoSubjects.AutoSize = true;
            this.llAssigntoSubjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llAssigntoSubjects.Location = new System.Drawing.Point(408, 35);
            this.llAssigntoSubjects.Name = "llAssigntoSubjects";
            this.llAssigntoSubjects.Size = new System.Drawing.Size(141, 20);
            this.llAssigntoSubjects.TabIndex = 152;
            this.llAssigntoSubjects.TabStop = true;
            this.llAssigntoSubjects.Text = "Assign to Subjects";
            this.llAssigntoSubjects.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAssigntoSubjects_LinkClicked);
            // 
            // llAssigntoClasses
            // 
            this.llAssigntoClasses.AutoSize = true;
            this.llAssigntoClasses.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llAssigntoClasses.Location = new System.Drawing.Point(232, 35);
            this.llAssigntoClasses.Name = "llAssigntoClasses";
            this.llAssigntoClasses.Size = new System.Drawing.Size(135, 20);
            this.llAssigntoClasses.TabIndex = 151;
            this.llAssigntoClasses.TabStop = true;
            this.llAssigntoClasses.Text = "Assign to Classes";
            this.llAssigntoClasses.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llAssigntoClasses_LinkClicked);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::SchoolProject.Properties.Resources.Number_32;
            this.pictureBox3.Location = new System.Drawing.Point(131, 29);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(31, 26);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 149;
            this.pictureBox3.TabStop = false;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // ctrlEmployeeCardWithFilter1
            // 
            this.ctrlEmployeeCardWithFilter1.BackColor = System.Drawing.Color.White;
            this.ctrlEmployeeCardWithFilter1.FilterEnabled = true;
            this.ctrlEmployeeCardWithFilter1.Location = new System.Drawing.Point(3, 43);
            this.ctrlEmployeeCardWithFilter1.Name = "ctrlEmployeeCardWithFilter1";
            this.ctrlEmployeeCardWithFilter1.ShowAddEmploye = true;
            this.ctrlEmployeeCardWithFilter1.Size = new System.Drawing.Size(829, 403);
            this.ctrlEmployeeCardWithFilter1.TabIndex = 161;
            // 
            // frmAddUpdateTeacher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(839, 539);
            this.Controls.Add(this.ctrlEmployeeCardWithFilter1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.gbTeacher);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmAddUpdateTeacher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmAddUpdateTeacher";
            this.Load += new System.EventHandler(this.frmAddUpdateTeacher_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.gbTeacher.ResumeLayout(false);
            this.gbTeacher.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider3;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox gbTeacher;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTeacherID;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.ErrorProvider errorProvider2;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Employees.ctrlEmployeeCardWithFilter ctrlEmployeeCardWithFilter1;
        private System.Windows.Forms.CheckBox chkHasSubjects;
        private System.Windows.Forms.CheckBox chkHasClasses;
        private System.Windows.Forms.LinkLabel llAssigntoSubjects;
        private System.Windows.Forms.LinkLabel llAssigntoClasses;
    }
}