using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IfPatcher.Controller;
using IfPatcher.Model;

namespace IfPatcher
{
    public partial class MainForm : Form, IMain, IDLCView
    {
        private MainController cont;

        #region Properties

        public string PatchFolderPath
        {
            get { return TB_PatchPath.Text; }
            set { TB_PatchPath.Text = value; }
        }

        public string GameFilePath
        {
            get { return TB_GamePath.Text; }
            set { TB_GamePath.Text = value; }
        }

        public string ContentFilePath
        {
            get { return TB_ContentPath.Text; }
            set { TB_ContentPath.Text = value; }
        }

        public int PercentageComplete
        {
            get { return PB_PatchProgress.Value; }
            set { PB_PatchProgress.Value = value; }
        }

        public int TitlePercentageComplete
        {
            get { return PB_DLCProgress.Value; }
            set { PB_DLCProgress.Value = value; }
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        public void SetController(MainController c)
        {
            if(cont == null)
                cont = c;
        }

        private Panel GenerateListEntry(string tName, string tIndexID, bool tPatchable, bool tWillPatch, Image tIcon)
        {
            //Create the panel
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(382, 58)
            };

            //DLC name
            var titleName = new Label
            {
                AutoSize = true,
                Font = new Font("Microsoft Sans Serif", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0),
                Location = new Point(3, 3),
                Size = new Size(215, 18),
                Text = tName
            };

            //Index and ID
            var indexID = new Label
            {
                AutoSize = true,
                Location = new Point(3, 21),
                Size = new Size(82, 13),
                Text = tIndexID
            };

            //Patchable?
            var patchable = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TextAlign = ContentAlignment.TopRight,
                Location = new Point(221, 3),
                Size = new Size(100, 13),
                ForeColor = tPatchable ? Color.Green : Color.Red,
                Text = tPatchable ? "Patch available" : "No patch available"
            };

            //Patch checkmark
            var patchCheck = new CheckBox
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                AutoCheck = true,
                CheckAlign = ContentAlignment.MiddleRight,
                Checked = tWillPatch,
                CheckState = tWillPatch ? CheckState.Checked : CheckState.Unchecked,
                Location = new Point(267, 19),
                Size = new Size(54, 17),
                Text = "Patch",
                UseVisualStyleBackColor = true,
                Visible = tPatchable
            };

