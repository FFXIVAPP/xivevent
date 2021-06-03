// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TTSMessageData.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   TTSMessageData.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Audio {
    using System;

    public class TTSMessageData : IEquatable<TTSMessageData> {
        public TTSMessageData(string message, float volume, float rate) {
            this.Text = message;
            this.Volume = volume;
            this.Rate = rate;
        }

        public float Rate { get; set; }

        public string Text { get; }

        public float Volume { get; set; }

        public bool Equals(TTSMessageData other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return string.Equals(this.Text, other.Text, StringComparison.OrdinalIgnoreCase) && Math.Abs(this.Volume - other.Volume) < 0.0001337 && Math.Abs(this.Rate - other.Rate) < 0.0001337;
        }

        public static bool operator ==(TTSMessageData left, TTSMessageData right) {
            return Equals(left, right);
        }

        public static bool operator !=(TTSMessageData left, TTSMessageData right) {
            return !Equals(left, right);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != this.GetType()) {
                return false;
            }

            return this.Equals((TTSMessageData) obj);
        }

        public override int GetHashCode() {
            return this.Text == null
                       ? 0
                       : this.Text.GetHashCode();
        }
    }
}