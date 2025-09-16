namespace SchoolProject.Students
{
    partial class frmStudentDetail
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
            this.ctrlStudentCard1 = new SchoolProject.Students.ctrlStudentCard();
            this.ctrlStudentCard2 = new SchoolProject.Students.ctrlStudentCard();
            this.lblTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ctrlStudentCard1
            // 
            this.ctrlStudentCard1.BackColor = System.Drawing.Color.White;
            this.ctrlStudentCard1.Location = new System.Drawing.Point(1, 38);
            this.ctrlStudentCard1.Name = "ctrlStudentCard1";
            this.ctrlStudentCard1.Size = new System.Drawing.Size(766, 282);
            this.ctrlStudentCard1.TabIndex = 0;
            // 
            // ctrlStudentCard2
            // 
            this.ctrlStudentCard2.BackColor = System.Drawing.Color.White;
            this.ctrlStudentCard2.Location = new System.Drawing.Point(452, 372);
            this.ctrlStudentCard2.Name = "ctrlStudentCard2";
            this.ctrlStudentCard2.Size = new System.Drawing.Size(8, 8);
            this.ctrlStudentCard2.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(259, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(258, 39);
            this.lblTitle.TabIndex = 92;
            this.lblTitle.Text = "Student Details";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmStudentDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(765, 314);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.ctrlStudentCard2);
            this.Controls.Add(this.ctrlStudentCard1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmStudentDetail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmStudentDetail";
            this.ResumeLayout(false);

        }

        #endregion

        private ctrlStudentCard ctrlStudentCard1;
        private ctrlStudentCard ctrlStudentCard2;
        private System.Windows.Forms.Label lblTitle;
    }
}