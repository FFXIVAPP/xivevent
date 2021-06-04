// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedAudioFile.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   CachedAudioFile.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Audio {
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NAudio.Wave;

    public class CachedAudioFile : IDisposable {
        public CachedAudioFile(string audioFileName) {
            using AudioFileReader audioFileReader = new AudioFileReader(audioFileName);
            this.WaveFormat = audioFileReader.WaveFormat;
            if (this.WaveFormat.SampleRate != 44100 || this.WaveFormat.Channels != 2) {
                using ResamplerDmoStream resampler = new ResamplerDmoStream(audioFileReader, WaveFormat.CreateIeeeFloatWaveFormat(44100, 2));
                ISampleProvider sampleProvider = resampler.ToSampleProvider();

                this.WaveFormat = sampleProvider.WaveFormat;

                List<float> waveData = new List<float>((int) resampler.Length);

                float[] waveBuffer = new float[resampler.WaveFormat.SampleRate * resampler.WaveFormat.Channels];
                int samplesRead;

                while ((samplesRead = sampleProvider.Read(waveBuffer, 0, waveBuffer.Length)) > 0) {
                    waveData.AddRange(waveBuffer.Take(samplesRead));
                }

                this.AudioData = waveData.ToArray();
            }
            else {
                List<float> waveData = new List<float>((int) (audioFileReader.Length / 4));

                float[] waveBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];

                int samplesRead;

                while ((samplesRead = audioFileReader.Read(waveBuffer, 0, waveBuffer.Length)) > 0) {
                    waveData.AddRange(waveBuffer.Take(samplesRead));
                }

                this.AudioData = waveData.ToArray();
            }
        }

        public float[] AudioData { get; private set; }

        public WaveFormat WaveFormat { get; }

        public void Dispose() {
            this.AudioData = null;
            // this is needed or refreshing audio files won't recover the data fast enough and it will be a growing memory size
            GC.Collect();
        }

        ~CachedAudioFile() {
            this.Dispose();
        }
    }
}