// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioDirectories.xaml.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AudioDirectories.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Controls {
    using System.Windows.Controls;

    using XIVEVENT.ViewModels;

    /// <summary>
    /// Interaction logic for AudioDirectories.xaml
    /// </summary>
    public partial class AudioDirectories : UserControl {
        public AudioDirectories() {
            this.InitializeComponent();

            this.DataContext = AudioDirectoriesViewModel.Instance;
        }
    }
}