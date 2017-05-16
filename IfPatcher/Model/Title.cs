using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IfPatcher.Model
{
    public class ContentData
    {
        public string Name;
        public string Index;
        public string ID;

        public bool Patchable;
        public bool WillPatch;
        public Image Icon;

        public ContentData(string index, string id, bool patchable)
        {
            Name = TranslateName(index) ?? "???";
            Index = index;
            ID = id;
            WillPatch = Patchable = patchable;
        }

        private string TranslateName(string index)
        {
            string[] names = Properties.Resources.DLCNames.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, string> engNames = names.Select(name => name.Split('\t')).ToDictionary(entry => entry[0].ToLower(), entry => entry[1]);

            if (!engNames.ContainsKey(index))
            {
                Debug.WriteLine($"{index} does not have a translated name!");
                return null;
            }

            return engNames[index];
        }
    }
}
