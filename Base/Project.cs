using MobiFlight.Base.Migration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MobiFlight.Base
{
    /// <summary>
    /// Lightweight project metadata used for listings, previews or indexing.
    /// Can be loaded from a full project file (it ignores unknown properties).
    /// </summary>
    public class ProjectInfo
    {
        public string Name { get; set; }
        public string Sim { get; set; }
        public ProjectFeatures Features { get; set; }
        public List<string> Aircraft { get; set; }
        public List<string> Controllers { get; set; }
        public string FilePath { get; set; }
        public bool Favorite { get; set; } = false;
    }


    /// <summary>
    /// Represents optional features that can be enabled for a MobiFlight project.
    /// </summary>
    public class ProjectFeatures : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _useFsuipc = false;
        /// <summary>
        /// Gets or sets whether the project uses FSUIPC integration.
        /// </summary>
        public bool FSUIPC
        {
            get => _useFsuipc;
            set
            {
                if (_useFsuipc != value)
                {
                    _useFsuipc = value;
                    OnPropertyChanged(nameof(FSUIPC));
                }
            }
        }

        private bool _useProSim = false;
        /// <summary>
        /// Gets or sets whether the project uses ProSim integration.
        /// </summary>
        public bool ProSim
        {
            get => _useProSim;
            set
            {
                if (_useProSim != value)
                {
                    _useProSim = value;
                    OnPropertyChanged(nameof(ProSim));
                }
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Creates a deep copy of the current features.
        /// </summary>
        public ProjectFeatures Clone()
        {
            return new ProjectFeatures
            {
                FSUIPC = this.FSUIPC,
                ProSim = this.ProSim
            };
        }
    }

    /// <summary>
    /// Represents a MobiFlight project containing configuration files and project metadata.
    /// </summary>
    public class Project
    {
        public const string FileExtension = ".mfproj";
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ProjectChanged;

        [JsonIgnore]
        public readonly Version SchemaVersion = new Version(0, 9);
        [JsonIgnore]
        public Version OriginalSchemaVersion { get; private set; } = null;

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
                        _configFiles.CollectionChanged -= CollectionChanged;
                    }

                    _configFiles = value;

                    if (_configFiles != null)
                    {
                        _configFiles.CollectionChanged += CollectionChanged;
                    }

                    OnPropertyChanged(nameof(ConfigFiles));
                    OnProjectChanged();
                }
            }
        }

        private string _sim;
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Sim
        {
            get => _sim;
            set
            {
                if (_sim != value)
                {
                    _sim = value;
                    OnPropertyChanged(nameof(Sim));
                    OnProjectChanged();
                }
            }
        }

        private ObservableCollection<string> _aircraft = new ObservableCollection<string>();
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public ObservableCollection<string> Aircraft
        {
            get => _aircraft;
            set
            {
                if (_aircraft != value)
                {
                    if (_aircraft != null)
                    {
                        _aircraft.CollectionChanged -= CollectionChanged;
                    }

                    _aircraft = value;

                    if (_aircraft != null)
                    {
                        _aircraft.CollectionChanged += CollectionChanged;
                    }

                    OnPropertyChanged(nameof(Aircraft));
                    OnProjectChanged();
                }
            }
        }

        private ObservableCollection<string> _controllers = new ObservableCollection<string>();
        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public ObservableCollection<string> Controllers
        {
            get => _controllers;
            set
            {
                if (_aircraft != value)
                {
                    if (_controllers != null)
                    {
                        _controllers.CollectionChanged -= CollectionChanged;
                    }

                    _controllers = value;

                    if (_controllers != null)
                    {
                        _controllers.CollectionChanged += CollectionChanged;
                    }

                    OnPropertyChanged(nameof(Controllers));
                    OnProjectChanged();
                }
            }
        }

        private ProjectFeatures _features = new ProjectFeatures();

        /// <summary>
        /// Gets or sets optional features enabled for this project.
        /// </summary>
        public ProjectFeatures Features
        {
            get => _features;
            set
            {
                if (_features != value)
                {
                    // Unsubscribe from old instance
                    if (_features != null)
                    {
                        _features.PropertyChanged -= Features_PropertyChanged;
                    }

                    _features = value;

                    // Subscribe to new instance
                    if (_features != null)
                    {
                        _features.PropertyChanged += Features_PropertyChanged;
                    }

                    OnPropertyChanged(nameof(Features));
                    OnProjectChanged();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class with default values.
        /// </summary>
        public Project()
        {
            ConfigFiles.CollectionChanged += CollectionChanged;
            Name = "New MobiFlight Project";
        }

        public ProjectInfo ToProjectInfo()
        {
            if (string.IsNullOrEmpty(Sim))
            {
                DetermineProjectInfos();
            }

            var projectInfo = new ProjectInfo()
            {
                Name = Name,
                Sim = Sim,
                Features = Features,
                Aircraft = Aircraft?.ToList() ?? new List<string>(),
                Controllers = Controllers?.ToList() ?? new List<string>(),
                FilePath = FilePath
            };

            return projectInfo;
        }

        public void DetermineProjectInfos()
        {
            var controllerSerials = new List<string>();

            // reset features for clean determination
            Features = new ProjectFeatures();

            foreach (var item in ConfigFiles)
            {
                if (string.IsNullOrEmpty(Sim))
                {
                    var sim = item.DetermineSim();

                    // set it only once for now
                    if (Sim == null && sim != null)
                    {
                        Sim = sim;
                    }
                }

                Features.FSUIPC |= item.DetermineUsingFsuipc();
                Features.ProSim |= item.ContainsConfigOfSourceType(new ProSimSource());

                item.GetIUniqueControllerSerials().ForEach(c => controllerSerials.Add(c));
            }

            Controllers.Clear();

            controllerSerials.Distinct().ToList().ForEach(c =>
            {
                Controllers.Add(c);
            });
        }

        /// <summary>
        /// Handles changes to the ConfigFiles collection and triggers the ProjectChanged event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The collection change event arguments.</param>
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Features));
            OnProjectChanged();
        }

        // Handle property changes within Features
        private void Features_PropertyChanged(object sender, PropertyChangedEventArgs e)
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
        public void OpenFile(bool suppressMigrationLogging = false)
        {
            if (IsJson(FilePath))
            {
                var json = File.ReadAllText(FilePath);

                // Parse and migrate JSON document
                var document = JObject.Parse(json);
                var migratedDocument = ApplyMigrations(document, suppressMigrationLogging);

                // Deserialize the clean, migrated JSON
                var project = migratedDocument.ToObject<Project>();
                if (project == null)
                {
                    Log.Instance.log("Project could not be loaded", LogSeverity.Error);
                    throw new InvalidDataException("Failed to deserialize project file.");
                }
                this.CopyFrom(project);

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

        private void CopyFrom(Project project)
        {
            this.Name = project.Name;
            this.Sim = project.Sim;
            this.Features = project.Features.Clone();
            this.Aircraft = project.Aircraft;
            this.ConfigFiles = project.ConfigFiles;
        }

        /// <summary>
        /// Apply all migrations to bring document to current version
        /// Simple, direct approach - no registry needed
        /// </summary>
        private JObject ApplyMigrations(JObject document, bool suppressLogging = false)
        {
            // Determine current document version with safe parsing
            var currentVersion = GetDocumentSchemaVersion(document);
            OriginalSchemaVersion = currentVersion;

            if (currentVersion > SchemaVersion)
            {
                if (!suppressLogging) { 
                    Log.Instance.log($"Document version {currentVersion} too new. Update MobiFlight to latest version. ({Name} - {FilePath})", LogSeverity.Info);
                }

                return document;
            }

            if (currentVersion == SchemaVersion)
            {
                // No migration needed
                return document;
            }

            if (!suppressLogging) { 
                Log.Instance.log($"Migrating document from version {currentVersion} to {SchemaVersion}. ({Name} - {FilePath})", LogSeverity.Debug);
            }

            var migratedDocument = document;

            // Apply migrations step by step
            if (currentVersion < new Version(0, 9))
            {
                if (!suppressLogging)
                {
                    Log.Instance.log("Applying V0.9 migrations", LogSeverity.Debug);
                }
                migratedDocument = Precondition_V_0_9_Migration.Apply(migratedDocument);
                migratedDocument = Output_V_0_9_Migration.Apply(migratedDocument);
            }

            // Update version in migrated document
            migratedDocument["_version"] = SchemaVersion.ToString();

            if (!suppressLogging)
            {
                Log.Instance.log($"Migration complete. Document is now version {SchemaVersion}", LogSeverity.Debug);
            }

            return migratedDocument;
        }

        /// <summary>
        /// Safely parse the document version, defaulting to 0.1 if not present or invalid
        /// </summary>
        private Version GetDocumentSchemaVersion(JObject document)
        {
            try
            {
                var versionToken = document["_version"];
                if (versionToken == null)
                {
                    return new Version(0, 1); // Default for documents without version
                }

                var versionString = versionToken.ToString();
                if (string.IsNullOrEmpty(versionString))
                {
                    return new Version(0, 1);
                }

                // Try to parse as Version object
                if (Version.TryParse(versionString, out Version parsedVersion))
                {
                    return parsedVersion;
                }

                // If parsing fails, default to 0.1
                Log.Instance.log($"Could not parse version '{versionString}', defaulting to 0.1", LogSeverity.Debug);
                return new Version(0, 1);
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error parsing document version: {ex.Message}, defaulting to 0.1", LogSeverity.Debug);
                return new Version(0, 1);
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
            var document = JObject.FromObject(this);
            document["_version"] = SchemaVersion.ToString();

            // we don't want to serialize the FilePath
            document.Property("FilePath").Remove();

            File.WriteAllText(FilePath, document.ToString(Formatting.Indented));
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

        public bool ContainsConfigOfSourceType(Source type)
        {
            return ConfigFiles.ToList().Any(file => file.ContainsConfigOfSourceType(type));
        }

        public string MigrateFileExtension()
        {
            if (FilePath.EndsWith(".mcc", StringComparison.OrdinalIgnoreCase) || 
                FilePath.EndsWith(".aic", StringComparison.OrdinalIgnoreCase))
            {
                FilePath = Path.ChangeExtension(FilePath, FileExtension);
            }

            return FilePath;
        }
    }
}