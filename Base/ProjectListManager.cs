using MobiFlight.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MobiFlight.Base
{
    /// <summary>
    /// Manages both the MRU RecentFiles list and the stable ProjectList for UI display,
    /// with caching to avoid redundant disk I/O
    /// </summary>
    public class ProjectListManager
    {
        /// <summary>
        /// Stable list for UI - new projects added at top, existing items never reordered
        /// </summary>
        private List<string> projectListFiles = new List<string>();

        /// <summary>
        /// Cache of ProjectInfo objects to avoid redundant disk I/O
        /// </summary>
        private readonly Dictionary<string, ProjectInfo> projectInfoCache = new Dictionary<string, ProjectInfo>();

        /// <summary>
        /// Event raised when the project list changes
        /// </summary>
        public event EventHandler ProjectListChanged;

        /// <summary>
        /// Initializes the project list from settings, cleans missing files, and loads project infos into cache
        /// </summary>
        public async Task InitializeFromSettingsAsync(ControllerBindingService controllerBindingService = null)
        {
            projectListFiles = Properties.Settings.Default.RecentFiles?.Cast<string>().ToList() ?? new List<string>();
            projectInfoCache.Clear();

            // Clean missing files
            try
            {
                await CleanMissingFilesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Exception cleaning project files: {ex.Message}", LogSeverity.Error);
            }

            try
            {
                // Load all project infos in parallel to populate cache
                await LoadAllProjectInfosAsync(controllerBindingService).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Exception during fetching project info for project list. {ex.Message}", LogSeverity.Error);
            }

            // Notify that initialization is complete
            ProjectListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Loads all project files and populates the cache in parallel
        /// </summary>
        private async Task LoadAllProjectInfosAsync(ControllerBindingService controllerBindingService)
        {
            if (projectListFiles.Count == 0) return;

            foreach (var projectPath in projectListFiles)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        var p = new Project();
                        p.FilePath = projectPath;
                        p.OpenFile(suppressMigrationLogging: true);
                        p.DetermineProjectInfos();
                        controllerBindingService?.PerformAutoBinding(p);

                        var projectInfo = p.ToProjectInfo();
                        projectInfoCache[projectPath] = projectInfo;
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.log($"Could not load recent project file {projectPath}: {ex.Message}", LogSeverity.Warn);
                    }
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Adds a project to both the MRU RecentFiles and the stable project list
        /// </summary>
        public void OpenProject(ProjectInfo projectInfo)
        {
            var filePath = projectInfo?.FilePath;

            if (string.IsNullOrEmpty(filePath?.Trim())) return;

            // Update cache with the loaded project
            projectInfoCache[filePath] = projectInfo;

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

            ProjectListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the cached ProjectInfo for a specific project
        /// </summary>
        public void UpdateProjectInfo(ProjectInfo projectInfo)
        {
            if (projectInfo == null || string.IsNullOrEmpty(projectInfo.FilePath)) return;

            projectInfoCache[projectInfo.FilePath] = projectInfo;

            ProjectListChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the stable project list as ProjectInfo objects (from cache)
        /// </summary>
        public List<ProjectInfo> GetProjects()
        {
            var result = new List<ProjectInfo>();

            foreach (var filePath in projectListFiles)
            {
                if (projectInfoCache.TryGetValue(filePath, out var projectInfo))
                {
                    result.Add(projectInfo);
                }
            }

            return result;
        }

        /// <summary>
        /// Removes a project from both lists and cache by file path
        /// </summary>
        public void RemoveProject(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            Properties.Settings.Default.RecentFiles.Remove(filePath);
            projectListFiles.Remove(filePath);
            projectInfoCache.Remove(filePath);
            Properties.Settings.Default.Save();

            ProjectListChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void RemoveProjectByIndex(int index)
        {
            if (index < 0 || index >= projectListFiles.Count) return;

            var filePath = projectListFiles[index];
            RemoveProject(filePath);
        }

        /// <summary>
        /// Removes missing files from both lists and cache
        /// </summary>
        public void RemoveMissingFiles(IEnumerable<string> missingFiles)
        {
            if (missingFiles == null) return;

            bool changed = false;
            foreach (var file in missingFiles)
            {
                Properties.Settings.Default.RecentFiles.Remove(file);
                projectListFiles.Remove(file);
                projectInfoCache.Remove(file);
                changed = true;
            }

            if (changed)
            {
                Properties.Settings.Default.Save();
                ProjectListChanged?.Invoke(this, EventArgs.Empty);
            }
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