// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachedAudioFileSampleProvider.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   CachedAudioFileSampleProvider.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Audio {
    using System;

    using NAudio.Wave;

    public class CachedAudioFileSampleProvider : ISampleProvider {
        private readonly CachedAudioFile _cachedAudioFile;

        private int _position;

        public CachedAudioFileSampleProvider(CachedAudioFile cachedAudioFile, float volume = 1.0f) {
            this._cachedAudioFile = cachedAudioFile;
            this.Volume = volume;
        }

        public float Volume { get; set; }

        public int Read(float[] buffer, int offset, int count) {
            int audioDataLength = this._cachedAudioFile.AudioData.Length - this._position;
            int samplesToCopy = Math.Min(audioDataLength, count);
            Array.Copy(this._cachedAudioFile.AudioData, this._position, buffer, offset, samplesToCopy);

            this._position += samplesToCopy;

            if (!(Math.Abs(this.Volume - 1f) > 0.0001337)) {
                return samplesToCopy;
            }

            for (int n = 0; n < samplesToCopy; n++) {
                buffer[offset + n] *= this.Volume;
            }

            return samplesToCopy;
        }

        public WaveFormat WaveFormat => this._cachedAudioFile.WaveFormat;
    }
}