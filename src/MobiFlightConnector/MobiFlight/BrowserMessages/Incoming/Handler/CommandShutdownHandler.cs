using System;
using System.Collections.Generic;
using System.Text;
using MobiFlight.UI;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandShutdownHandler
    {
        private readonly MainForm _mainForm;

        public CommandShutdownHandler(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public void Handle(CommandShutdown command)
        {
            switch (command.Action)
            {
                case CommandShutdownAction.saveChanges:
                    _mainForm.confirmShutdownSavingChanges();
                    break;
                case CommandShutdownAction.discardChanges:
                    _mainForm.confirmShutdownDiscardingChanges();
                    break;
            }
        }
    }
}
