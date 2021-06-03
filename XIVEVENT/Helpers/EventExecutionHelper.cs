// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventExecutionHelper.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   EventExecutionHelper.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Helpers {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.IO;
    using System.Speech.Synthesis;

    using NAudio.Wave;

    using NLog;

    using XIVEVENT.Audio;
    using XIVEVENT.Utilities;
    using XIVEVENT.ViewModels;

    public static class EventExecutionHelper {
        private const int Latency = 100;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly ConcurrentDictionary<TTSMessageData, byte[]> Cached = new ConcurrentDictionary<TTSMessageData, byte[]>();

        public static bool PlayCachedAudioFile(string soundFile, float volume = 1.0f) {
            try {
                if (AppViewModel.Instance.CachedAudioFiles.TryGetValue(soundFile, out CachedAudioFile cachedAudioFile)) {
                    AudioPlaybackEngine.Instance.PlayAudioFile(cachedAudioFile, volume);
                }
            }
            catch (Exception ex) {
                Logging.Log(Logger, ex);
                return false;
            }

            return true;
        }

        public static void RunProgram(string path, string arguments) {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) {
                return;
            }

            try {
                ProcessStartInfo processStartInfo = new ProcessStartInfo {
                    Arguments = string.IsNullOrWhiteSpace(arguments)
                                    ? string.Empty
                                    : arguments,
                    FileName = path,
                };
                using Process? start = Process.Start(processStartInfo);
            }
            catch (Exception ex) {
                Logging.Log(Logger, "Executable Failed", ex);
            }
        }

        public static void SpeakMessage(string message, float volume, float rate) {
            TTSMessageData ttsData = new TTSMessageData(message, volume, rate);
            DisposableSound disposable = new DisposableSound {
                MemoryStream = GetMemoryStream(ttsData),
                DirectSoundOut = SoundSettingsViewModel.Instance.SelectedAudioDevice?.Guid is null
                                     ? new DirectSoundOut(Latency)
                                     : new DirectSoundOut(SoundSettingsViewModel.Instance.SelectedAudioDevice.Guid, Latency),
            };
            disposable.WaveFileReader = new WaveFileReader(disposable.MemoryStream);
            disposable.WaveChannel = new WaveChannel32(disposable.WaveFileReader);
            disposable.DirectSoundOut.Init(disposable.WaveChannel);
            disposable.DirectSoundOut.PlaybackStopped += disposable.DisposeDirectSound;
            disposable.DirectSoundOut.Play();
        }

        private static byte[] CreateNewMemoryStream(TTSMessageData messageData) {
            using MemoryStream memoryStream = new MemoryStream();
            using SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

            speechSynthesizer.SetOutputToWaveStream(memoryStream);
            speechSynthesizer.Volume = (int) messageData.Volume;
            speechSynthesizer.Rate = (int) messageData.Rate;
            speechSynthesizer.Speak(messageData.Text);

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }

        private static MemoryStream GetMemoryStream(TTSMessageData messageData) {
            byte[] ttsBytes = Cached.GetOrAdd(messageData, t => CreateNewMemoryStream(messageData));
            return new MemoryStream(ttsBytes);
        }
    }
}