using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IfPatcher.Model;

namespace IfPatcher.Controller
{
    interface IDLCView
    {
        string PatchFolderPath { get; set; }
        string ContentFilePath { get; set; }
        int TitlePercentageComplete { get; set; }

        void SetController(MainController c);
        void SetPatcherMode(Mode mode, bool busy);
        void PopulateContentList(List<ContentData> titles);
        void SetContentProgress(int index, int progress);
        List<bool> GetContentPatchStatus();
    }
}
