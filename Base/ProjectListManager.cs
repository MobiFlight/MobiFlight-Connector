using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobiFlight.Base
{
    /// <summary>
    /// Manages both the MRU RecentFiles list and the stable ProjectList for UI display
    /// </summary>
    public class ProjectListManager
    {
        /// <summary>
        /// Stable list for UI - new projects added at top, existing items never reordered
        /// </summary>
        private List<string> projectListFiles = new List<string>();

        /// <summary>
        /// Initializes both lists from the RecentFiles setting
        /// </summary>
        public void InitializeFromSettings()
        {
            projectListFiles = Properties.Settings.Default.RecentFiles.Cast<string>().ToList();
        }

        /// <summary>
        /// Adds a project to both the MRU RecentFiles and the stable project list
        /// </summary>
        public void OpenProject(string filePath)
        {
            if (string.IsNullOrEmpty(filePath?.Trim())) return;

            // Update MRU RecentFiles (always reorder)
            if (Properties.Settings.Default.RecentFiles.Contains(filePath))
            {
                Properties.Settings.Default.RecentFiles.Remove(filePath);
            }
            Properties.Settings.Default.RecentFiles.Insert(0, filePath);

            // Update stable project list (no reorder, only add if new)
            if (!projectListFiles.Contains(filePath))
            {
                projectListFiles.Insert(0, filePath);
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Removes a project from both lists by file path
        /// </summary>
        public void RemoveProject(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            Properties.Settings.Default.RecentFiles.Remove(filePath);
            projectListFiles.Remove(filePath);
            Properties.Settings.Default.Save();
        }

        internal void RemoveProjectByIndex(int index)
        {
            if (index < 0 || index >= projectListFiles.Count) return;

            var filePath = projectListFiles[index];
            RemoveProject(filePath);
        }

        /// <summary>
        /// Removes missing files from both lists
        /// </summary>
        public void RemoveMissingFiles(IEnumerable<string> missingFiles)
        {
            if (missingFiles == null) return;

            foreach (var file in missingFiles)
            {
                Properties.Settings.Default.RecentFiles.Remove(file);
                projectListFiles.Remove(file);
            }

            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Gets the stable project list as file paths
        /// </summary>
        public List<string> GetProjectFiles()
        {
            return projectListFiles.ToList();
        }

        /// <summary>
        /// Removes non-existing files from both lists asynchronously
        /// </summary>
        public async Task CleanMissingFilesAsync()
        {
            var snapshot = projectListFiles.ToList();
            var missingFiles = await Task.Run(() => CheckForMissingFiles(snapshot)).ConfigureAwait(false);

            if (missingFiles.Count == 0) return;

            RemoveMissingFiles(missingFiles);
        }

        /// <summary>
        /// Checks which files from the provided list don't exist or are inaccessible
        /// </summary>
        public static List<string> CheckForMissingFiles(IEnumerable<string> files)
        {
            var missingFiles = new List<string>();
            if (files == null) return missingFiles;

            foreach (var f in files)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(f) || !File.Exists(f))
                        missingFiles.Add(f);
                }
                catch
                {
                    // Treat IO errors as missing; keep scanning
                    missingFiles.Add(f);
                }
            }

            return missingFiles;
        }
    }
}