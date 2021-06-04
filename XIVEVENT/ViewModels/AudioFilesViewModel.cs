// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFilesViewModel.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AudioFilesViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.ViewModels {
    using System;

    public class AudioFilesViewModel : PropertyChangedBase {
        private static Lazy<AudioFilesViewModel> _instance = new Lazy<AudioFilesViewModel>(() => new AudioFilesViewModel());

        public AudioFilesViewModel() {
            this.RefreshAudioFilesCommand = new DelegatedCommand(
                _ => {
                    AppViewModel.Instance.RefreshAudioCache();
                });
        }

        public static AudioFilesViewModel Instance => _instance.Value;

        public DelegatedCommand RefreshAudioFilesCommand { get; }
    }
}