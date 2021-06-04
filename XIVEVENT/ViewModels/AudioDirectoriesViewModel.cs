// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioDirectoriesViewModel.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AudioDirectoriesViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.ViewModels {
    using System;
    using System.Threading.Tasks;

    using MaterialDesignThemes.Wpf;

    using XIVEVENT.Controls;
    using XIVEVENT.Models;
    using XIVEVENT.Properties;

    public class AudioDirectoriesViewModel : PropertyChangedBase {
        private static Lazy<AudioDirectoriesViewModel> _instance = new Lazy<AudioDirectoriesViewModel>(() => new AudioDirectoriesViewModel());

        public AudioDirectoriesViewModel() {
            this.AddDirectoryCommand = new DelegatedCommand(
                async _ => {
                    DirectoryEdit view = new DirectoryEdit {
                        DataContext = new DirectoryItem {
                            SettingsDefault = "",
                            Current = "",
                        },
                    };
                    await DialogHost.Show(view, "RootDialog", null, this.AddDirectory_ClosingEventHandler);
                });

            this.DeleteDirectoryCommand = new DelegatedCommand(
                value => {
                    if (value is not DirectoryItem directoryItem) {
                        return;
                    }

                    Settings.Default.AudioCacheDirectories.Remove(directoryItem.Current);
                    AppViewModel.Instance.AudioCacheDirectories.Remove(directoryItem);
                    AppViewModel.Instance.RefreshAudioCache();
                });

            this.EditDirectoryCommand = new DelegatedCommand(
                async value => {
                    if (value is not DirectoryItem directoryItem) {
                        return;
                    }

                    DirectoryEdit view = new DirectoryEdit {
                        DataContext = directoryItem,
                    };

                    await DialogHost.Show(view, "RootDialog", null, this.EditDirectory_ClosingEventHandler);
                });
        }

        public static AudioDirectoriesViewModel Instance => _instance.Value;

        public DelegatedCommand AddDirectoryCommand { get; }

        public DelegatedCommand EditDirectoryCommand { get; }

        public DelegatedCommand DeleteDirectoryCommand { get; }

        private void AddDirectory_ClosingEventHandler(object sender, DialogClosingEventArgs e) {
            if (e.Parameter is bool and false) {
                return;
            }

            e.Cancel();

            if (e.Session.Content is DirectoryEdit { DataContext: DirectoryItem directoryItem, }) {
                directoryItem.SettingsDefault = directoryItem.Current;

                if (!string.IsNullOrWhiteSpace(directoryItem.Current) && !string.IsNullOrWhiteSpace(directoryItem.SettingsDefault)) {
                    AppViewModel.Instance.AudioCacheDirectories.Add(directoryItem);
                    Settings.Default.AudioCacheDirectories.Add(directoryItem.Current);

                    AppViewModel.Instance.RefreshAudioCache();
                }
            }

            Task.Delay(TimeSpan.Zero).ContinueWith((t, _) => e.Session.Close(false), null, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void EditDirectory_ClosingEventHandler(object sender, DialogClosingEventArgs e) {
            if (e.Parameter is bool and false) {
                return;
            }

            e.Cancel();

            if (e.Session.Content is DirectoryEdit { DataContext: DirectoryItem directoryItem, }) {
                if (!string.IsNullOrWhiteSpace(directoryItem.Current) && !string.IsNullOrWhiteSpace(directoryItem.SettingsDefault)) {
                    Settings.Default.AudioCacheDirectories.Remove(directoryItem.SettingsDefault);
                    Settings.Default.AudioCacheDirectories.Add(directoryItem.Current);

                    AppViewModel.Instance.RefreshAudioCache();
                }
            }

            Task.Delay(TimeSpan.Zero).ContinueWith((t, _) => e.Session.Close(false), null, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}