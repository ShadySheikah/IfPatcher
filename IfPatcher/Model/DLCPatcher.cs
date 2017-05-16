using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IfPatcher.Controller;
using _3DSExplorer.Utils;

namespace IfPatcher.Model
{
    public class DLCPatcher
    {
        private MainController cont;
        private List<ContentData> contentList;

        public void SetController(MainController c)
        {
            cont = c;
        }

        private void VerifyPaths(string game, string patch)
        {
            if (!File.Exists(game))
                throw new ArgumentException("Path to DLC file does not exist.");

            var pathInfo = new DirectoryInfo(patch);
            if (!pathInfo.Exists)
                throw new ArgumentException("Path to patch directory does not exist.");

            DirectoryInfo[] innerDirs = pathInfo.GetDirectories();
            if (innerDirs.All(info => info.Name != "dlc"))
                throw new Exception("Patch directory does not contain a \"dlc\" folder.");
        }

        public List<ContentData> UnpackContents(string gamePath, string patchPath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            VerifyPaths(gamePath, patchPath);
            Workspace.Setup();

            //Copy CIA to workspace
            string tempGame = Path.Combine(Workspace.TempDir, "dlc.cia");
            File.Copy(gamePath, tempGame);

            //Extract CIA contents
            var tool = new Process
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
                    Arguments = "--contents=tmp dlc.cia"
                }
            };
            tool.Start();
            tool.WaitForExit();

            //Set up list of discovered titles
            contentList = new List<ContentData>();
            string[] dlcPaths = Directory.GetDirectories(Path.Combine(patchPath, "dlc"));
            foreach (string path in Directory.EnumerateFiles(Workspace.TempDir, "tmp.*", SearchOption.TopDirectoryOnly))
            {
                string[] indexId = Path.GetFileName(path).Substring(4).Split('.');
                var patchable = false;
                foreach (string str in dlcPaths)
                    if (str.Contains($"{indexId[0]}.{indexId[1]}"))
                        patchable = true;

                contentList.Add(new ContentData(indexId[0], indexId[1], patchable));
            }

