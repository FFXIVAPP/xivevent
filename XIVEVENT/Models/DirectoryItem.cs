// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectoryItem.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   DirectoryItem.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Models {
    public class DirectoryItem : PropertyChangedBase {
        private string _current;

        private string _settingsDefault;

        public string SettingsDefault {
            get => this._settingsDefault;
            set => this.SetProperty(ref this._settingsDefault, value);
        }

        public string Current {
            get => this._current;
            set => this.SetProperty(ref this._current, value);
        }
    }
}