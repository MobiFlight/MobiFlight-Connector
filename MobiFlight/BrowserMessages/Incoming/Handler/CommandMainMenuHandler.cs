using MobiFlight.UI;

namespace MobiFlight.BrowserMessages.Incoming.Handler
{
    public class CommandMainMenuHandler
    {
        private readonly MainForm _mainForm;
        public CommandMainMenuHandler(MainForm mainForm)
        {
            _mainForm = mainForm;
        }

        public void Handle(CommandMainMenu message)
        {
            switch (message.Action)
            {
                // File Menu Actions
                case CommandMainMenuAction.file_new:
                    _mainForm.newFileToolStripMenuItem_Click(message.Options);
                    break;
                case CommandMainMenuAction.file_open:
                    _mainForm.loadToolStripMenuItem_Click(null, null);
                    break;
                case CommandMainMenuAction.file_save:
                    _mainForm.saveToolStripButton_Click(null, null);
                    break;
                case CommandMainMenuAction.file_saveas:
                    _mainForm.saveAsToolStripMenuItem_Click(null, null);
                    break;
                case CommandMainMenuAction.file_recent:
                    var filePath = message.Options?.FilePath != null ? message.Options.FilePath : message.Options?.Project.FilePath;
                    if (filePath == null) return;

                    _mainForm.LoadConfig(filePath);
                    break;
                case CommandMainMenuAction.project_edit:
                    _mainForm.updateProjectSettings(message.Options.Project);
                    break;
                case CommandMainMenuAction.file_exit:
                    _mainForm.exitToolStripMenuItem_Click(_mainForm, null);
                    break;

                // Extras Menu Actions
                case CommandMainMenuAction.extras_hubhop_download:
                    _mainForm.downloadHubHopPresetsToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.extras_msfs_reinstall:
                    _mainForm.installWasmModuleToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.extras_copylogs:
                    _mainForm.copyLogsToClipboardToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.extras_serials:
                    _mainForm.orphanedSerialsFinderToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.extras_settings:
                    _mainForm.settingsToolStripMenuItem_Click(null, null);
                    break;

                // Help Menu Actions
                case CommandMainMenuAction.help_docs:
                    _mainForm.documentationToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.help_checkforupdate:
                    _mainForm.checkForUpdateToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.help_discord:
                    _mainForm.openDiscordServer_Click(null, null);
                    break;

                case CommandMainMenuAction.help_youtube:
                    _mainForm.YouTubeToolStripButton_Click(null, null);
                    break;

                case CommandMainMenuAction.help_hubhop:
                    _mainForm.HubHopToolStripButton_Click(null, null);
                    break;

                case CommandMainMenuAction.help_about:
                    _mainForm.AboutToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.help_releasenotes:
                    _mainForm.releaseNotesToolStripMenuItem_Click(null, null);
                    break;

                case CommandMainMenuAction.help_donate:
                    _mainForm.donateToolStripButton_Click(null, null);
                    break;

                // Virtual Menu Actions
                case CommandMainMenuAction.virtual_recent_remove:
                    var index = message.Index;
                    _mainForm.RecentFilesRemove(index);
                    break;
            }
        }
    }
}

