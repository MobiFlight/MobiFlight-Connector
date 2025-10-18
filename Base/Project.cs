using MobiFlight.Base.Migration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MobiFlight.Base
{
    /// <summary>
    /// Represents a MobiFlight project containing configuration files and project metadata.
    /// </summary>
    public class Project
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ProjectChanged;

        public readonly Version Version = new Version(1, 1);

        private string _name;
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    OnProjectChanged();
                }
            }
        }
        
        private string _filePath;
        /// <summary>
        /// Gets or sets the file path where the project is stored. This property is not serialized to JSON.
        /// </summary>
        [JsonIgnore]
        public string FilePath
        {
            get => _filePath;
            set
            {
                if (_filePath != value)
                {
                    _filePath = value;
                    OnPropertyChanged(nameof(FilePath));
                    OnProjectChanged();
                }
            }
        }

        private ObservableCollection<ConfigFile> _configFiles = new ObservableCollection<ConfigFile>();
        /// <summary>
        /// Gets or sets the collection of configuration files contained in this project.
        /// </summary>
        public ObservableCollection<ConfigFile> ConfigFiles
        {
            get => _configFiles;
            set
            {
                if (_configFiles != value)
                {
                    if (_configFiles != null)
                    {
                        _configFiles.CollectionChanged -= ConfigFiles_CollectionChanged;
                    }

                    _configFiles = value;

                    if (_configFiles != null)
                    {
                        _configFiles.CollectionChanged += ConfigFiles_CollectionChanged;
                    }

                    OnPropertyChanged(nameof(ConfigFiles));
                    OnProjectChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class with default values.
        /// </summary>
        public Project()
        {
            ConfigFiles.CollectionChanged += ConfigFiles_CollectionChanged;
            Name = "New MobiFlight Project";
        }

        /// <summary>
        /// Handles changes to the ConfigFiles collection and triggers the ProjectChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The collection change event arguments.</param>
        private void ConfigFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnProjectChanged();
        }

        /// <summary>
        /// Raises the PropertyChanged event for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Raises the ProjectChanged event to notify listeners that the project has been modified.
        /// </summary>
        protected virtual void OnProjectChanged()
        {
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Opens and loads a project from the file specified in the FilePath property.
        /// Supports both JSON (.mfproj) and legacy XML (.mcc, .aic) formats.
        /// </summary>
        /// <exception cref="InvalidDataException">Thrown when the file format is not supported.</exception>
        public void OpenFile()
        {
            if (IsJson(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                
                // Parse and migrate JSON document
                var document = JObject.Parse(json);
                var migratedDocument = ApplyMigrations(document);
                
                // Deserialize the clean, migrated JSON
                var project = migratedDocument.ToObject<Project>();
                
                Name = project.Name;
                ConfigFiles = project.ConfigFiles;

                foreach (var configFile in ConfigFiles)
                {
                    if (!configFile.EmbedContent)
                    {
                        configFile.OpenFile();
                    }

                    if (configFile.Label == null)
                    {
                        configFile.Label = Path.GetFileName(FilePath).Replace(".mfproj", "").Replace(".mcc", "");
                    }
                }
            }
            else if (IsXml(FilePath))
            {
                // Create a dummy project for old XML files
                var deprecatedConfigFile = ConfigFileFactory.CreateConfigFile(FilePath);
                deprecatedConfigFile.OpenFile();

                var configFile = new ConfigFile
                {
                    Label = Path.GetFileName(FilePath).Replace(".mfproj", "").Replace(".mcc", ""),
                    FileName = FilePath,
                    EmbedContent = true,
                    ReferenceOnly = false,
                    ConfigItems = deprecatedConfigFile.ConfigItems
                };

                Name = Path.GetFileNameWithoutExtension(FilePath);
                FilePath = FilePath;
                ConfigFiles.Add(configFile);
            }
            else
            {
                throw new InvalidDataException("Unsupported file format.");
            }
        }
        
        /// <summary>
        /// Apply all migrations to bring document to current version
        /// Simple, direct approach - no registry needed
        /// </summary>
        private JObject ApplyMigrations(JObject document)
        {
            // Determine current document version with safe parsing
            var currentVersion = GetDocumentVersion(document);
            
            if (currentVersion >= Version)
            {
                // No migration needed
                return document;
            }
            
            Log.Instance.log($"Migrating document from version {currentVersion} to {Version}", LogSeverity.Info);
            
            var migratedDocument = document;
            
            // Apply migrations step by step
            if (currentVersion < new Version(1, 1))
            {
                Log.Instance.log("Applying V1 → V1.1 migrations", LogSeverity.Debug);
                migratedDocument = Precondition_V1_1_Migration.Apply(migratedDocument);
            }

            // Update version in migrated document
            migratedDocument["_version"] = Version.ToString();

            Log.Instance.log($"Migration complete. Document is now version {Version}", LogSeverity.Info);

            return migratedDocument;
        }

        /// <summary>
        /// Safely parse the document version, defaulting to 1.0 if not present or invalid
        /// </summary>
        private Version GetDocumentVersion(JObject document)
        {
            try
            {
                var versionToken = document["_version"];
                if (versionToken == null)
                {
                    return new Version(1, 0); // Default for documents without version
                }

                var versionString = versionToken.ToString();
                if (string.IsNullOrEmpty(versionString))
                {
                    return new Version(1, 0);
                }

                // Try to parse as Version object
                if (Version.TryParse(versionString, out Version parsedVersion))
                {
                    return parsedVersion;
                }

                // If parsing fails, default to 1.0
                Log.Instance.log($"Could not parse version '{versionString}', defaulting to 1.0", LogSeverity.Warn);
                return new Version(1, 0);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error parsing document version: {ex.Message}, defaulting to 1.0", LogSeverity.Warn);
                return new Version(1, 0);
            }
        }

        /// <summary>
        /// Saves the project to the file specified in the FilePath property in JSON format.
        /// Also saves any non-embedded, non-reference-only configuration files.
        /// </summary>
        public void SaveFile()
        {
            foreach (var configFile in ConfigFiles)
            {
                if (!configFile.EmbedContent && !configFile.ReferenceOnly)
                {
                    configFile.SaveFile();
                }
            }

            // Add version when serializing
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            var document = JObject.Parse(json);
            document["_version"] = Version.ToString();
            
            File.WriteAllText(FilePath, document.ToString());
        }

        /// <summary>
        /// Determines whether the specified file contains JSON content by examining the first character.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns><c>true</c> if the file appears to contain JSON; otherwise, <c>false</c>.</returns>
        private static bool IsJson(string filePath)
        {
            var firstChar = File.ReadAllText(filePath).TrimStart()[0];
            return firstChar == '{' || firstChar == '[';
        }

        /// <summary>
        /// Determines whether the specified file contains XML content by examining the first few characters.
        /// </summary>
        /// <param name="filePath">The path to the file to check.</param>
        /// <returns><c>true</c> if the file appears to contain XML; otherwise, <c>false</c>.</returns>
        private static bool IsXml(string filePath)
        {
            var firstFewChars = File.ReadAllText(filePath).TrimStart().Substring(0, 5);
            return firstFewChars.StartsWith("<?xml");
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current project.
        /// Compares Name, FilePath, and all ConfigFiles for equality.
        /// </summary>
        /// <param name="obj">The object to compare with the current project.</param>
        /// <returns><c>true</c> if the specified object is equal to the current project; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Project)) return false;
            var other = obj as Project;

            if (this.ConfigFiles.Count != other.ConfigFiles.Count)
            {
                return false;
            }

            for (int i = 0; i < ConfigFiles.Count; i++)
            {
                if (!ConfigFiles[i].AreEqual(other.ConfigFiles[i]))
                {
                    return false;
                }
            }

            return
                this.Name.Equals(other.Name) &&
                this.FilePath.Equals(other.FilePath) &&
                this.ConfigFiles.SequenceEqual(other.ConfigFiles);
        }

        /// <summary>
        /// Merges all configuration files from the specified source project into this project.
        /// The source project and its ConfigFiles collection must not be null.
        /// </summary>
        /// <param name="project">The source project to merge from. If null or if ConfigFiles is null, no action is taken.</param>
        public void Merge(Project project)
        {
            if (project == null || project.ConfigFiles == null) return;

            project.ConfigFiles.ToList().ForEach(file => ConfigFiles.Add(file));
        }

        /// <summary>
        /// Loads a project from the specified file and merges all its configuration files into this project.
        /// This is a convenience method that combines loading and merging operations.
        /// </summary>
        /// <param name="fileName">The path to the project file to load and merge.</param>
        /// <exception cref="FileNotFoundException">Thrown when the specified file does not exist.</exception>
        /// <exception cref="InvalidDataException">Thrown when the file format is not supported.</exception>
        /// <exception cref="Newtonsoft.Json.JsonReaderException">Thrown when the JSON file is malformed.</exception>
        public void MergeFromProjectFile(string fileName)
        {
            // take all config files and add them to the current project
            var additionalProject = new Project() { FilePath = fileName };
            additionalProject.OpenFile();
            Merge(additionalProject);
        }
    }
}