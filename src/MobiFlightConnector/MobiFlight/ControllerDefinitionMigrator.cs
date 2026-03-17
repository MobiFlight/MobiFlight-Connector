using System;
using System.IO;
using System.Linq;

namespace MobiFlight.Joysticks
{
    /// <summary>
    /// Handles migration of joystick definition files from old folder structure to new nested structure.
    /// </summary>
    internal static class ControllerDefinitionMigrator
    {
        /// <summary>
        /// Remove duplicate definition files from old, flat folder structure.
        /// </summary>
        public static void MigrateJoysticks()
        {
            MigrateDefinitions("Joysticks", "*.joystick.json");
        }

        /// <summary>
        /// Remove duplicate definition files from old, flat folder structure.
        /// </summary>
        public static void MigrateMidiControllers()
        {
            MigrateDefinitions("MidiBoards", "*.midiboard.json");
        }

        /// <summary>
        /// Remove duplicate definition files from old, flat folder structure.
        /// This overload allows specifying a custom base folder for testing.
        /// </summary>
        /// <param name="baseFolder">The base folder containing the definition files</param>
        internal static void MigrateDefinitions(string baseFolder, string pattern)
        {
            try
            {
                if (!Directory.Exists(baseFolder))
                {
                    return; // Nothing to migrate if folder doesn't exist
                }

                var oldFiles = Directory.GetFiles(baseFolder, pattern, SearchOption.TopDirectoryOnly);

                foreach (var oldFile in oldFiles)
                {
                    ProcessOldDefinitionFile(oldFile, baseFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error during definition migration: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Process a single old definition file for migration.
        /// </summary>
        /// <param name="oldFile">Path to the old definition file</param>
        /// <param name="baseFolder">The base folder containing the definition files</param>
        private static void ProcessOldDefinitionFile(string oldFile, string baseFolder)
        {
            try
            {
                // Skip read-only files - they should not be migrated
                if ((File.GetAttributes(oldFile) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    Log.Instance.log($"Skipping read-only file {oldFile} - will not be migrated", LogSeverity.Info);
                    return;
                }

                var fileName = Path.GetFileName(oldFile);
                var allFilesWithSameName = Directory.GetFiles(baseFolder, fileName, SearchOption.AllDirectories);
                var duplicateFiles = allFilesWithSameName.Where(f => f != oldFile).ToArray();
                
                if (duplicateFiles.Length == 0)
                {
                    // If only one file exists, we want to keep it (the old one)
                    // this might be a definition file that a user created and
                    // which does not ship with MobiFlight
                    KeepOldFile(oldFile, baseFolder);
                    return; // No actual duplicates found
                }

                var newFile = duplicateFiles.First();
                
                if (AreFilesIdentical(oldFile, newFile))
                {
                    DeleteOldFile(oldFile);
                }
                else
                {
                    // backup the conflicted file
                    BackupConflictedFile(oldFile, baseFolder);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error processing old definition file {oldFile}: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Keeps the old file and moves it to a "_migrated_" sub folder
        /// </summary>
        /// <param name="oldFile">Path to the old file</param>
        /// <param name="baseFolder">The base folder containing the definition files</param>
        private static void KeepOldFile(string oldFile, string baseFolder)
        {
            try
            {
                var migratedFolder = Path.Combine(baseFolder, "_MIGRATED_");
                Directory.CreateDirectory(migratedFolder); 
                var baseFileName = Path.GetFileName(oldFile);
                var migrationFilePath = Path.Combine(migratedFolder, baseFileName);
                File.Move(oldFile, migrationFilePath);
                Log.Instance.log($"Migrated unique joystick definition file {oldFile} to {migrationFilePath}", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to migrate old file {oldFile}: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Check if two files have identical content.
        /// </summary>
        /// <param name="file1">First file path</param>
        /// <param name="file2">Second file path</param>
        /// <returns>True if files are identical</returns>
        private static bool AreFilesIdentical(string file1, string file2)
        {
            try
            {
                var content1 = File.ReadAllText(file1);
                var content2 = File.ReadAllText(file2);
                return content1 == content2;
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error comparing files {file1} and {file2}: {ex.Message}", LogSeverity.Error);
                return false; // Assume different if we can't read them
            }
        }

        /// <summary>
        /// Delete an old definition file that's identical to the new one.
        /// </summary>
        /// <param name="oldFile">Path to the old file</param>
        private static void DeleteOldFile(string oldFile)
        {
            try
            {
                File.Delete(oldFile);
                Log.Instance.log($"Removed duplicate joystick definition file {oldFile}", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to delete old file {oldFile}: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Backup an old definition file that differs from the new one.
        /// </summary>
        /// <param name="oldFile">Path to the old file</param>
        /// <param name="baseFolder">The base folder containing the definition files</param>
        private static void BackupConflictedFile(string oldFile, string baseFolder)
        {
            try
            {
                var conflictFolder = Path.Combine(baseFolder, "_CONFLICT_");
                Directory.CreateDirectory(conflictFolder); // Creates if doesn't exist

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var fileName = Path.GetFileName(oldFile);
                var backupFileName = $"{fileName}_{timestamp}.bak";
                var backupFilePath = Path.Combine(conflictFolder, backupFileName);

                File.Move(oldFile, backupFilePath);
                Log.Instance.log($"Old joystick definition file {oldFile} was moved to {backupFilePath}", LogSeverity.Info);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Failed to backup old file {oldFile}: {ex.Message}", LogSeverity.Error);
            }
        }
    }
}