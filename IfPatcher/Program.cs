using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IfPatcher.Controller;
using IfPatcher.Model;

namespace IfPatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var model = new BasePatcher();
            var dlcModel = new DLCPatcher();
            var view = new MainForm();
            new MainController(view, view, model, dlcModel);
            Application.Run(view);
        }
    }
}
