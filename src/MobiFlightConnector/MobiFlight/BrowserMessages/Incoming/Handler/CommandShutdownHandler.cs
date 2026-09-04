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
            if (command.Action == CommandShutdownAction.discardChanges)
            {
                _mainForm.confirmShutdownDiscardingChanges();
            }
        }
    }
}
