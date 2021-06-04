// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFiles.xaml.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   AudioFiles.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Controls {
    using System.Windows.Controls;

    using XIVEVENT.ViewModels;

    /// <summary>
    /// Interaction logic for AudioFiles.xaml
    /// </summary>
    public partial class AudioFiles : UserControl {
        public AudioFiles() {
            this.InitializeComponent();

            this.DataContext = AudioFilesViewModel.Instance;
        }
    }
}