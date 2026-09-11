using MobiFlight.UI;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandShutdownHandler
    {
        private readonly MainForm _mainForm;

        public bool IsShutdownConfirmed { get; private set; }

        public CommandShutdownHandler(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public void Handle(CommandShutdown command)
        {
            switch (command.Action)
            {
                case CommandShutdownAction.saveChanges:
                    IsShutdownConfirmed = true;
                    _mainForm.confirmShutdownSavingChanges();

                    if (_mainForm.ProjectHasUnsavedChanges)
                    {
                        IsShutdownConfirmed = false;
                    }
                    break;

                case CommandShutdownAction.discardChanges:
                    IsShutdownConfirmed = true;
                    _mainForm.confirmShutdownDiscardingChanges();
                    break;
            }
        }
    }
}
