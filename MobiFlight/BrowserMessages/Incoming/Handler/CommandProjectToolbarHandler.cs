using MobiFlight.UI;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandProjectToolbarHandler
    {
        private readonly MainForm _mainForm;
        public CommandProjectToolbarHandler(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public void Handle(CommandProjectToolbar message)
        {
            switch (message.Action)
            {
                // File Menu Actions
                case CommandProjectToolbarAction.run:
                    _mainForm.startToolStripButton_Click(null, null);
                    break;

                case CommandProjectToolbarAction.stop:
                    _mainForm.stopToolStripButton_Click(null, null);
                    _mainForm.stopTestToolStripButton_Click(null, null);
                    break;

                case CommandProjectToolbarAction.test:
                    _mainForm.runTestToolStripLabel_Click(null, null);
                    break;

                case CommandProjectToolbarAction.toggleAutoRun:
                    _mainForm.AutoRunToolStripButton_Click(null, null);
                    break;

                case CommandProjectToolbarAction.rename:
                    if (string.IsNullOrEmpty(message.Value)) return;
                    
                    break;
            }
        }
    }
}
    
