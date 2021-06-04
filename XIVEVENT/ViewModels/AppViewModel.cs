// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppViewModel.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.ViewModels {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    using NAudio.Wave;

    using Sharlayan.Core;

    using XIVEVENT.Audio;
    using XIVEVENT.Helpers;
    using XIVEVENT.Models;
    using XIVEVENT.Properties;
    using XIVEVENT.Utilities;

    public class AppViewModel : PropertyChangedBase {
        private static Lazy<AppViewModel> _instance = new Lazy<AppViewModel>(() => new AppViewModel());

        private string _appTitle;

        private ObservableCollection<AudioFile> _audioFiles;

        private string _cachePath;

        private ObservableCollection<ChatCode> _chatCodes;

        private string _configurationsPath;

        private CultureInfo _cultureInfo;

        private ObservableCollection<EventItem> _eventItems;

        private ObservableCollection<LanguageItem> _interfaceLanguages;

        private ConcurrentDictionary<string, string> _locale;

        private string _logsPath;

        private List<string> _savedLogsDirectoryList;

        private string _settingsPath;

        private ObservableCollection<DirectSoundDeviceInfo> _systemAudioDevices;

        private XDocument _xChatCodes;

        private XDocument _xEvents;

        public AppViewModel() {
            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "English",
                    ImageURI = "pack://application:,,,/Resources/EN.png",
                    Title = "English",
                    CultureInfo = new CultureInfo("en"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Japanese",
                    ImageURI = "pack://application:,,,/Resources/JA.png",
                    Title = "日本語",
                    CultureInfo = new CultureInfo("ja"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "French",
                    ImageURI = "pack://application:,,,/Resources/FR.png",
                    Title = "Français",
                    CultureInfo = new CultureInfo("fr"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "German",
                    ImageURI = "pack://application:,,,/Resources/DE.png",
                    Title = "Deutsch",
                    CultureInfo = new CultureInfo("de"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Chinese",
                    ImageURI = "pack://application:,,,/Resources/ZH.png",
                    Title = "中國",
                    CultureInfo = new CultureInfo("zh"),
                });

            this.InterfaceLanguages.Add(
                new LanguageItem {
                    Language = "Korean",
                    ImageURI = "pack://application:,,,/Resources/KO.png",
                    Title = "한국어",
                    CultureInfo = new CultureInfo("ko"),
                });
        }

        public ObservableCollection<AudioFile> AudioFiles {
            get => this._audioFiles ??= new ObservableCollection<AudioFile>();
            set => this.SetProperty(ref this._audioFiles, value);
        }

        public ConcurrentDictionary<string, CachedAudioFile> CachedAudioFiles { get; set; } = new ConcurrentDictionary<string, CachedAudioFile>();

        public static AppViewModel Instance => _instance.Value;

        public string AppTitle {
            get => this._appTitle;
            set {
                string appTitle = "XIVEVENT";
                string title = string.IsNullOrWhiteSpace(value)
                                   ? appTitle
                                   : $"{appTitle}: {value}";
                this.SetProperty(ref this._appTitle, title.ToUpperInvariant());
            }
        }

        public string CachePath {
            get => this._cachePath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._cachePath, value);
            }
        }

        public ObservableCollection<ChatCode> ChatCodes {
            get => this._chatCodes ??= new ObservableCollection<ChatCode>();
            set => this.SetProperty(ref this._chatCodes, value);
        }

        public ConcurrentDictionary<string, List<ChatLogItem>> ChatHistory { get; } = new ConcurrentDictionary<string, List<ChatLogItem>>();

        public string ConfigurationsPath {
            get => this._configurationsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._configurationsPath, value);
            }
        }

        public CultureInfo CultureInfo {
            get => this._cultureInfo ??= new CultureInfo("en");
            set => this.SetProperty(ref this._cultureInfo, value);
        }

        public ObservableCollection<LanguageItem> InterfaceLanguages {
            get => this._interfaceLanguages ??= new ObservableCollection<LanguageItem>();
            set => this.SetProperty(ref this._interfaceLanguages, value);
        }

        public ConcurrentDictionary<string, string> Locale {
            get => this._locale ??= new ConcurrentDictionary<string, string>();
            set => this.SetProperty(ref this._locale, value);
        }

        public ObservableCollection<EventItem> EventItems {
            get => this._eventItems ??= new ObservableCollection<EventItem>();
            set => this.SetProperty(ref this._eventItems, value);
        }

        public string LogsPath {
            get => this._logsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._logsPath, value);
            }
        }

        public List<string> SavedLogsDirectoryList {
            get => this._savedLogsDirectoryList ??= new List<string>();
            set {
                List<string> directoryPaths = value;
                foreach (string directoryPath in directoryPaths) {
                    string path = Path.Combine(this.LogsPath, directoryPath);
                    if (!Directory.Exists(path)) {
                        Directory.CreateDirectory(path);
                    }
                }

                this.SetProperty(ref this._savedLogsDirectoryList, value);
            }
        }

        public string SettingsPath {
            get => this._settingsPath;
            set {
                if (!Directory.Exists(value)) {
                    Directory.CreateDirectory(value);
                }

                this.SetProperty(ref this._settingsPath, value);
            }
        }

        public XDocument XChatCodes {
            get {
                if (this._xChatCodes is not null) {
                    return this._xChatCodes;
                }

                string path = Path.Combine(this.CachePath, "Configurations", "ChatCodes.xml");
                try {
                    this._xChatCodes = File.Exists(path)
                                           ? XDocument.Load(path)
                                           : ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatCodes.xml");
                }
                catch (Exception) {
                    this._xChatCodes = ResourceHelper.LoadXML($"{Constants.AppPack}Resources/ChatCodes.xml");
                }

                return this._xChatCodes;
            }
            set => this.SetProperty(ref this._xChatCodes, value);
        }

        public XDocument XEvents {
            get {
                if (this._xEvents is not null) {
                    return this._xEvents;
                }

                string path = Path.Combine(this.CachePath, "Settings", "Events.xml");
                try {
                    this._xEvents = File.Exists(path)
                                        ? XDocument.Load(path)
                                        : ResourceHelper.LoadXML($"{Constants.AppPack}Resources/Events.xml");
                }
                catch (Exception) {
                    this._xEvents = ResourceHelper.LoadXML($"{Constants.AppPack}Resources/Events.xml");
                }

                return this._xEvents;
            }
            set => this.SetProperty(ref this._xEvents, value);
        }

        public ObservableCollection<DirectSoundDeviceInfo> SystemAudioDevices {
            get => this._systemAudioDevices ??= new ObservableCollection<DirectSoundDeviceInfo>();
            set => this.SetProperty(ref this._systemAudioDevices, value);
        }

        public void RefreshAudioDevices() {
            this.SystemAudioDevices.Clear();

            List<DirectSoundDeviceInfo> systemAudioDevices = new List<DirectSoundDeviceInfo>(DirectSoundOut.Devices.Where(deviceInfo => deviceInfo.Guid != Guid.Empty));

            // Add "System Default" as first in the list
            this.SystemAudioDevices.Add(
                new DirectSoundDeviceInfo {
                    Description = "System Default",
                    Guid = Guid.Empty,
                    ModuleName = $"{{0.0.0.00000000}}.{{{Guid.Empty}}}",
                });
            // Add the rest of the values available
            foreach (DirectSoundDeviceInfo deviceInfo in systemAudioDevices) {
                this.SystemAudioDevices.Add(deviceInfo);
            }

            DirectSoundDeviceInfo selectedDevice = systemAudioDevices.FirstOrDefault(deviceInfo => deviceInfo.Guid.Equals(Settings.Default.AudioDevice));
            if (selectedDevice is null) {
                Settings.Default.AudioDevice = Guid.Empty;
                SoundSettingsViewModel.Instance.SelectedAudioDevice = this.SystemAudioDevices.First();
            }
            else {
                SoundSettingsViewModel.Instance.SelectedAudioDevice = selectedDevice;
            }
        }

        public void RefreshAudioCache() {
            this.AudioFiles.Clear();
            this.CachedAudioFiles.Clear();

            List<string> filters = new List<string> {
                "*.wav",
                "*.mp3",
            };

            string baseDirectory = Directory.GetCurrentDirectory();

            foreach (string filter in filters) {
                IEnumerable<FileInfo> filteredFiles = Directory.GetFiles(baseDirectory, filter, SearchOption.AllDirectories).Select(file => new FileInfo(file));
                foreach (FileInfo fileInfo in filteredFiles) {
                    this.AudioFiles.Add(
                        new AudioFile {
                            Name = fileInfo.Name,
                            Type = fileInfo.Extension.Trim('.').ToUpperInvariant(),
                            Size = FileUtilities.SizeSuffix(fileInfo.Length),
                        });

                    this.CachedAudioFiles.TryAdd(fileInfo.Name, new CachedAudioFile(fileInfo.FullName));
                }
            }
        }

        public ConcurrentDictionary<int, Process> GameInstances { get; set; } = new ConcurrentDictionary<int, Process>();
    }
}