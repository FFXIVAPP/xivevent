﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppContext.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AppContext.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    using MaterialDesignColors;

    using MaterialDesignThemes.Wpf;

    using NLog;

    using Sharlayan;
    using Sharlayan.Models;

    using XIVEVENT.Controls;
    using XIVEVENT.Helpers;
    using XIVEVENT.Models;
    using XIVEVENT.Properties;
    using XIVEVENT.SharlayanWrappers;
    using XIVEVENT.Utilities;
    using XIVEVENT.ViewModels;

    public class AppContext {
        private static Lazy<AppContext> _instance = new Lazy<AppContext>(() => new AppContext());

        private readonly ConcurrentDictionary<int, WorkerSet> _workerSets = new ConcurrentDictionary<int, WorkerSet>();

        public static AppContext Instance => _instance.Value;

        public void Initialize() {
            this.SetupCurrentUICulture();
            this.SetupDirectories();
            this.ApplyTheme();
            this.LoadChatCodes();
            this.SetupAudio();
            this.LoadEvents();
            this.FindGameInstances();
            this.SetupSharlayanManager();
            this.SetupWorkerSets();
            this.StartAllSharlayanWorkers();
        }

        private void LoadEvents() {
            AppViewModel.Instance.EventItems.Clear();

            foreach (XElement xElement in AppViewModel.Instance.XEvents.Descendants().Elements("Event")) {
                Guid xKey = xElement.GetAttributeValue("Key", Guid.NewGuid());
                bool xIsEnabled = xElement.GetElementValue("IsEnabled", false);
                string xRegEx = xElement.GetElementValue("RegEx", string.Empty);
                string xCategory = xElement.GetElementValue("Category", string.Empty);
                string xSound = xElement.GetElementValue("Sound", string.Empty);
                float xVolume = xElement.GetElementValue("Volume", 100.0f);
                float xSpeechRate = xElement.GetElementValue("SpeechRate", -2.0f);
                string xTTSMessage = xElement.GetElementValue("TTSMessage", string.Empty);
                string xExecutablePath = xElement.GetElementValue("ExecutablePath", string.Empty);
                string xExecutableArguments = xElement.GetElementValue("ExecutableArguments", string.Empty);

                if (string.IsNullOrWhiteSpace(xRegEx)) {
                    continue;
                }

                AppViewModel.Instance.EventItems.Add(
                    new EventItem {
                        Key = xKey,
                        Category = xCategory,
                        ExecutableArguments = xExecutableArguments,
                        ExecutablePath = xExecutablePath,
                        IsEnabled = xIsEnabled,
                        RegEx = xRegEx,
                        Sound = xSound,
                        SpeechRate = xSpeechRate,
                        TTSMessage = xTTSMessage,
                        Volume = xVolume,
                    });
            }
        }

        private void SetupAudio() {
            AppViewModel.Instance.RefreshAudioCacheDirectories();
            AppViewModel.Instance.RefreshAudioCache();
            AppViewModel.Instance.RefreshAudioDevices();
        }

        private void ApplyTheme() {
            ThemeUtilities.ModifyTheme(
                theme => theme.SetBaseTheme(
                    Settings.Default.DarkMode
                        ? Theme.Dark
                        : Theme.Light));
            SwatchesProvider swatchesProvider = new SwatchesProvider();
            Swatch primaryColor = swatchesProvider.Swatches.FirstOrDefault(a => string.Equals(a.Name, Settings.Default.UserThemePrimary, StringComparison.OrdinalIgnoreCase));
            if (primaryColor is not null) {
                ThemeUtilities.ModifyTheme(theme => theme.SetPrimaryColor(primaryColor.ExemplarHue.Color));
            }

            Swatch accentColor = swatchesProvider.Swatches.FirstOrDefault(a => string.Equals(a.Name, Settings.Default.UserThemeAccent, StringComparison.OrdinalIgnoreCase));
            if (accentColor is { AccentExemplarHue : not null, }) {
                ThemeUtilities.ModifyTheme(theme => theme.SetSecondaryColor(accentColor.AccentExemplarHue.Color));
            }
        }

        private void FindGameInstances() {
            AppViewModel.Instance.GameInstances.Clear();
            foreach (Process process in Process.GetProcessesByName("ffxiv_dx11")) {
                AppViewModel.Instance.GameInstances.TryAdd(process.Id, process);
            }
        }

        private void LoadChatCodes() {
            foreach (XElement xElement in AppViewModel.Instance.XChatCodes.Descendants().Elements("Code")) {
                string xKey = xElement.Attribute("Key")?.Value;
                string xColor = xElement.Element("Color")?.Value ?? "FFFFFF";
                string xDescription = xElement.Element("Description")?.Value ?? "Unknown";

                if (string.IsNullOrWhiteSpace(xKey)) {
                    continue;
                }

                AppViewModel.Instance.ChatCodes.Add(new ChatCode(xKey, xColor, xDescription));
            }
        }

        private void MemoryHandler_OnExceptionEvent(object sender, Logger logger, Exception ex) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            // TODO: this should be handled in sharlayan; when we can detect character changes this will be updated/removed and placed in sharlayan
            if (ex.GetType() != typeof(OverflowException)) {
                return;
            }

            if (ex.StackTrace is null || !ex.StackTrace.Contains("ChatLogReader")) {
                return;
            }

            SharlayanConfiguration configuration = memoryHandler.Configuration;

            if (!this._workerSets.TryGetValue(configuration.ProcessModel.ProcessID, out WorkerSet workerSet)) {
                return;
            }

            workerSet.ChatLogWorker.StopScanning();

            Task.Run(
                async () => {
                    await Task.Delay(1000);
                    workerSet.ChatLogWorker.Reset();
                    workerSet.ChatLogWorker.StartScanning();
                });
        }

        private void MemoryHandler_OnMemoryHandlerDisposedEvent(object sender) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            memoryHandler.OnException -= this.MemoryHandler_OnExceptionEvent;
            memoryHandler.OnMemoryHandlerDisposed -= this.MemoryHandler_OnMemoryHandlerDisposedEvent;
            memoryHandler.OnMemoryLocationsFound -= this.MemoryHandler_OnMemoryLocationsFoundEvent;

            if (this._workerSets.TryRemove(memoryHandler.Configuration.ProcessModel.ProcessID, out WorkerSet workerSet)) {
                workerSet.StopMemoryWorkers();
            }

            AppViewModel.Instance.GameInstances.TryRemove(memoryHandler.Configuration.ProcessModel.ProcessID, out Process process);
        }

        private void MemoryHandler_OnMemoryLocationsFoundEvent(object sender, ConcurrentDictionary<string, MemoryLocation> memoryLocations, long processingTime) {
            if (sender is not MemoryHandler memoryHandler) {
                return;
            }

            foreach ((string key, MemoryLocation memoryLocation) in memoryLocations) {
                FlowDocHelper.AppendMessage(memoryHandler, $"MemoryLocation Found -> {key} => {memoryLocation.GetAddress():X}", DebugTabItem.Instance.DebugLogReader._FDR);
            }
        }

        private void SetupCurrentUICulture() {
            string cultureInfo = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            CultureInfo currentCulture = new CultureInfo(cultureInfo);
            AppViewModel.Instance.CultureInfo = Settings.Default.CultureSet
                                                    ? Settings.Default.Culture
                                                    : currentCulture;
            Settings.Default.CultureSet = true;
        }

        private void SetupDirectories() {
            AppViewModel.Instance.CachePath = Constants.CachePath;
            AppViewModel.Instance.ConfigurationsPath = Constants.ConfigurationsPath;
            AppViewModel.Instance.LogsPath = Constants.LogsPath;
            AppViewModel.Instance.SettingsPath = Constants.SettingsPath;
            AppViewModel.Instance.SavedLogsDirectoryList = new List<string> {
                "Say",
                "Shout",
                "Party",
                "Tell",
                "LS",
                "CWLS",
                "FC",
                "Yell",
            };
        }

        private void SetupSharlayanManager() {
            foreach ((int _, Process process) in AppViewModel.Instance.GameInstances) {
                SharlayanConfiguration sharlayanConfiguration = new SharlayanConfiguration {
                    ProcessModel = new ProcessModel {
                        Process = process,
                    },
                };
                MemoryHandler handler = SharlayanMemoryManager.Instance.AddHandler(sharlayanConfiguration);
                handler.OnException += this.MemoryHandler_OnExceptionEvent;
                handler.OnMemoryHandlerDisposed += this.MemoryHandler_OnMemoryHandlerDisposedEvent;
                handler.OnMemoryLocationsFound += this.MemoryHandler_OnMemoryLocationsFoundEvent;
            }
        }

        private void SetupWorkerSets() {
            ICollection<MemoryHandler> memoryHandlers = SharlayanMemoryManager.Instance.GetHandlers();
            foreach (MemoryHandler memoryHandler in memoryHandlers) {
                WorkerSet workerSet = new WorkerSet(memoryHandler);
                this._workerSets.AddOrUpdate(memoryHandler.Configuration.ProcessModel.ProcessID, workerSet, (k, v) => workerSet);
            }
        }

        private void StartAllSharlayanWorkers() {
            this.StopAllSharlayanWorkers();

            foreach (WorkerSet workerSet in this._workerSets.Values) {
                workerSet.StartMemoryWorkers();
            }
        }

        private void StopAllSharlayanWorkers() {
            foreach (WorkerSet workerSet in this._workerSets.Values) {
                workerSet.StopMemoryWorkers();
            }
        }
    }
}