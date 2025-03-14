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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallationForm));
            this.installBtn = new System.Windows.Forms.Button();
            this.uninstallBtn = new System.Windows.Forms.Button();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.featureBox = new System.Windows.Forms.CheckedListBox();
            this.themeGroup = new System.Windows.Forms.GroupBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.cornersGroup = new System.Windows.Forms.GroupBox();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.searchRestartBtn = new System.Windows.Forms.Button();
            this.optionsBtnTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.optionsBtn = new System.Windows.Forms.Button();
            this.optionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.optionsMenu_ClearIconCache = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu_EditTopApps = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu_EditSearchBoxText = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu_ToggleFileExplorerClassicSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenu_SearchBoxTheme = new System.Windows.Forms.ToolStripMenuItem();
            this.themeGroup.SuspendLayout();
            this.cornersGroup.SuspendLayout();
            this.optionsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // installBtn
            // 
            this.installBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.installBtn.Location = new System.Drawing.Point(11, 552);
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
            this.uninstallBtn.Location = new System.Drawing.Point(323, 552);
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
            this.copyrightLabel.Location = new System.Drawing.Point(166, 562);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(139, 17);
            this.copyrightLabel.TabIndex = 5;
            this.copyrightLabel.Text = "(c) krlvm, 2020-2025";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(10, 523);
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
            this.label2.Location = new System.Drawing.Point(10, 489);
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
            // themeGroup
            // 
            this.themeGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.themeGroup.Controls.Add(this.radioButton3);
            this.themeGroup.Controls.Add(this.radioButton2);
            this.themeGroup.Controls.Add(this.radioButton1);
            this.themeGroup.Location = new System.Drawing.Point(12, 383);
            this.themeGroup.Name = "themeGroup";
            this.themeGroup.Size = new System.Drawing.Size(200, 100);
            this.themeGroup.TabIndex = 16;
            this.themeGroup.TabStop = false;
            this.themeGroup.Text = "Theme";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(6, 73);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(59, 21);
            this.radioButton3.TabIndex = 2;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Dark";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(6, 46);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(60, 21);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "Light";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 21);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(58, 21);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Auto";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // cornersGroup
            // 
            this.cornersGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cornersGroup.Controls.Add(this.radioButton4);
            this.cornersGroup.Controls.Add(this.radioButton5);
            this.cornersGroup.Controls.Add(this.radioButton6);
            this.cornersGroup.Location = new System.Drawing.Point(260, 383);
            this.cornersGroup.Name = "cornersGroup";
            this.cornersGroup.Size = new System.Drawing.Size(200, 100);
            this.cornersGroup.TabIndex = 17;
            this.cornersGroup.TabStop = false;
            this.cornersGroup.Text = "Corners";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(6, 73);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(87, 21);
            this.radioButton4.TabIndex = 2;
            this.radioButton4.Text = "Rounded";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Checked = true;
            this.radioButton5.Location = new System.Drawing.Point(6, 46);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(67, 21);
            this.radioButton5.TabIndex = 1;
            this.radioButton5.TabStop = true;
            this.radioButton5.Text = "Sharp";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(6, 21);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(74, 21);
            this.radioButton6.TabIndex = 0;
            this.radioButton6.Text = "Default";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // searchRestartBtn
            // 
            this.searchRestartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.searchRestartBtn.Font = new System.Drawing.Font("Segoe MDL2 Assets", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchRestartBtn.Location = new System.Drawing.Point(323, 489);
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
            this.optionsBtn.Location = new System.Drawing.Point(392, 489);
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
            this.optionsMenu_ClearIconCache,
            this.optionsMenu_EditTopApps,
            this.optionsMenu_EditSearchBoxText,
            this.optionsMenu_ToggleFileExplorerClassicSearch,
            this.optionsMenu_SearchBoxTheme});
            this.optionsMenu.Name = "optionsMenu";
            this.optionsMenu.Size = new System.Drawing.Size(309, 124);
            // 
            // optionsMenu_ClearIconCache
            // 
            this.optionsMenu_ClearIconCache.Name = "optionsMenu_ClearIconCache";
            this.optionsMenu_ClearIconCache.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_ClearIconCache.Text = "Clear Icon Cache";
            this.optionsMenu_ClearIconCache.Click += new System.EventHandler(this.optionsMenu_ClearIconCache_Click);
            // 
            // optionsMenu_EditTopApps
            // 
            this.optionsMenu_EditTopApps.Name = "optionsMenu_EditTopApps";
            this.optionsMenu_EditTopApps.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_EditTopApps.Text = "Customize \"Top Apps\"";
            this.optionsMenu_EditTopApps.Click += new System.EventHandler(this.optionsMenu_EditTopApps_Click);
            // 
            // optionsMenu_EditSearchBoxText
            // 
            this.optionsMenu_EditSearchBoxText.Name = "optionsMenu_EditSearchBoxText";
            this.optionsMenu_EditSearchBoxText.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_EditSearchBoxText.Text = "Edit Search Box Text";
            this.optionsMenu_EditSearchBoxText.Click += new System.EventHandler(this.optionsMenu_EditSearchBoxText_Click);
            // 
            // optionsMenu_ToggleFileExplorerClassicSearch
            // 
            this.optionsMenu_ToggleFileExplorerClassicSearch.Name = "optionsMenu_ToggleFileExplorerClassicSearch";
            this.optionsMenu_ToggleFileExplorerClassicSearch.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_ToggleFileExplorerClassicSearch.Text = "Restore classic File Explorer Search";
            this.optionsMenu_ToggleFileExplorerClassicSearch.Click += new System.EventHandler(this.optionsMenu_ToggleFileExplorerClassicSearch_Click);
            // 
            // optionsMenu_SearchBoxTheme
            // 
            this.optionsMenu_SearchBoxTheme.CheckOnClick = true;
            this.optionsMenu_SearchBoxTheme.Name = "optionsMenu_SearchBoxTheme";
            this.optionsMenu_SearchBoxTheme.Size = new System.Drawing.Size(308, 24);
            this.optionsMenu_SearchBoxTheme.Text = "Enforce Dark Search Box";
            this.optionsMenu_SearchBoxTheme.CheckedChanged += new System.EventHandler(this.optionsMenu_SearchBoxTheme_CheckedChanged);
            // 
            // InstallationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(472, 602);
            this.Controls.Add(this.optionsBtn);
            this.Controls.Add(this.searchRestartBtn);
            this.Controls.Add(this.cornersGroup);
            this.Controls.Add(this.themeGroup);
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
            this.Name = "InstallationForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BeautySearch Installer";
            this.Load += new System.EventHandler(this.InstallationForm_Load);
            this.themeGroup.ResumeLayout(false);
            this.themeGroup.PerformLayout();
            this.cornersGroup.ResumeLayout(false);
            this.cornersGroup.PerformLayout();
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
        private System.Windows.Forms.GroupBox themeGroup;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox cornersGroup;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.Button searchRestartBtn;
        private System.Windows.Forms.ToolTip optionsBtnTooltip;
        private System.Windows.Forms.Button optionsBtn;
        private System.Windows.Forms.ContextMenuStrip optionsMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_ClearIconCache;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_EditTopApps;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_EditSearchBoxText;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_ToggleFileExplorerClassicSearch;
        private System.Windows.Forms.ToolStripMenuItem optionsMenu_SearchBoxTheme;
    }
}

