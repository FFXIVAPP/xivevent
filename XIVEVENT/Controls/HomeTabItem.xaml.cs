// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeTabItem.xaml.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   HomeTabItem.xaml.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.Controls {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows.Controls;

    using Sharlayan;
    using Sharlayan.Core;

    using XIVEVENT.Helpers;
    using XIVEVENT.Models;
    using XIVEVENT.Properties;
    using XIVEVENT.SharlayanWrappers;
    using XIVEVENT.Utilities;
    using XIVEVENT.ViewModels;

    /// <summary>
    /// Interaction logic for HomeTabItem.xaml
    /// </summary>
    public partial class HomeTabItem : UserControl, IDisposable {
        public HomeTabItem() {
            this.InitializeComponent();

            this.DataContext = HomeTabItemViewModel.Instance;

            Instance = this;

            EventHost.Instance.OnNewChatLogItem += this.OnNewChatLogItem;
        }

        public static HomeTabItem Instance { get; set; }

        public void Dispose() {
            EventHost.Instance.OnNewChatLogItem -= this.OnNewChatLogItem;
        }

        ~HomeTabItem() {
            this.Dispose();
        }

        private void OnNewChatLogItem(object? sender, MemoryHandler memoryHandler, ChatLogItem chatLogItem) {
            foreach (EventItem eventItem in AppViewModel.Instance.EventItems.Where(item => item.IsEnabled)) {
                string arguments = eventItem.ExecutableArguments;
                string message = eventItem.TTSMessage;
                switch (eventItem.RegEx) {
                    case "*":
                    case ".+":
                        break;
                    default:
                        Match matches = eventItem.CompiledRegEx.Match(chatLogItem.Line);
                        if (!matches.Success) {
                            continue;
                        }

                        arguments = matches.Result(eventItem.ExecutableArguments);
                        message = matches.Result(message);
                        break;
                }

                this.SetupActions(eventItem, arguments, message);
            }
        }

        private void ExecuteActions(IEnumerable<Action> actions) {
            foreach (Action action in actions.AsParallel()) {
                action();
            }
        }

        private void SetupActions(EventItem eventItem, string arguments, string message) {
            int volume = Convert.ToInt32(eventItem.Volume * Settings.Default.MasterVolume);

            List<Action> actions = new List<Action> {
                this.PlayCachedAudioFile(eventItem, volume),
                this.RunProgram(eventItem, arguments),
                this.SpeakMessage(message, volume, eventItem.SpeechRate),
            };

            actions.RemoveAll(action => action == null);

            if (!actions.Any()) {
                return;
            }

            int delay = eventItem.Delay;
            if (delay <= 0) {
                this.ExecuteActions(actions);
            }
            else {
                Task.Run(
                    async () => {
                        await Task.Delay(TimeSpan.FromSeconds(delay));
                        this.ExecuteActions(actions);
                    });
            }
        }

        private Action PlayCachedAudioFile(EventItem eventItem, float volume) {
            string soundFile = eventItem.Sound;
            if (string.IsNullOrWhiteSpace(soundFile)) {
                return null;
            }

            return () => EventExecutionHelper.PlayCachedAudioFile(soundFile, volume);
        }

        private Action SpeakMessage(string message, float volume, float rate) {
            if (string.IsNullOrWhiteSpace(message)) {
                return null;
            }

            return () => EventExecutionHelper.SpeakMessage(message, volume, rate);
        }

        private Action RunProgram(EventItem eventItem, string arguments) {
            if (string.IsNullOrWhiteSpace(eventItem.ExecutablePath)) {
                return null;
            }

            return () => EventExecutionHelper.RunProgram(eventItem.ExecutablePath, arguments);
        }
    }
}