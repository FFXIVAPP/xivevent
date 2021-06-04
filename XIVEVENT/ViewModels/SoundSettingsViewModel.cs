// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundSettingsViewModel.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SoundSettingsViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.ViewModels {
    using System;
    using System.Linq;

    using NAudio.Wave;

    using XIVEVENT.Helpers;
    using XIVEVENT.Properties;

    public class SoundSettingsViewModel : PropertyChangedBase {
        private static Lazy<SoundSettingsViewModel> _instance = new Lazy<SoundSettingsViewModel>(() => new SoundSettingsViewModel());

        private DirectSoundDeviceInfo _selectedAudioDevice;

        public SoundSettingsViewModel() {
            this.RefreshAudioDevicesCommand = new DelegatedCommand(
                _ => {
                    AppViewModel.Instance.RefreshAudioDevices();
                });

            this.TestAudioFilesCommand = new DelegatedCommand(
                _ => {
                    string audioFileName = AppViewModel.Instance.AudioFiles.FirstOrDefault()?.Name;
                    if (audioFileName is not null) {
                        EventExecutionHelper.PlayCachedAudioFile(audioFileName, Settings.Default.MasterVolume);
                    }
                });
        }

        public static SoundSettingsViewModel Instance => _instance.Value;

        public DelegatedCommand RefreshAudioDevicesCommand { get; }

        public DelegatedCommand TestAudioFilesCommand { get; }

        public DirectSoundDeviceInfo SelectedAudioDevice {
            get => this._selectedAudioDevice;
            set => this.SetProperty(ref this._selectedAudioDevice, value);
        }
    }
}