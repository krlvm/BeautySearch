namespace BeautySearch
{
    partial class InstallationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationForm));
            this.installBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.featureBox = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // installBtn
            // 
            this.installBtn.Location = new System.Drawing.Point(15, 407);
            this.installBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.installBtn.Name = "installBtn";
            this.installBtn.Size = new System.Drawing.Size(136, 36);
            this.installBtn.TabIndex = 0;
            this.installBtn.Text = "Install";
            this.installBtn.UseVisualStyleBackColor = true;
            this.installBtn.Click += new System.EventHandler(this.installBtn_Click);
            // 
            // uninstallBtn
            // 
            this.uninstallBtn.Location = new System.Drawing.Point(324, 407);
            this.uninstallBtn.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.uninstallBtn.Name = "uninstallBtn";
            this.uninstallBtn.Size = new System.Drawing.Size(136, 36);
            this.uninstallBtn.TabIndex = 1;
            this.uninstallBtn.Text = "Uninstall";
            this.uninstallBtn.UseVisualStyleBackColor = true;
            this.uninstallBtn.Click += new System.EventHandler(this.uninstallBtn_Click);
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.copyrightLabel.Location = new System.Drawing.Point(167, 417);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(139, 17);
            this.copyrightLabel.TabIndex = 5;
            this.copyrightLabel.Text = "(c) krlvm, 2020-2021";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(12, 377);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(250, 17);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/krlvm/BeautySearch";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 339);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 34);
            this.label2.TabIndex = 13;
            this.label2.Text = "Licensed under the GNU GPLv3 License\r\nVisit GitHub Repository for instructions an" +
    "d help";
            // 
            // featureBox
            // 
            this.featureBox.CheckOnClick = true;
            this.featureBox.FormattingEnabled = true;
            this.featureBox.Location = new System.Drawing.Point(12, 45);
            this.featureBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.featureBox.Name = "featureBox";
            this.featureBox.Size = new System.Drawing.Size(448, 276);
            this.featureBox.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(182, 23);
            this.label1.TabIndex = 16;
            this.label1.Text = "Select tweaks to apply:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InstallationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(472, 455);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.featureBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.uninstallBtn);
            this.Controls.Add(this.installBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "InstallationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeautySearch Installer";
            this.Load += new System.EventHandler(this.InstallationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button installBtn;
        private System.Windows.Forms.Button uninstallBtn;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckedListBox featureBox;
        private System.Windows.Forms.Label label1;
    }
}

