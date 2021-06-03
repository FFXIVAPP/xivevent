// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HomeTabItemViewModel.cs">
//   Copyright© 2021 Ryan Wilson
//   Licensed under the MIT license. See LICENSE.md in the solution root for full license information.
// </copyright>
// <summary>
//   HomeTabItemViewModel.cs Implementation
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XIVEVENT.ViewModels {
    using System;
    using System.Threading.Tasks;

    using MaterialDesignThemes.Wpf;

    using XIVEVENT.Controls;
    using XIVEVENT.Models;

    public class HomeTabItemViewModel : PropertyChangedBase {
        private static Lazy<HomeTabItemViewModel> _instance = new Lazy<HomeTabItemViewModel>(() => new HomeTabItemViewModel());

        public HomeTabItemViewModel() {
            this.AddEventItemCommand = new DelegatedCommand(
                _ => {
                    AppViewModel.Instance.EventItems.Add(new EventItem());
                });

            this.SaveEventItemCommand = new DelegatedCommand(
                _ => {
                    HomeTabItem.Instance?.EventItemsDataGrid.CommitEdit();
                });

            this.DeleteEventItemCommand = new DelegatedCommand(
                eventItem => {
                    if (eventItem is EventItem item) {
                        AppViewModel.Instance.EventItems.Remove(item);
                    }
                });

            this.EditEventItemCommand = new DelegatedCommand(
                async eventItem => {
                    if (eventItem is not EventItem selectedEventItem) {
                        return;
                    }

                    EventItemEdit view = new EventItemEdit {
                        DataContext = selectedEventItem,
                    };

                    //show the dialog
                    await DialogHost.Show(view, "RootDialog", null, this.ExtendedClosingEventHandler);
                });
        }

        public static HomeTabItemViewModel Instance => _instance.Value;

        public DelegatedCommand AddEventItemCommand { get; }

        public DelegatedCommand EditEventItemCommand { get; }

        public DelegatedCommand SaveEventItemCommand { get; }

        public DelegatedCommand DeleteEventItemCommand { get; }

        private void ExtendedClosingEventHandler(object sender, DialogClosingEventArgs e) {
            if (e.Parameter is bool and false) {
                return;
            }

            e.Cancel();

            if (e.Session.Content is EventItemEdit { DataContext: EventItem eventItem, }) {
                AppViewModel.Instance.EventItems[AppViewModel.Instance.EventItems.IndexOf(eventItem)] = eventItem;
            }

            Task.Delay(TimeSpan.Zero).ContinueWith((t, _) => e.Session.Close(false), null, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}