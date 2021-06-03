// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundSettings.xaml.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   SoundSettings.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Controls {
    using System.Windows.Controls;

    using XIVEVENT.ViewModels;

    /// <summary>
    /// Interaction logic for SoundSettings.xaml
    /// </summary>
    public partial class SoundSettings : UserControl {
        public SoundSettings() {
            this.InitializeComponent();

            this.DataContext = SoundSettingsViewModel.Instance;
        }
    }
}