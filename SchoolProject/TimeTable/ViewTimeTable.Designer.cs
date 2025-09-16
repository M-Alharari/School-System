namespace SchoolProject.TimeTable
{
    partial class ViewTimeTable
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
            this.BtnSwap = new System.Windows.Forms.Button();
            this.cmbGrades = new System.Windows.Forms.ComboBox();
            this.dgvClasses = new System.Windows.Forms.DataGridView();
            this.btnGenerate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClasses)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnSwap
            // 
            this.BtnSwap.Location = new System.Drawing.Point(536, 97);
            this.BtnSwap.Name = "BtnSwap";
            this.BtnSwap.Size = new System.Drawing.Size(101, 23);
            this.BtnSwap.TabIndex = 9;
            this.BtnSwap.Text = "Show classes";
            this.BtnSwap.UseVisualStyleBackColor = true;
            // 
            // cmbGrades
            // 
            this.cmbGrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbGrades.FormattingEnabled = true;
            this.cmbGrades.Location = new System.Drawing.Point(643, 92);
            this.cmbGrades.Name = "cmbGrades";
            this.cmbGrades.Size = new System.Drawing.Size(175, 28);
            this.cmbGrades.TabIndex = 8;
            this.cmbGrades.SelectedIndexChanged += new System.EventHandler(this.CmbGrades_SelectedIndexChanged);
            // 
            // dgvClasses
            // 
            this.dgvClasses.AllowUserToAddRows = false;
            this.dgvClasses.AllowUserToDeleteRows = false;
            this.dgvClasses.BackgroundColor = System.Drawing.Color.White;
            this.dgvClasses.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvClasses.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dgvClasses.Location = new System.Drawing.Point(0, 126);
            this.dgvClasses.Name = "dgvClasses";
            this.dgvClasses.ReadOnly = true;
            this.dgvClasses.Size = new System.Drawing.Size(830, 431);
            this.dgvClasses.TabIndex = 7;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(365, 92);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 10;
            this.btnGenerate.Text = "button1";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // ViewTimeTable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 557);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.BtnSwap);
            this.Controls.Add(this.cmbGrades);
            this.Controls.Add(this.dgvClasses);
            this.Name = "ViewTimeTable";
            this.Text = "ViewTimeTable";
            ((System.ComponentModel.ISupportInitialize)(this.dgvClasses)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSwap;
        private System.Windows.Forms.ComboBox cmbGrades;
        private System.Windows.Forms.DataGridView dgvClasses;
        private System.Windows.Forms.Button btnGenerate;
    }
}