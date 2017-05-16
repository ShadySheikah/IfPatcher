using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IfPatcher.Model;

namespace IfPatcher.Controller
{
    public class MainController
    {
        private readonly IMain mainView;
        private readonly IDLCView dlcView;
        private readonly BasePatcher model;
        private readonly DLCPatcher dlcModel;
        private Mode curMode;

        private BackgroundWorker bw;
        private FolderBrowserDialog fbd;
        private OpenFileDialog ofd;

        internal MainController(IMain v, IDLCView v2, BasePatcher m, DLCPatcher m2)
        {
            mainView = v;
            dlcView = v2;
            model = m;
            dlcModel = m2;

            mainView.SetController(this);
            model.SetController(this);
            InitializeBackgroundWorker();
        }

        #region BackgroundWorker

        private void InitializeBackgroundWorker()
        {
            bw = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bw.DoWork += Bw_DoWork;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }

        private void Bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker) sender;
            switch (curMode)
            {
                case Mode.GamePatching:
                    e.Result = model.BeginPatch(mainView.PatchFolderPath, mainView.GameFilePath, worker, e);
                    break;
                case Mode.DLCUnpacking:
                    e.Result = dlcModel.UnpackContents(dlcView.ContentFilePath, dlcView.PatchFolderPath, worker, e);
                    break;
                case Mode.DLCPatching:
                    e.Result = dlcModel.PatchContents(dlcView.GetContentPatchStatus(), dlcView.PatchFolderPath, worker, e);
                    break;
            }
        }

        private void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (curMode == Mode.DLCPatching)
            {
                if (e.UserState != null)
                    dlcView.SetContentProgress((int)e.UserState, e.ProgressPercentage);
                else
                    dlcView.TitlePercentageComplete = e.ProgressPercentage;

                return;
            }

            mainView.PercentageComplete = e.ProgressPercentage;
        }

        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
                curMode = Mode.GamePatching;
            }
            else
            {
                if (e.Cancelled)
                {
                    AppendLog("Patching cancelled.");
                    mainView.PercentageComplete = 0;
                    curMode = Mode.GamePatching;

                }
                else if (e.Result == null)
                {
                    MessageBox.Show("Unknown error occured.", "Error");
                    mainView.PercentageComplete = 0;
                    curMode = Mode.GamePatching;
                }
                else
                {
                    switch (curMode)
                    {
                        case Mode.GamePatching:
                            SavePatchedFile((string)e.Result);
                            Workspace.Clean();
                            break;
                        case Mode.DLCUnpacking:
                            dlcView.PopulateContentList((List<ContentData>)e.Result);
                            break;
                        case Mode.DLCPatching:
                            SavePatchedFile((string) e.Result);
                            Workspace.Clean();
                            break;
                    }
                }
            }

            mainView.SetPatcherMode(curMode, false);
        }
        #endregion

        public void BeginPatching(bool dlc)
        {
            if (bw.IsBusy)
            {
                DialogResult result = MessageBox.Show("This will cancel the action in progress!\nAre you sure you want to cancel?", "Cancel", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    AppendLog("Cancelling...");
                    bw.CancelAsync();
                }

                return;
            }

            switch (curMode)
            {
                case Mode.GamePatching:
                    curMode = dlc ? Mode.DLCUnpacking : Mode.GamePatching;
                    break;
                case Mode.DLCUnpacking:
                    curMode = dlc ? Mode.DLCPatching : Mode.GamePatching;
                    break;
                case Mode.DLCPatching:
                    curMode = dlc ? Mode.DLCUnpacking : Mode.GamePatching;
                    break;
            }

            mainView.SetPatcherMode(curMode, true);
            bw.RunWorkerAsync();
        }

        public string GetPatchPath()
        {
            fbd = new FolderBrowserDialog {Description = "Select the directory containing the patch files."};
            return fbd.ShowDialog() == DialogResult.OK ? fbd.SelectedPath : null;
        }

        public string GetGamePath()
        {
            ofd = new OpenFileDialog
            {
                Filter = "CTR Importable Archive (*.cia)|*.cia|All files (*.*)|*.*",
                FilterIndex = 1,
                FileName = string.Empty
            };

            return ofd.ShowDialog() == DialogResult.OK ? ofd.FileName : null;
        }

        public static void ConfirmOutOfRegion()
        {
            MessageBox.Show("You are patching a non-Japanese version of Fire Emblem If.\nCharacter names and other text will be changed according to the patch, but the removed Skinship minigame cannot be re-enabled.", "Out of Region Notice", MessageBoxButtons.OK);
        }

        private void SavePatchedFile(string tempPath)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "CTR Importable Archive (*.cia)|*.cia",
                FilterIndex = 1,
                FileName = Path.GetFileName(tempPath)
            };

            if (sfd.ShowDialog() != DialogResult.OK)
                return;

            if (File.Exists(sfd.FileName))
                File.Replace(tempPath, sfd.FileName, null);
            else
                File.Move(tempPath, sfd.FileName);

            AppendLog($"Patched game successfully saved as {Path.GetFileName(sfd.FileName)}!");
        }

        public void AppendLog(string message)
        {
            mainView.AppendLog(message + Environment.NewLine);
        }

        public void ResetMode()
        {
            curMode = Mode.GamePatching;
            mainView.SetPatcherMode(curMode, false);
        }
    }
}

public enum Mode
{
    GamePatching,
    DLCUnpacking,
    DLCPatching
}
