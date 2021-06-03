// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisposableSound.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DisposableSound.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Audio {
    using System;
    using System.IO;

    using NAudio.Wave;

    public class DisposableSound : IDisposable {
        public DirectSoundOut DirectSoundOut;

        public MemoryStream MemoryStream;

        public WaveChannel32 WaveChannel;

        public WaveFileReader WaveFileReader;

        public void Dispose() {
            if (this.WaveFileReader is not null) {
                this.WaveFileReader.Dispose();
            }

            this.WaveFileReader = null;

            if (this.WaveChannel is not null) {
                this.WaveChannel.Dispose();
            }

            this.WaveChannel = null;

            if (this.MemoryStream is not null) {
                this.MemoryStream.Dispose();
            }

            this.MemoryStream = null;

            if (this.DirectSoundOut is not null) {
                this.DirectSoundOut.Dispose();
            }

            this.DirectSoundOut = null;
        }

        public void DisposeDirectSound(object sender, StoppedEventArgs e) {
            this.DirectSoundOut.PlaybackStopped -= this.DisposeDirectSound;
            this.Dispose();
        }
    }
}
