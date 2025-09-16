namespace SchoolProject.Guardians
{
    partial class frmAddUpdateGuardian
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
            this.gbGuardian = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRelationship = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblGuardianID = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.ctGuardianCardWithFilter1 = new SchoolProject.Guardians.ctGuardianCardWithFilter();
            this.gbGuardian.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // gbGuardian
            // 
            this.gbGuardian.Controls.Add(this.label1);
            this.gbGuardian.Controls.Add(this.txtRelationship);
            this.gbGuardian.Controls.Add(this.btnSave);
            this.gbGuardian.Controls.Add(this.lblGuardianID);
            this.gbGuardian.Controls.Add(this.pictureBox3);
            this.gbGuardian.Controls.Add(this.label5);
            this.gbGuardian.Location = new System.Drawing.Point(2, 387);
            this.gbGuardian.Name = "gbGuardian";
            this.gbGuardian.Size = new System.Drawing.Size(769, 69);
            this.gbGuardian.TabIndex = 2;
            this.gbGuardian.TabStop = false;
            this.gbGuardian.Text = "Guardian Box";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(320, 31);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 172;
            this.label1.Text = "Relationship:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtRelationship
            // 
            this.txtRelationship.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRelationship.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRelationship.Location = new System.Drawing.Point(441, 31);
            this.txtRelationship.Name = "txtRelationship";
            this.txtRelationship.Size = new System.Drawing.Size(145, 24);
            this.txtRelationship.TabIndex = 171;
            // 
            // btnSave
            // 
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.Location = new System.Drawing.Point(628, 19);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(113, 36);
            this.btnSave.TabIndex = 170;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblGuardianID
            // 
            this.lblGuardianID.AutoSize = true;
            this.lblGuardianID.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGuardianID.Location = new System.Drawing.Point(172, 28);
            this.lblGuardianID.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGuardianID.Name = "lblGuardianID";
            this.lblGuardianID.Size = new System.Drawing.Size(59, 20);
            this.lblGuardianID.TabIndex = 164;
            this.lblGuardianID.Text = "[????]";
            this.lblGuardianID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::SchoolProject.Properties.Resources.briefcase;
            this.pictureBox3.Location = new System.Drawing.Point(134, 22);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(31, 26);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 163;
            this.pictureBox3.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(11, 28);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(112, 20);
            this.label5.TabIndex = 162;
            this.label5.Text = "Guardian ID:";
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(-5, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(831, 39);
            this.lblTitle.TabIndex = 131;
            this.lblTitle.Text = "Add New Guardian";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctGuardianCardWithFilter1
            // 
            this.ctGuardianCardWithFilter1.BackColor = System.Drawing.Color.White;
            this.ctGuardianCardWithFilter1.Location = new System.Drawing.Point(2, 52);
            this.ctGuardianCardWithFilter1.Name = "ctGuardianCardWithFilter1";
            this.ctGuardianCardWithFilter1.PersonCannotBeSelectedCheck = null;
            this.ctGuardianCardWithFilter1.Size = new System.Drawing.Size(769, 329);
            this.ctGuardianCardWithFilter1.TabIndex = 132;
            // 
            // frmAddUpdateGuardian
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(777, 462);
            this.Controls.Add(this.ctGuardianCardWithFilter1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.gbGuardian);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmAddUpdateGuardian";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmAddUpdateGuardian";
            this.Load += new System.EventHandler(this.frmAddUpdateGuardian_Load);
            this.gbGuardian.ResumeLayout(false);
            this.gbGuardian.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbGuardian;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblGuardianID;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRelationship;
        private ctGuardianCardWithFilter ctGuardianCardWithFilter1;
    }
}