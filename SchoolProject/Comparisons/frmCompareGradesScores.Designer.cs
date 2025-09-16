namespace SchoolProject.Comparisons
{
    partial class frmCompareGradesScores
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chartClasses = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxGrade = new System.Windows.Forms.ComboBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.comboBoxTerm = new System.Windows.Forms.ComboBox();
            this.lblLowestGrade = new System.Windows.Forms.Label();
            this.lblHighestGrade = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chartClasses)).BeginInit();
            this.SuspendLayout();
            // 
            // chartClasses
            // 
            chartArea6.Name = "ChartArea1";
            this.chartClasses.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chartClasses.Legends.Add(legend6);
            this.chartClasses.Location = new System.Drawing.Point(2, 113);
            this.chartClasses.Name = "chartClasses";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chartClasses.Series.Add(series6);
            this.chartClasses.Size = new System.Drawing.Size(851, 397);
            this.chartClasses.TabIndex = 0;
            this.chartClasses.Text = "chart1";
            // 
            // comboBoxGrade
            // 
            this.comboBoxGrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxGrade.FormattingEnabled = true;
            this.comboBoxGrade.Location = new System.Drawing.Point(10, 79);
            this.comboBoxGrade.Name = "comboBoxGrade";
            this.comboBoxGrade.Size = new System.Drawing.Size(163, 28);
            this.comboBoxGrade.TabIndex = 1;
            this.comboBoxGrade.SelectedIndexChanged += new System.EventHandler(this.comboBoxGrade_SelectedIndexChanged);
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTitle.Location = new System.Drawing.Point(58, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(754, 45);
            this.lblTitle.TabIndex = 121;
            this.lblTitle.Text = "Compare Grades";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // comboBoxTerm
            // 
            this.comboBoxTerm.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxTerm.FormattingEnabled = true;
            this.comboBoxTerm.Location = new System.Drawing.Point(179, 79);
            this.comboBoxTerm.Name = "comboBoxTerm";
            this.comboBoxTerm.Size = new System.Drawing.Size(163, 28);
            this.comboBoxTerm.TabIndex = 122;
            this.comboBoxTerm.SelectedIndexChanged += new System.EventHandler(this.comboBoxTerm_SelectedIndexChanged);
            // 
            // lblLowestGrade
            // 
            this.lblLowestGrade.AutoSize = true;
            this.lblLowestGrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLowestGrade.Location = new System.Drawing.Point(659, 83);
            this.lblLowestGrade.Name = "lblLowestGrade";
            this.lblLowestGrade.Size = new System.Drawing.Size(88, 18);
            this.lblLowestGrade.TabIndex = 124;
            this.lblLowestGrade.Text = "Lowest: N/A";
            // 
            // lblHighestGrade
            // 
            this.lblHighestGrade.AutoSize = true;
            this.lblHighestGrade.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHighestGrade.Location = new System.Drawing.Point(659, 61);
            this.lblHighestGrade.Name = "lblHighestGrade";
            this.lblHighestGrade.Size = new System.Drawing.Size(90, 18);
            this.lblHighestGrade.TabIndex = 125;
            this.lblHighestGrade.Text = "Highest: N/A";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Image = global::SchoolProject.Properties.Resources.close_button_15600737__2_1;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(733, 518);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 37);
            this.button1.TabIndex = 127;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmCompareGradesScores
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(855, 565);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblHighestGrade);
            this.Controls.Add(this.lblLowestGrade);
            this.Controls.Add(this.comboBoxTerm);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.comboBoxGrade);
            this.Controls.Add(this.chartClasses);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmCompareGradesScores";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmCompareGradesScores";
            this.Load += new System.EventHandler(this.frmCompareGradesScores_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartClasses)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chartClasses;
        private System.Windows.Forms.ComboBox comboBoxGrade;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox comboBoxTerm;
        private System.Windows.Forms.Label lblLowestGrade;
        private System.Windows.Forms.Label lblHighestGrade;
        private System.Windows.Forms.Button button1;
    }
}