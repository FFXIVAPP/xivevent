// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventItem.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   EventItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Models {
    using System;
    using System.Text.RegularExpressions;

    using XIVEVENT.Utilities;

    public class EventItem : PropertyChangedBase {
        private string _category;

        private int _delay;

        private string _executableArguments;

        private string _executablePath;

        private bool _isEnabled;

        private Guid? _key;

        private string _regEx;

        private string _sound;

        private float _speechRate = -2.0f;

        private string _ttsMessage;

        private float _volume = 100.0f;

        public int Delay {
            get => this._delay;
            set => this.SetProperty(ref this._delay, value);
        }

        public Guid? Key {
            get => this._key ??= Guid.NewGuid();
            set => this.SetProperty(ref this._key, value);
        }

        public bool IsEnabled {
            get => this._isEnabled;
            set => this.SetProperty(ref this._isEnabled, value);
        }

        public string Category {
            get => this._category;
            set => this.SetProperty(ref this._category, value);
        }

        public string RegEx {
            get => this._regEx;
            set {
                if (SharedRegEx.IsValidRegex(value)) {
                    this.CompiledRegEx = new Regex(value, SharedRegEx.DefaultOptions);
                }

                this.SetProperty(ref this._regEx, value);
            }
        }

        public string Sound {
            get => this._sound;
            set => this.SetProperty(ref this._sound, value);
        }

        public float Volume {
            get => this._volume;
            set => this.SetProperty(ref this._volume, value);
        }

        public float SpeechRate {
            get => this._speechRate;
            set => this.SetProperty(ref this._speechRate, value);
        }

        public string TTSMessage {
            get => this._ttsMessage;
            set => this.SetProperty(ref this._ttsMessage, value);
        }

        public string ExecutablePath {
            get => this._executablePath;
            set => this.SetProperty(ref this._executablePath, value);
        }

        public string ExecutableArguments {
            get => this._executableArguments;
            set => this.SetProperty(ref this._executableArguments, value);
        }

        public Regex CompiledRegEx { get; set; } = new Regex(@".+", SharedRegEx.DefaultOptions);
    }
}