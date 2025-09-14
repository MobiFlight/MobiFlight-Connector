using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace MobiFlight.Base
{
    public class Project
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler ProjectChanged;
        private string _name;
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

        public Project()
        {
            ConfigFiles.CollectionChanged += ConfigFiles_CollectionChanged;
            Name = "New MobiFlight Project";
        }

        private void ConfigFiles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnProjectChanged();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnProjectChanged()
        {
            ProjectChanged?.Invoke(this, EventArgs.Empty);
        }

        public void OpenFile()
        {
            if (IsJson(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                var project = JsonConvert.DeserializeObject<Project>(json);
                Name = project.Name;
                ConfigFiles = project.ConfigFiles;

                foreach (var configFile in ConfigFiles)
                {
                    if (!configFile.EmbedContent)
                    {
                        configFile.OpenFile();
                    }

                    if (configFile.Label==null)
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

                var configFile = new ConfigFile {
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

        public void SaveFile()
        {
            foreach (var configFile in ConfigFiles)
            {
                if (!configFile.EmbedContent && !configFile.ReferenceOnly)
                {
                    configFile.SaveFile();
                }
            }

            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }

        private static bool IsJson(string filePath)
        {
            var firstChar = File.ReadAllText(filePath).TrimStart()[0];
            return firstChar == '{' || firstChar == '[';
        }

        private static bool IsXml(string filePath)
        {
            var firstFewChars = File.ReadAllText(filePath).TrimStart().Substring(0, 5);
            return firstFewChars.StartsWith("<?xml");
        }

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

        public void Merge(Project project)
        {
            project.ConfigFiles.ToList().ForEach(file => ConfigFiles.Add(file));
        }

        public void MergeFromProjectFile(string fileName)
        {
            // take all config files and add them to the current project
            var additionalProject = new Project() { FilePath = fileName };
            additionalProject.OpenFile();
            Merge(additionalProject);
        }
    }
}