            //Icon
            var icon = new PictureBox
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.White,
                Location = new Point(327, 3),
                Size = new Size(50, 50),
                Image = tIcon
            };

            //Progress bar
            var progress = new ProgressBar
            {
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Location = new Point(3, 37),
                Size = new Size(318, 16)
            };

            //Add to panel
            panel.Controls.Add(titleName);
            panel.Controls.Add(indexID);
            panel.Controls.Add(patchable);
            panel.Controls.Add(patchCheck);
            panel.Controls.Add(icon);
            panel.Controls.Add(progress);

            //Solves FLP focus issues
            panel.Click += Panel_Click;
            titleName.Click += Panel_Click;
            indexID.Click += Panel_Click;
            patchable.Click += Panel_Click;
            patchCheck.Click += Panel_Click;
            icon.Click += Panel_Click;
            progress.Click += Panel_Click;

            return panel;
        }

        public void AppendLog(string message)
        {
            TB_Progress.Invoke(new Action(() => TB_Progress.AppendText(message)));
        }

        public void PopulateContentList(List<ContentData> titles)
        {
            for (int i = FLP_Titles.Controls.Count - 1; i >= 0; i--)
                FLP_Titles.Controls[i].Dispose();

            foreach (ContentData t in titles)
            {
                Panel newPanel = GenerateListEntry(t.Name, t.Index + '.' + t.ID, t.Patchable, t.WillPatch, t.Icon);
                FLP_Titles.Controls.Add(newPanel);
            }
        }

        public void SetContentProgress(int index, int progress)
        {
            var panel = (Panel) FLP_Titles.Controls[index];
            if (panel == null)
                throw new Exception("Panel wasn't found at index!");

            foreach (object o in panel.Controls)
            {
                if (o.GetType() != typeof(ProgressBar))
                    continue;

                ((ProgressBar) o).Value = progress;
            }
        }

        public List<bool> GetContentPatchStatus()
        {
            var titlesToPatch = new List<bool>();

            foreach (object o in FLP_Titles.Controls)
            {
                if (o.GetType() != typeof(Panel))
                    continue;

                titlesToPatch.AddRange(from object obj in ((Panel) o).Controls where obj.GetType() == typeof(CheckBox) select ((CheckBox) obj).Checked);
            }

            return titlesToPatch;
        }

        public void SetPatcherMode(Mode mode, bool busy)
        {
            switch (mode)
            {
                case Mode.GamePatching:
                    B_BrowsePatch.Enabled = B_BrowseDLC.Enabled = B_BrowseGame.Enabled = TB_PatchPath.Enabled = TB_GamePath.Enabled = TB_ContentPath.Enabled = !busy;
                    B_ApplyPatch.Text = busy ? "Cancel" : "Apply Patch";
                    B_PatchDLC.Text = busy ? "Cancel" : "Unpack Content";
                    break;
                case Mode.DLCUnpacking:
                    B_BrowsePatch.Enabled = B_BrowseDLC.Enabled = TB_PatchPath.Enabled = TB_ContentPath.Enabled = !busy;
                    B_ApplyPatch.Text = busy ? "Cancel" : "Apply Patch";
                    B_PatchDLC.Text = busy ? "Cancel" : "Apply Patch";
                    break;
                case Mode.DLCPatching:
                    B_BrowsePatch.Enabled = B_BrowseDLC.Enabled = TB_PatchPath.Enabled = TB_ContentPath.Enabled = !busy;
                    B_ApplyPatch.Text = busy ? "Cancel" : "Apply Patch";
                    B_PatchDLC.Text = busy ? "Cancel" : "Unpack Content";
                    break;
            }
        }

        #region Events

        private void B_BrowsePatch_Click(object sender, EventArgs e)
        {
            PatchFolderPath = cont.GetPatchPath() ?? PatchFolderPath;
        }

        private void B_BrowseGame_Click(object sender, EventArgs e)
        {
            var btn = (Button) sender;
            if(btn.Name == "B_BrowseGame")
                GameFilePath = cont.GetGamePath() ?? GameFilePath;
            else
                ContentFilePath = cont.GetGamePath() ?? ContentFilePath;
        }

        private void B_ApplyPatch_Click(object sender, EventArgs e)
        {
            cont.BeginPatching(false);
        }

        private void B_PatchDLC_Click(object sender, EventArgs e)
        {
            cont.BeginPatching(true);
        }

        private void FLP_Titles_SizeChanged(object sender, EventArgs e)
        {
            int newWidth = FLP_Titles.Size.Width - 8;
            if (FLP_Titles.VerticalScroll.Visible)
                newWidth -= 20;
            if (newWidth < 1)
                newWidth = 1;

            foreach (object o in FLP_Titles.Controls)
                ((Panel)o).Size = new Size(newWidth, 58);
        }

        private void FLP_Titles_ControlAdded(object sender, ControlEventArgs e)
        {
            int newWidth = FLP_Titles.Size.Width - 8;
            if (FLP_Titles.VerticalScroll.Visible)
                newWidth -= 20;
            if (newWidth < 1)
                newWidth = 1;

            foreach (object o in FLP_Titles.Controls)
                ((Panel)o).Size = new Size(newWidth, 58);
        }

        private void TB_TextChanged(object sender, EventArgs e)
        {
            cont.ResetMode();
        }

        private void Panel_Click(object sender, EventArgs e)
        {
            FLP_Titles.Focus();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Workspace.Clean();
        }
        #endregion
    }
}
