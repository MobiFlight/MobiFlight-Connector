using System;
using System.IO;
using System.Linq;

namespace MobiFlight.Joysticks
{
    /// <summary>
    /// Handles migration of joystick definition files from old folder structure to new nested structure.
    /// </summary>
    internal static class JoystickDefinitionMigrator
    {
        /// <summary>
        /// Remove duplicate definition files from old, flat folder structure.
        /// </summary>
        public static void MigrateDefinitions()
        {
            try
            {
                var oldFiles = Directory.GetFiles("Joysticks", "*.joystick.json", SearchOption.TopDirectoryOnly);
                
                foreach (var oldFile in oldFiles)
                {
                    ProcessOldDefinitionFile(oldFile);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error during definition migration: {ex.Message}", LogSeverity.Error);
            }
        }

        /// <summary>
        /// Process a single old definition file for migration.
        /// <param name="oldFile">Path to the old definition file</param>
        /// </summary>
        private static void ProcessOldDefinitionFile(string oldFile)
        {
            try
            {
                var fileName = Path.GetFileName(oldFile);
                var allFilesWithSameName = Directory.GetFiles("Joysticks", fileName, SearchOption.AllDirectories);
                var duplicateFiles = allFilesWithSameName.Where(f => f != oldFile).ToArray();
                
                if (duplicateFiles.Length == 0)
                {
                    // If only one file exists, we want to keep it (the old one)
                    // this might be a definition file that a user created and
                    // which does not ship with MobiFlight
                    KeepOldFile(oldFile);
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
                    BackupConflictedFile(oldFile);
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
        /// <param name="oldFile"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void KeepOldFile(string oldFile)
        {
            try
            {
                var migratedFolder = Path.Combine("Joysticks", "_MIGRATED_");
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
        private static void BackupConflictedFile(string oldFile)
        {
            try
            {
                var conflictFolder = Path.Combine("Joysticks", "_CONFLICT_");
                Directory.CreateDirectory(conflictFolder); // Creates if doesn't exist

                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                var baseFileName = Path.GetFileNameWithoutExtension(oldFile);
                var backupFileName = $"{baseFileName}_{timestamp}.joystick.bak";
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