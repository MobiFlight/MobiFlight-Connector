using MobiFlight.UI;
using System;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandProjectToolbarHandler
    {
        private readonly IProjectToolbar _form;
        
        public CommandProjectToolbarHandler(IProjectToolbar form)
        {
            _form = form;
        }

        public void Handle(CommandProjectToolbar message)
        {
            switch (message.Action)
            {
                // File Menu Actions
                case CommandProjectToolbarAction.run:
                    _form.StartProjectExecution();
                    break;

                case CommandProjectToolbarAction.stop:
                    _form.StopExecution();
                    break;

                case CommandProjectToolbarAction.test:
                    _form.StartTestModeExecution();
                    break;

                case CommandProjectToolbarAction.toggleAutoRun:
                    _form.ToggleAutoRunSetting();
                    break;

                case CommandProjectToolbarAction.rename:
                    _form.RenameProject(message.Value);
                    break;
            }
        }
    }
}
    
