namespace SchoolProject.Attendance
{
    partial class frmEmployeesPayroll
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tcEnrollHistory = new System.Windows.Forms.TabControl();
            this.tpTeachersSalary = new System.Windows.Forms.TabPage();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvPayroll = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpPayrollDate = new System.Windows.Forms.DateTimePicker();
            this.button1 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tcEnrollHistory.SuspendLayout();
            this.tpTeachersSalary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayroll)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tcEnrollHistory);
            this.groupBox1.Location = new System.Drawing.Point(6, 82);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(853, 464);
            this.groupBox1.TabIndex = 141;
            this.groupBox1.TabStop = false;
            // 
            // tcEnrollHistory
            // 
            this.tcEnrollHistory.Controls.Add(this.tpTeachersSalary);
            this.tcEnrollHistory.Location = new System.Drawing.Point(12, 19);
            this.tcEnrollHistory.Name = "tcEnrollHistory";
            this.tcEnrollHistory.SelectedIndex = 0;
            this.tcEnrollHistory.Size = new System.Drawing.Size(835, 435);
            this.tcEnrollHistory.TabIndex = 131;
            // 
            // tpTeachersSalary
            // 
            this.tpTeachersSalary.Controls.Add(this.lblRecordCount);
            this.tpTeachersSalary.Controls.Add(this.label2);
            this.tpTeachersSalary.Controls.Add(this.dgvPayroll);
            this.tpTeachersSalary.Location = new System.Drawing.Point(4, 22);
            this.tpTeachersSalary.Name = "tpTeachersSalary";
            this.tpTeachersSalary.Padding = new System.Windows.Forms.Padding(3);
            this.tpTeachersSalary.Size = new System.Drawing.Size(827, 409);
            this.tpTeachersSalary.TabIndex = 0;
            this.tpTeachersSalary.UseVisualStyleBackColor = true;
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.AutoSize = true;
            this.lblRecordCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecordCount.Location = new System.Drawing.Point(94, 388);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(24, 18);
            this.lblRecordCount.TabIndex = 134;
            this.lblRecordCount.Text = "??";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 384);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 20);
            this.label2.TabIndex = 133;
            this.label2.Text = "# Records:";
            // 
            // dgvPayroll
            // 
            this.dgvPayroll.AllowUserToAddRows = false;
            this.dgvPayroll.AllowUserToDeleteRows = false;
            this.dgvPayroll.AllowUserToResizeRows = false;
            this.dgvPayroll.BackgroundColor = System.Drawing.Color.White;
            this.dgvPayroll.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPayroll.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvPayroll.Location = new System.Drawing.Point(7, 34);
            this.dgvPayroll.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dgvPayroll.MultiSelect = false;
            this.dgvPayroll.Name = "dgvPayroll";
            this.dgvPayroll.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPayroll.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvPayroll.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPayroll.Size = new System.Drawing.Size(804, 345);
            this.dgvPayroll.TabIndex = 132;
            this.dgvPayroll.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(22, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 20);
            this.label4.TabIndex = 136;
            this.label4.Text = "Payroll as of";
            // 
            // dtpPayrollDate
            // 
            this.dtpPayrollDate.CustomFormat = "yyyy-mm";
            this.dtpPayrollDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpPayrollDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpPayrollDate.Location = new System.Drawing.Point(135, 52);
            this.dtpPayrollDate.Name = "dtpPayrollDate";
            this.dtpPayrollDate.Size = new System.Drawing.Size(110, 24);
            this.dtpPayrollDate.TabIndex = 142;
            this.dtpPayrollDate.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(717, 556);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 23);
            this.button1.TabIndex = 143;
            this.button1.Text = "Pay for Employees";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(261, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(296, 37);
            this.label3.TabIndex = 144;
            this.label3.Text = "Employees Payroll";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmEmployeesPayroll
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(862, 588);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dtpPayrollDate);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmEmployeesPayroll";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmEmployeesPayroll";
            this.Load += new System.EventHandler(this.frmEmployeesPayroll_Load);
            this.groupBox1.ResumeLayout(false);
            this.tcEnrollHistory.ResumeLayout(false);
            this.tpTeachersSalary.ResumeLayout(false);
            this.tpTeachersSalary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayroll)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpPayrollDate;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tcEnrollHistory;
        private System.Windows.Forms.TabPage tpTeachersSalary;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvPayroll;
        private System.Windows.Forms.Label label3;
    }
}