            GetContentIcons();
            return contentList;
        }

        private void GetContentIcons()
        {
            //Most of this block is based on code written by SciresM.
            //Original project: https://github.com/SciresM/DLCTool

            //Extract romfs from dummy content
            ContentData content = contentList.First(cd => cd.Index == "0000");
            if (content == null)
                throw new Exception("Could not find dummy content.");

            var cmd = new Process
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
                    Arguments = $"--romfsdir=meta tmp.{content.Index}.{content.ID}"
                }
            };
            cmd.Start();
            cmd.WaitForExit();

            //Grab the content info archive
            byte[] archive = Directory.EnumerateFiles(Path.Combine(Workspace.TempDir, "meta"), "ContentInfoArchive*", SearchOption.TopDirectoryOnly).Select(File.ReadAllBytes).FirstOrDefault();

            if (archive == null)
                throw new Exception("Could not read content info archive.");

            //Find icons
            for (int i = 0; i < contentList.Count; i++)
            {
                //Get the icon index
                uint iconIndex = BitConverter.ToUInt32(archive, 0xC8 + 0xC8 * i);
                if (iconIndex == 0)
                    iconIndex = 22;

                //Determine icon file based on index
                string iconPath = Path.Combine(Workspace.TempDir, "meta", "icons", iconIndex + ".icn");

                //Load icon, save to content
                byte[] iconData = File.ReadAllBytes(iconPath);
                if (iconData.Length != 0x1200)
                    throw new Exception("Error reading icon file.");

                Image icon;
                using (var stream = new MemoryStream(iconData))
                    icon = ImageUtil.ReadImageFromStream(stream, 48, 48, ImageUtil.PixelFormat.RGB565);

                contentList[i].Icon = icon;
            }
        }

        public string PatchContents(List<bool> patchList, string patchPath, BackgroundWorker worker, DoWorkEventArgs e)
        {
            for (int i = 0; i < patchList.Count; i++)
            {
                float progress = (float)i / patchList.Count * 100f;
                worker.ReportProgress((int) progress);
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return null;
                }

                //If it's not going to get patched, skip
                if (!patchList[i])
                    continue;

                //Patch the title that correlates to the patch list
                worker.ReportProgress(25, i);
                ExtractContent(contentList[i]);
                worker.ReportProgress(50, i);

                PatchContent(contentList[i], patchPath);
                worker.ReportProgress(75, i);

                RebuildContent(contentList[i]);
                worker.ReportProgress(100, i);
            }

            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return null;
            }

            string savePath = RebuildCIA();
            worker.ReportProgress(100);
            return savePath;
        }

        private void ExtractContent(ContentData t)
        {
            string contentID = t.Index + '.' + t.ID;
            string fileName = "tmp." + contentID;

            //Extract the romfs
            var cmd = new Process
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
                    Arguments = $"--romfsdir={contentID}rom {fileName}"
                }
            };
            cmd.Start();
            cmd.WaitForExit();

            //Extract the header
            cmd.StartInfo.FileName = "3dstool.exe";
            cmd.StartInfo.Arguments = $"-xvtf cfa {fileName} --header {contentID}header.bin";
            cmd.Start();
            cmd.WaitForExit();
        }

        private void PatchContent(ContentData t, string patch)
        {
            string contentID = t.Index + '.' + t.ID;

            //Get rom path, make sure it's there
            string contentRom = Path.Combine(Workspace.TempDir, contentID + "rom", "local");
            if (!Directory.Exists(contentRom))
                throw new Exception("Could not find necessary data.\nMake sure the provided CIA is decrypted and a valid DLC file.");

            //Get path to the patch's dlc-specific path
            string patchRom = Path.Combine(patch, "dlc", contentID);

            //Copy patch files
            foreach (string dir in Directory.GetDirectories(patchRom, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(patchRom, contentRom));
            foreach (string file in Directory.GetFiles(patchRom, "*", SearchOption.AllDirectories))
                File.Copy(file, file.Replace(patchRom, contentRom), true);
        }

        private void RebuildContent(ContentData t)
        {
            string contentID = t.Index + '.' + t.ID;

            //Rebuild romfs
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
                    Arguments = $"-cvtf romfs {contentID}rom.bin --romfs-dir {contentID}rom/"
                }
            };
            cmd.Start();
            cmd.WaitForExit();

            if (!File.Exists(Path.Combine(Workspace.TempDir, contentID + "rom.bin")))
                throw new Exception("RomFS was not rebuilt successfully.");

            //Rebuild content
            cmd.StartInfo.Arguments = $"-cvtf cfa tmp.{contentID} --romfs {contentID}rom.bin --header {contentID}header.bin";
            cmd.Start();
            cmd.WaitForExit();

            //Clean up mess
            Directory.Delete(Path.Combine(Workspace.TempDir, contentID + "rom"), true);
            File.Delete(Path.Combine(Workspace.TempDir, contentID + "rom.bin"));
            File.Delete(Path.Combine(Workspace.TempDir, contentID + "header.bin"));
        }

        private string RebuildCIA()
        {
            //Prep content for makerom
            var builder = new StringBuilder();
            foreach (ContentData t in contentList)
            {
                builder.Append($"-i tmp.{t.Index}.{t.ID}");
                builder.Append($":0x{t.Index}:0x{t.ID} ");
            }

            //Build CIA
            string tempTitleName = "patchedDLC.cia";
            var cmd = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Workspace.TempDir,
                    FileName = "makerom.exe",
#if DEBUG
                    WindowStyle = ProcessWindowStyle.Normal,
#else
                    WindowStyle = ProcessWindowStyle.Hidden,
#endif
                    Arguments = $"-f cia -o {tempTitleName} -ckeyid 0 -major 0 -minor 10 -micro 0 -DSaveSize=0 -dlc {builder}"
                }
            };
            cmd.Start();
            cmd.WaitForExit();

            if (!File.Exists(Path.Combine(Workspace.TempDir, tempTitleName)))
                throw new Exception("Rebuilt CIA cannot be found.");

            return Path.Combine(Workspace.TempDir, tempTitleName);
        }
    }
}
