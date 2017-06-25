namespace IfPatcher
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TC_PatchMode = new System.Windows.Forms.TabControl();
            this.T_Game = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.TB_GamePath = new System.Windows.Forms.TextBox();
            this.B_BrowseGame = new System.Windows.Forms.Button();
            this.TB_Progress = new System.Windows.Forms.TextBox();
            this.PB_PatchProgress = new System.Windows.Forms.ProgressBar();
            this.B_ApplyPatch = new System.Windows.Forms.Button();
            this.T_DLC = new System.Windows.Forms.TabPage();
            this.PB_DLCProgress = new System.Windows.Forms.ProgressBar();
            this.B_BrowseDLC = new System.Windows.Forms.Button();
            this.FLP_Titles = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.TB_ContentPath = new System.Windows.Forms.TextBox();
            this.B_PatchDLC = new System.Windows.Forms.Button();
            this.TB_PatchPath = new System.Windows.Forms.TextBox();
            this.B_BrowsePatch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.TC_PatchMode.SuspendLayout();
            this.T_Game.SuspendLayout();
            this.T_DLC.SuspendLayout();
            this.SuspendLayout();
            // 
            // TC_PatchMode
            // 
            this.TC_PatchMode.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TC_PatchMode.Controls.Add(this.T_Game);
            this.TC_PatchMode.Controls.Add(this.T_DLC);
            this.TC_PatchMode.Location = new System.Drawing.Point(12, 51);
            this.TC_PatchMode.Multiline = true;
            this.TC_PatchMode.Name = "TC_PatchMode";
            this.TC_PatchMode.SelectedIndex = 0;
            this.TC_PatchMode.Size = new System.Drawing.Size(410, 348);
            this.TC_PatchMode.TabIndex = 0;
            // 
            // T_Game
            // 
            this.T_Game.Controls.Add(this.label1);
            this.T_Game.Controls.Add(this.TB_GamePath);
            this.T_Game.Controls.Add(this.B_BrowseGame);
            this.T_Game.Controls.Add(this.TB_Progress);
            this.T_Game.Controls.Add(this.PB_PatchProgress);
            this.T_Game.Controls.Add(this.B_ApplyPatch);
            this.T_Game.Location = new System.Drawing.Point(4, 22);
            this.T_Game.Name = "T_Game";
            this.T_Game.Padding = new System.Windows.Forms.Padding(3);
            this.T_Game.Size = new System.Drawing.Size(402, 322);
            this.T_Game.TabIndex = 0;
            this.T_Game.Text = "Game";
            this.T_Game.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Decrypted Game CIA:";
            // 
            // TB_GamePath
            // 
            this.TB_GamePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_GamePath.Location = new System.Drawing.Point(6, 19);
            this.TB_GamePath.Name = "TB_GamePath";
            this.TB_GamePath.Size = new System.Drawing.Size(358, 20);
            this.TB_GamePath.TabIndex = 11;
            this.TB_GamePath.TextChanged += new System.EventHandler(this.TB_TextChanged);
            // 
            // B_BrowseGame
            // 
            this.B_BrowseGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_BrowseGame.AutoSize = true;
            this.B_BrowseGame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.B_BrowseGame.Location = new System.Drawing.Point(370, 17);
            this.B_BrowseGame.Name = "B_BrowseGame";
            this.B_BrowseGame.Size = new System.Drawing.Size(26, 23);
            this.B_BrowseGame.TabIndex = 10;
            this.B_BrowseGame.Text = "...";
            this.B_BrowseGame.UseVisualStyleBackColor = true;
            this.B_BrowseGame.Click += new System.EventHandler(this.B_BrowseGame_Click);
            // 
            // TB_Progress
            // 
            this.TB_Progress.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_Progress.Location = new System.Drawing.Point(8, 74);
            this.TB_Progress.Multiline = true;
            this.TB_Progress.Name = "TB_Progress";
            this.TB_Progress.ReadOnly = true;
            this.TB_Progress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TB_Progress.Size = new System.Drawing.Size(388, 242);
            this.TB_Progress.TabIndex = 7;
            // 
            // PB_PatchProgress
            // 
            this.PB_PatchProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_PatchProgress.Location = new System.Drawing.Point(6, 45);
            this.PB_PatchProgress.Name = "PB_PatchProgress";
            this.PB_PatchProgress.Size = new System.Drawing.Size(304, 23);
            this.PB_PatchProgress.TabIndex = 6;
            // 
            // B_ApplyPatch
            // 
            this.B_ApplyPatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_ApplyPatch.Location = new System.Drawing.Point(316, 45);
            this.B_ApplyPatch.Name = "B_ApplyPatch";
            this.B_ApplyPatch.Size = new System.Drawing.Size(80, 23);
            this.B_ApplyPatch.TabIndex = 5;
            this.B_ApplyPatch.Text = "Apply Patch";
            this.B_ApplyPatch.UseVisualStyleBackColor = true;
            this.B_ApplyPatch.Click += new System.EventHandler(this.B_ApplyPatch_Click);
            // 
            // T_DLC
            // 
            this.T_DLC.Controls.Add(this.PB_DLCProgress);
            this.T_DLC.Controls.Add(this.B_BrowseDLC);
            this.T_DLC.Controls.Add(this.FLP_Titles);
            this.T_DLC.Controls.Add(this.label3);
            this.T_DLC.Controls.Add(this.TB_ContentPath);
            this.T_DLC.Controls.Add(this.B_PatchDLC);
            this.T_DLC.Location = new System.Drawing.Point(4, 22);
            this.T_DLC.Name = "T_DLC";
            this.T_DLC.Padding = new System.Windows.Forms.Padding(3);
            this.T_DLC.Size = new System.Drawing.Size(402, 322);
            this.T_DLC.TabIndex = 1;
            this.T_DLC.Text = "DLC";
            this.T_DLC.UseVisualStyleBackColor = true;
            // 
            // PB_DLCProgress
            // 
            this.PB_DLCProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_DLCProgress.Location = new System.Drawing.Point(6, 45);
            this.PB_DLCProgress.Name = "PB_DLCProgress";
            this.PB_DLCProgress.Size = new System.Drawing.Size(304, 23);
            this.PB_DLCProgress.TabIndex = 18;
            // 
            // B_BrowseDLC
            // 
            this.B_BrowseDLC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_BrowseDLC.AutoSize = true;
            this.B_BrowseDLC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.B_BrowseDLC.Location = new System.Drawing.Point(370, 17);
            this.B_BrowseDLC.Name = "B_BrowseDLC";
            this.B_BrowseDLC.Size = new System.Drawing.Size(26, 23);
            this.B_BrowseDLC.TabIndex = 17;
            this.B_BrowseDLC.Text = "...";
            this.B_BrowseDLC.UseVisualStyleBackColor = true;
            this.B_BrowseDLC.Click += new System.EventHandler(this.B_BrowseGame_Click);
            // 
            // FLP_Titles
            // 
            this.FLP_Titles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FLP_Titles.AutoScroll = true;
            this.FLP_Titles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FLP_Titles.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FLP_Titles.Location = new System.Drawing.Point(6, 74);
            this.FLP_Titles.Name = "FLP_Titles";
            this.FLP_Titles.Size = new System.Drawing.Size(390, 242);
            this.FLP_Titles.TabIndex = 16;
            this.FLP_Titles.WrapContents = false;
            this.FLP_Titles.SizeChanged += new System.EventHandler(this.FLP_Titles_SizeChanged);
            this.FLP_Titles.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.FLP_Titles_ControlAdded);
            this.FLP_Titles.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.FLP_Titles_ControlAdded);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Decrypted DLC CIA:";
            // 
            // TB_ContentPath
            // 
            this.TB_ContentPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_ContentPath.Location = new System.Drawing.Point(6, 19);
            this.TB_ContentPath.Name = "TB_ContentPath";
            this.TB_ContentPath.Size = new System.Drawing.Size(358, 20);
            this.TB_ContentPath.TabIndex = 14;
            this.TB_ContentPath.TextChanged += new System.EventHandler(this.TB_TextChanged);
            // 
            // B_PatchDLC
            // 
            this.B_PatchDLC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_PatchDLC.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.B_PatchDLC.Location = new System.Drawing.Point(316, 45);
            this.B_PatchDLC.Name = "B_PatchDLC";
            this.B_PatchDLC.Size = new System.Drawing.Size(80, 23);
            this.B_PatchDLC.TabIndex = 13;
            this.B_PatchDLC.Text = "Unpack";
            this.B_PatchDLC.UseVisualStyleBackColor = true;
            this.B_PatchDLC.Click += new System.EventHandler(this.B_PatchDLC_Click);
            // 
            // TB_PatchPath
            // 
            this.TB_PatchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TB_PatchPath.Location = new System.Drawing.Point(12, 25);
            this.TB_PatchPath.Name = "TB_PatchPath";
            this.TB_PatchPath.Size = new System.Drawing.Size(378, 20);
            this.TB_PatchPath.TabIndex = 11;
            this.TB_PatchPath.Text = "Patch folder contains the \"exe\", \"rom\", and \"dlc\" directories.";
            this.TB_PatchPath.TextChanged += new System.EventHandler(this.TB_TextChanged);
            // 
            // B_BrowsePatch
            // 
            this.B_BrowsePatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.B_BrowsePatch.AutoSize = true;
            this.B_BrowsePatch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.B_BrowsePatch.Location = new System.Drawing.Point(396, 22);
            this.B_BrowsePatch.Name = "B_BrowsePatch";
            this.B_BrowsePatch.Size = new System.Drawing.Size(26, 23);
            this.B_BrowsePatch.TabIndex = 10;
            this.B_BrowsePatch.Text = "...";
            this.B_BrowsePatch.UseVisualStyleBackColor = true;
            this.B_BrowsePatch.Click += new System.EventHandler(this.B_BrowsePatch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Patch Folder:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 411);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TB_PatchPath);
            this.Controls.Add(this.B_BrowsePatch);
            this.Controls.Add(this.TC_PatchMode);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Fire Emblem If Patcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.TC_PatchMode.ResumeLayout(false);
            this.T_Game.ResumeLayout(false);
            this.T_Game.PerformLayout();
            this.T_DLC.ResumeLayout(false);
            this.T_DLC.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl TC_PatchMode;
        private System.Windows.Forms.TabPage T_Game;
        private System.Windows.Forms.TextBox TB_GamePath;
        private System.Windows.Forms.Button B_BrowseGame;
        private System.Windows.Forms.TextBox TB_Progress;
        private System.Windows.Forms.ProgressBar PB_PatchProgress;
        private System.Windows.Forms.Button B_ApplyPatch;
        private System.Windows.Forms.TabPage T_DLC;
        private System.Windows.Forms.TextBox TB_PatchPath;
        private System.Windows.Forms.Button B_BrowsePatch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TB_ContentPath;
        private System.Windows.Forms.Button B_PatchDLC;
        private System.Windows.Forms.FlowLayoutPanel FLP_Titles;
        private System.Windows.Forms.ProgressBar PB_DLCProgress;
        private System.Windows.Forms.Button B_BrowseDLC;
    }
}

