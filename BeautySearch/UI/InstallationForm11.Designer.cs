namespace BeautySearch
{
    partial class InstallationForm11
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationForm11));
            this.installBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.featureBox = new System.Windows.Forms.CheckedListBox();
            this.searchRestartBtn = new System.Windows.Forms.Button();
            this.optionsBtnTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.optionsBtn = new System.Windows.Forms.Button();
            this.optionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.optionsMenu_ToggleFileExplorerClassicSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // installBtn
            // 
            this.installBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.installBtn.Location = new System.Drawing.Point(11, 450);
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
            this.uninstallBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.uninstallBtn.Location = new System.Drawing.Point(323, 450);
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
            this.copyrightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.copyrightLabel.Location = new System.Drawing.Point(166, 460);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(139, 17);
            this.copyrightLabel.TabIndex = 5;
            this.copyrightLabel.Text = "(c) krlvm, 2020-2022";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(10, 421);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(250, 17);
            this.linkLabel1.TabIndex = 12;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://github.com/krlvm/BeautySearch";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 387);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 34);
            this.label2.TabIndex = 13;
            this.label2.Text = "Licensed under the GNU GPLv3 License\r\nVisit GitHub Repository for instructions an" +
    "d help";
            // 
            // featureBox
            // 
            this.featureBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.featureBox.CheckOnClick = true;
            this.featureBox.FormattingEnabled = true;
            this.featureBox.Location = new System.Drawing.Point(12, 11);
            this.featureBox.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.featureBox.Name = "featureBox";
            this.featureBox.Size = new System.Drawing.Size(450, 361);
            this.featureBox.TabIndex = 15;
            // 
            // searchRestartBtn
            // 
            this.searchRestartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.searchRestartBtn.Font = new System.Drawing.Font("Segoe MDL2 Assets", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchRestartBtn.Location = new System.Drawing.Point(323, 387);
            this.searchRestartBtn.Name = "searchRestartBtn";
            this.searchRestartBtn.Size = new System.Drawing.Size(67, 57);
            this.searchRestartBtn.TabIndex = 18;
            this.searchRestartBtn.Text = "";
            this.optionsBtnTooltip.SetToolTip(this.searchRestartBtn, "Restart Search App");
            this.searchRestartBtn.UseVisualStyleBackColor = true;
            this.searchRestartBtn.Click += new System.EventHandler(this.searchRestartBtn_Click);
            // 
            // optionsBtn
            // 
            this.optionsBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsBtn.Font = new System.Drawing.Font("Segoe MDL2 Assets", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.optionsBtn.Location = new System.Drawing.Point(392, 387);
            this.optionsBtn.Name = "optionsBtn";
            this.optionsBtn.Size = new System.Drawing.Size(67, 57);
            this.optionsBtn.TabIndex = 20;
            this.optionsBtn.Text = "";
            this.optionsBtnTooltip.SetToolTip(this.optionsBtn, "Options");
            this.optionsBtn.UseVisualStyleBackColor = true;
            this.optionsBtn.Click += new System.EventHandler(this.optionsBtn_Click);
            // 
            // optionsMenu
            // 
            this.optionsMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.optionsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsMenu_ToggleFileExplorerClassicSearch});
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.Size = new System.Drawing.Size(309, 28);
            // 
            // optionsMenu_ToggleFileExplorerClassicSearch
            // 
            this.optionsMenu_ToggleFileExplorerClassicSearch.Name = "optionsMenu_ToggleFileExplorerClassicSearch";
            this.optionsMenu_ToggleFileExplorerClassicSearch.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_ToggleFileExplorerClassicSearch.Text = "Restore classic File Explorer Search";
            this.optionsMenu_ToggleFileExplorerClassicSearch.Click += new System.EventHandler(this.optionsMenu_ToggleFileExplorerClassicSearch_Click);
            // 
            // InstallationForm11
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(472, 500);
            this.Controls.Add(this.optionsBtn);
            this.Controls.Add(this.searchRestartBtn);
            this.Controls.Add(this.featureBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.copyrightLabel);
            this.Controls.Add(this.uninstallBtn);
            this.Controls.Add(this.installBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallationForm11";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeautySearch Installer";
            this.Load += new System.EventHandler(this.InstallationForm11_Load);
            this.optionsMenu.ResumeLayout(false);
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
        private System.Windows.Forms.Button searchRestartBtn;
        private System.Windows.Forms.ToolTip optionsBtnTooltip;
        private System.Windows.Forms.Button optionsBtn;
        private System.Windows.Forms.ContextMenuStrip optionsMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_ToggleFileExplorerClassicSearch;
    }
}

