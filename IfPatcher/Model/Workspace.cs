using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfPatcher.Model
{
    public class Workspace
    {
        public static string TempDir;

        static Workspace()
        {
            TempDir = Path.Combine(Path.GetTempPath(), "IfPatcher");
        }

        public static void Setup()
        {
            if (string.IsNullOrEmpty(TempDir))
                throw new Exception("TempDir not assigned.");

            //Set up temp directory
            if (!Directory.Exists(TempDir))
                Directory.CreateDirectory(TempDir);

            Clean();

            File.WriteAllBytes(Path.Combine(TempDir, "ctrtool.exe"), Properties.Resources.ctrtool);
            File.WriteAllBytes(Path.Combine(TempDir, "makerom.exe"), Properties.Resources.makerom);
            File.WriteAllBytes(Path.Combine(TempDir, "3dstool.exe"), Properties.Resources._3dstool);
        }

        public static void Clean()
        {
            if (string.IsNullOrEmpty(TempDir))
                throw new Exception("TempDir not assigned.");

            //Clear out old junk
            var di = new DirectoryInfo(TempDir);
            foreach (FileInfo f in di.EnumerateFiles())
                f.Delete();
            foreach (DirectoryInfo d in di.EnumerateDirectories())
                d.Delete(true);
        }
    }
}
