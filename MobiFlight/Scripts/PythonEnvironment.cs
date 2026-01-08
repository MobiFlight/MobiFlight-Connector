using System;
using System.IO;
using System.IO.Compression;

namespace MobiFlight.Scripts
{
    /// <summary>
    /// Manages the Python runtime environment initialization
    /// </summary>
    internal class PythonEnvironment
    {
        public const string PYTHON_BASE_FOLDER = "Python";
        public const string PYTHON_RUNTIME_VERSION = "3.14.2";
        public static string PathPythonExecutable;

        /// <summary>
        /// Initializes the Python runtime by extracting archived files if needed
        /// <param name="baseDirectory">Optional base directory for testing. If null, uses AppDomain.CurrentDomain.BaseDirectory</param>
        /// </summary>
        public static void Initialize(string baseDirectory = null)
        {
            if (baseDirectory == null)
            {
                baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            }

            string pythonBaseFolder = Path.Combine(baseDirectory, PYTHON_BASE_FOLDER);
            string pythonRuntimeFolder = Path.Combine(pythonBaseFolder, PYTHON_RUNTIME_VERSION);
            PathPythonExecutable = Path.Combine(pythonRuntimeFolder, "python.exe");

            // Check if Python runtime does already exist
            if (File.Exists(PathPythonExecutable))
            {
                Log.Instance.log("Python runtime already initialized.", LogSeverity.Debug);
                return;
            }

            // Ensure Python base folder exists
            if (!Directory.Exists(pythonBaseFolder))
            { 
                Log.Instance.log($"Python folder not found: {pythonBaseFolder}", LogSeverity.Error);
                return;
            }

            // Delete all existing folders in Python base folder before extraction
            string[] existingFolders = Directory.GetDirectories(pythonBaseFolder);
            foreach (string folder in existingFolders)
            {
                try
                {
                    Log.Instance.log($"Deleting existing Python folder: {Path.GetFileName(folder)}", LogSeverity.Info);
                    Directory.Delete(folder, true);
                    Log.Instance.log($"Successfully deleted: {Path.GetFileName(folder)}", LogSeverity.Debug);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to delete folder {Path.GetFileName(folder)}: {ex.Message}", LogSeverity.Error);
                }
            }

            // Find all .zip files in Python folder
            string[] zipFiles = Directory.GetFiles(pythonBaseFolder, "*.zip", SearchOption.TopDirectoryOnly);

            if (zipFiles.Length == 0)
            {
                Log.Instance.log("No Python runtime .zip files found to extract.", LogSeverity.Error);         
                return;
            }

            Log.Instance.log($"Found {zipFiles.Length} Python runtime archive(s) to extract.", LogSeverity.Info);

            // Extract all .zip files
            foreach (string zipFile in zipFiles)
            {
                try
                {
                    Log.Instance.log($"Extracting Python runtime: {Path.GetFileName(zipFile)}", LogSeverity.Info);
                    ZipFile.ExtractToDirectory(zipFile, pythonBaseFolder);
                    Log.Instance.log($"Successfully extracted: {Path.GetFileName(zipFile)}", LogSeverity.Info);
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Failed to extract {Path.GetFileName(zipFile)}: {ex.Message}", LogSeverity.Error);
                }
            }

            // Verify extraction was successful
            if (File.Exists(PathPythonExecutable))
            {
                Log.Instance.log("Python runtime successfully initialized.", LogSeverity.Info);
            }
            else
            {
                Log.Instance.log("Python runtime folder not found after extraction. Archive may not contain expected structure.", LogSeverity.Warn);
            }
        }
    }
}

