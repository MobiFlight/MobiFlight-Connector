namespace MobiFlight.UI
{
    public interface IProjectToolbar
    {
        void StartProjectExecution();
        void StartTestModeExecution();
        void StopExecution();
        void ToggleAutoRunSetting();
        void RenameProject(string newName);
    }
}
