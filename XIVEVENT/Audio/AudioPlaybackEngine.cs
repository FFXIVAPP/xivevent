// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioPlaybackEngine.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AudioPlaybackEngine.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Audio {
    using System;

    using NAudio.Wave;
    using NAudio.Wave.SampleProviders;

    using NLog;

    using XIVEVENT.ViewModels;

    public class AudioPlaybackEngine : IDisposable {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static Lazy<AudioPlaybackEngine> _instance = new Lazy<AudioPlaybackEngine>(() => new AudioPlaybackEngine());

        private int ChannelCount = 2;

        private MixingSampleProvider Mixer;

        private IWavePlayer OutputDevice;

        private int SampleRate = 44100;

        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2) {
            this.SampleRate = sampleRate;
            this.ChannelCount = channelCount;
            this.SetupEngine();
        }

        public static AudioPlaybackEngine Instance => _instance.Value;

        private Guid LastAudioDevice { get; set; }

        public void Dispose() {
            this.OutputDevice?.Dispose();
        }

        ~AudioPlaybackEngine() {
            this.Dispose();
        }

        public void PlayAudioFile(CachedAudioFile cachedAudioFile, float volume = 1.0f) {
            if (this.LastAudioDevice != SoundSettingsViewModel.Instance.SelectedAudioDevice.Guid) {
                this.OutputDevice.Stop();
                this.SetupEngine();
            }

            this.Mixer.AddMixerInput(new CachedAudioFileSampleProvider(cachedAudioFile, volume / 100));
        }

        private void SetupEngine() {
            if (SoundSettingsViewModel.Instance.SelectedAudioDevice.Guid == Guid.Empty) {
                this.OutputDevice = new DirectSoundOut(100);
            }
            else {
                this.LastAudioDevice = SoundSettingsViewModel.Instance.SelectedAudioDevice.Guid;
                this.OutputDevice = new DirectSoundOut(SoundSettingsViewModel.Instance.SelectedAudioDevice.Guid, 100);
            }

            this.Mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(this.SampleRate, this.ChannelCount)) {
                ReadFully = true,
            };
            this.OutputDevice.Init(this.Mixer);
            this.OutputDevice.Play();
        }
    }
}