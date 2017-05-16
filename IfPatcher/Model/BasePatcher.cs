using IfPatcher.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfPatcher.Model
{
    public class BasePatcher
    {
        private MainController cont;
        private GameVersion version;

        public void SetController(MainController c)
        {
            cont = c;
        }

        public string BeginPatch(string patchPath, string gamePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            cont.AppendLog($"Beginning patching process for: {Path.GetFileName(gamePath)}");
            VerifyPaths(patchPath, gamePath);
            worker.ReportProgress(25);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return null;
            }

            ExtractFiles(gamePath, worker, e);
            worker.ReportProgress(50);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return null;
            }

            ApplyPatch(patchPath, worker, e);
            worker.ReportProgress(75);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return null;
            }

            string patchedPath = InitiateRebuild(worker, e);
            worker.ReportProgress(100);

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return null;
            }

            return patchedPath;
        }

        private void VerifyPaths(string patch, string game)
        {
            cont.AppendLog("Verifying chosen paths...");

            if (!File.Exists(game))
                throw new ArgumentException("Path to game file does not exist.");

            var pathInfo = new DirectoryInfo(patch);
            if(!pathInfo.Exists)
                throw new ArgumentException("Path to patch directory does not exist.");

            DirectoryInfo[] innerDirs = pathInfo.GetDirectories();
            if (innerDirs.All(info => info.Name != "exe"))
                throw new Exception("Patch directory does not contain an \"exe\" folder.");
            if (innerDirs.All(info => info.Name != "rom"))
                throw new Exception("Patch directory does not contain a \"rom\" folder.");
        }

        private void ExtractFiles(string gamePath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            Workspace.Setup();

            cont.AppendLog("Unpacking CIA...");

            //Copy game to workspace
            string tempGame = Path.Combine(Workspace.TempDir, "game.cia");
            File.Copy(gamePath, tempGame);

            if (worker.CancellationPending)
                return;

            //Begin extracting with ctrtool
            var ctrtool = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Workspace.TempDir,
                    FileName = "ctrtool.exe",
#if DEBUG
                    WindowStyle = ProcessWindowStyle.Normal,
#else
                    WindowStyle = ProcessWindowStyle.Hidden,
#endif
                    Arguments = "--contents=tmp game.cia"
                }
            };
            ctrtool.Start();
            ctrtool.WaitForExit();

            if (worker.CancellationPending)
                return;

            //No need for game copy anymore
            File.Delete(tempGame);

            //Verify we have the file we need
            FileInfo[] dir = new DirectoryInfo(Workspace.TempDir).GetFiles("tmp.*");
            if (dir.Length != 2)
                throw new Exception("Unpacked content was not what was expected.\nPlease ensure the provided CIA is not an update or DLC file.");

            string cxiName = string.Empty, cfaName = string.Empty;
            foreach (FileInfo fi in dir)
            {
                if (fi.Name.Contains(".0000."))
                    cxiName = fi.Name;
                else if (fi.Name.Contains(".0001."))
                    cfaName = fi.Name;
            }

            if (cxiName == string.Empty || cfaName == string.Empty)
                throw new Exception("Expected content not present.\nPlease make sure the provided CIA file is decrypted and valid.");

            //Grab serial
            string cxiPath = Path.Combine(Workspace.TempDir, cxiName);
            if (!File.Exists(cxiPath))
                throw new Exception("Could not find necessary data.\nMake sure the provided CIA is decrypted and not an update or DLC file.");

            var cxi = new byte[0x200];
            using (var b = new BinaryReader(File.Open(cxiPath, FileMode.Open)))
            {
                b.BaseStream.Read(cxi, 0, cxi.Length);
            }
            string serial = Encoding.ASCII.GetString(cxi, 0x150, 10);
            if (GetGameVersion(serial) == GameVersion.Other)
                throw new Exception("Game is not a version of Fire Emblem If.");

            if (worker.CancellationPending)
                return;

            //Extract the rest
            cont.AppendLog("Extracting game content. This will take some time...");
            ctrtool.StartInfo.FileName = "3dstool.exe";
            ctrtool.StartInfo.Arguments = $"-xvtf cxi {cxiName} --header ncchheader.bin --exh exheader.bin --plain plain.bin --exefs exe.bin --romfs rom.bin --logo logo.bin";
            ctrtool.Start();
            ctrtool.WaitForExit();

            if (worker.CancellationPending)
                return;

            ctrtool.StartInfo.Arguments = "-xuvtf exefs exe.bin --exefs-dir exe --header exefsheader.bin";
            ctrtool.Start();
            ctrtool.WaitForExit();

            if (worker.CancellationPending)
                return;

            ctrtool.StartInfo.Arguments = "-xvtf romfs rom.bin --romfs-dir rom";
            ctrtool.Start();
            ctrtool.WaitForExit();

            if (worker.CancellationPending)
                return;

            if (Directory.GetFiles(Path.Combine(Workspace.TempDir, "rom")).Length < 1)
                throw new Exception("Could not extract data.\nMake sure the CIA file is decrypted before attempting to patch it.");

            //Clean up
            File.Move(Path.Combine(Workspace.TempDir, cfaName), Path.Combine(Workspace.TempDir, "manual.cfa"));
            File.Delete(Path.Combine(Workspace.TempDir, cxiName));
            File.Delete(Path.Combine(Workspace.TempDir, "exe.bin"));
            File.Delete(Path.Combine(Workspace.TempDir, "rom.bin"));
        }

        private GameVersion GetGameVersion(string serial)
        {
            if (serial == string.Empty)
                throw new Exception("Game serial couldn't be found.");

            string shortSer = serial.Substring(6);
            switch (shortSer)
            {
                case "BFWJ":
                    version = GameVersion.JPN_eShop;
                    break;
                case "BFXJ":
                    version = GameVersion.JPN_Hoshido;
                    break;
                case "BFYJ":
                    version = GameVersion.JPN_Nohr;
                    break;
                case "BFZJ":
                    version = GameVersion.JPN_Special;
                    break;
                case "BFXE":
                    version = GameVersion.USA_Hoshido;
                    MainController.ConfirmOutOfRegion();
                    break;
                case "BFYE":
                    version = GameVersion.USA_Nohr;
                    MainController.ConfirmOutOfRegion();
                    break;
                case "BFZE":
                    version = GameVersion.USA_Special;
                    MainController.ConfirmOutOfRegion();
                    break;
                case "BFXP":
                    version = GameVersion.EUR_Hoshido;
                    MainController.ConfirmOutOfRegion();
                    break;
                case "BFYP":
                    version = GameVersion.EUR_Nohr;
                    MainController.ConfirmOutOfRegion();
                    break;
                case "BFZP":
                    version = GameVersion.EUR_Special;
                    MainController.ConfirmOutOfRegion();
                    break;
                default:
                    version = GameVersion.Other;
                    break;
            }

            if(version != GameVersion.Other)
                cont.AppendLog($"Game version: {version.ToString().Replace('_', ' ')}");

            return version;
        }

        private void ApplyPatch(string patch, BackgroundWorker worker, DoWorkEventArgs e)
        {
            cont.AppendLog("Replacing game files with patched copies...");

            //Get temp directories
            string tempExe = Path.Combine(Workspace.TempDir, "exe");
            string tempRom = Path.Combine(Workspace.TempDir, "rom");

            //Just in case, make sure we have the extracted content directories
            if (!Directory.Exists(tempExe) || !Directory.Exists(tempRom))
                throw new Exception("Extracted content unaccounted for.");

            //Get the patch directories
            string patchExe = Path.Combine(patch, "exe");
            string patchRom = Path.Combine(patch, "rom");

            //Recreate directory structure
            foreach (string dir in Directory.GetDirectories(patchExe, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(patchExe, tempExe));
            foreach (string dir in Directory.GetDirectories(patchRom, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(patchRom, tempRom));

            if (worker.CancellationPending)
                return;

            //Copy patch files
            foreach (string file in Directory.GetFiles(patchExe, "*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(patchExe, tempExe), true);
            foreach (string file in Directory.GetFiles(patchRom, "*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(patchRom, tempRom), true);

            if (worker.CancellationPending)
                return;

            //Clean up files from unused story paths
            cont.AppendLog("Cleaning up...");
            string mPath = Path.Combine(tempRom, "m");
            var m = new DirectoryInfo(mPath);

            switch (version)
            {
                case GameVersion.JPN_Hoshido:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name == "B" || di.Name == "C")
                            Directory.Delete(di.FullName, true);
                    break;
                case GameVersion.JPN_Nohr:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name == "A" || di.Name == "C")
                            Directory.Delete(di.FullName, true);
                    break;
                case GameVersion.JPN_Special:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name == "C")
                            Directory.Delete(di.FullName, true);
                    break;
                case GameVersion.JPN_eShop:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name != "common")
                            Directory.Delete(di.FullName, true);
                    break;
                case GameVersion.USA_Hoshido:
                case GameVersion.EUR_Hoshido:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name == "B" || di.Name == "C")
                            Directory.Delete(di.FullName, true);
                    break;
                case GameVersion.USA_Nohr:
                case GameVersion.EUR_Nohr:
                    foreach (DirectoryInfo di in m.GetDirectories())
                        if (di.Name == "A" || di.Name == "C")
                            Directory.Delete(di.FullName, true);
                    break;
            }

            //If the game is a localized version, format text files to match
            if (version.ToString().Contains("JPN"))
                return;

            if (worker.CancellationPending)
                return;

            cont.AppendLog("Reorganizing files to match localization file structure...");
            string locDir = version.ToString().Contains("USA") ? "@E" : "@U";
            foreach (DirectoryInfo di in m.GetDirectories())
            {
                if (di.Name == "common" || di.Name == locDir)
                    continue;

                Directory.CreateDirectory(Path.Combine(di.FullName, locDir));
                foreach (string file in Directory.GetFiles(di.FullName))
                {
                    string destPath = file.Replace(di.FullName, Path.Combine(di.FullName, locDir));
                    if (File.Exists(destPath))
                        File.Replace(file, destPath, null);
                    else
                        File.Move(file, destPath);
                }
            }

            if (worker.CancellationPending)
                return;

            m.CreateSubdirectory(locDir);
            foreach (FileInfo fi in m.GetFiles())
            {
                string destPath = Path.Combine(mPath, locDir, fi.Name);
                if (File.Exists(destPath))
                    File.Replace(fi.FullName, destPath, null);
                else
                    File.Move(fi.FullName, destPath);
            }
        }

        private string InitiateRebuild(BackgroundWorker worker, DoWorkEventArgs e)
        {
            //Rebuild exe and rom
            cont.AppendLog("Patch applied, rebuilding CIA...");
            var cmd = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Workspace.TempDir,
                    FileName = "3dstool.exe",
#if DEBUG
                    WindowStyle = ProcessWindowStyle.Normal,
#else
                    WindowStyle = ProcessWindowStyle.Hidden,
#endif
                    Arguments = "-cvtf exefs exefs.bin --exefs-dir exe/ --header exefsheader.bin"
                }
            };
            cmd.Start();
            cmd.WaitForExit();

            if (worker.CancellationPending)
                return null;

            cmd.StartInfo.Arguments = "-cvtf romfs romfs.bin --romfs-dir rom/";
            cmd.Start();
            cmd.WaitForExit();

            if (worker.CancellationPending)
                return null;

            //Rebuild cxi
            cmd.StartInfo.Arguments = "-cvtf cxi ncch.cxi --header ncchheader.bin --exh exheader.bin --logo logo.bin --plain plain.bin --exefs exefs.bin --romfs romfs.bin";
            cmd.Start();
            cmd.WaitForExit();

            if (worker.CancellationPending)
                return null;

            //Build CIA
            const string tempGameName = "patchedGame.cia";

            cmd.StartInfo.FileName = "makerom.exe";
            cmd.StartInfo.Arguments = $"-f cia -o {tempGameName} -content ncch.cxi:0:0 -content manual.cfa:1:1";
            cmd.Start();
            cmd.WaitForExit();

            return Path.Combine(Workspace.TempDir, tempGameName);
        }
    }
}

public enum GameVersion
{
    JPN_Hoshido,
    JPN_Nohr,
    JPN_Special,
    JPN_eShop,
    USA_Hoshido,
    USA_Nohr,
    USA_Special,
    EUR_Hoshido,
    EUR_Nohr,
    EUR_Special,
    Other
}
