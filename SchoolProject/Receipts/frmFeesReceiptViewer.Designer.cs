namespace SchoolProject.Receipts
{
    partial class frmFeesReceiptViewer
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
            this.btnClick = new System.Windows.Forms.Button();
            this.btnPrintIt = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClick
            // 
            this.btnClick.Location = new System.Drawing.Point(516, 528);
            this.btnClick.Name = "btnClick";
            this.btnClick.Size = new System.Drawing.Size(93, 33);
            this.btnClick.TabIndex = 0;
            this.btnClick.Text = "Save as file";
            this.btnClick.UseVisualStyleBackColor = true;
            this.btnClick.Click += new System.EventHandler(this.btnClick_Click);
            // 
            // btnPrintIt
            // 
            this.btnPrintIt.Location = new System.Drawing.Point(422, 528);
            this.btnPrintIt.Name = "btnPrintIt";
            this.btnPrintIt.Size = new System.Drawing.Size(88, 33);
            this.btnPrintIt.TabIndex = 1;
            this.btnPrintIt.Text = "button1";
            this.btnPrintIt.UseVisualStyleBackColor = true;
            this.btnPrintIt.Click += new System.EventHandler(this.btnPrintIt_Click);
            // 
            // frmFeesReceiptViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(630, 589);
            this.Controls.Add(this.btnPrintIt);
            this.Controls.Add(this.btnClick);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmFeesReceiptViewer";
            this.Text = "frmFeesReceiptViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClick;
        private System.Windows.Forms.Button btnPrintIt;
    }
}