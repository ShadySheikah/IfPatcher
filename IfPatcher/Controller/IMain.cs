namespace IfPatcher.Controller
{
    public interface IMain
    {
        string PatchFolderPath { get; set; }
        string GameFilePath { get; set; }
        int PercentageComplete { get; set; }

        void SetController(MainController c);
        void AppendLog(string message);
        void SetPatcherMode(Mode mode, bool busy);
    }
}
