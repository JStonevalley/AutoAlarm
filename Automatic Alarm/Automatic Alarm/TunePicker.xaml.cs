using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace Automatic_Alarm
{
    public partial class TunePicker : PhoneApplicationPage
    {
        private string save;
        private string _save;
        private string[] tunes;
        public TunePicker()
        {
            InitializeComponent();
            TuneListBox.SelectedIndex = -1;
            tunes = App.tuneList.getKeys();
            TuneListBox.ItemsSource = (new ObservableCollection<string>(tunes));
            
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            save = "";

            if (NavigationContext.QueryString.TryGetValue("save", out save))
            {
                _save = save;
                NavigationContext.QueryString.Remove("save");
            }
        }

        private void TuneListBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (TuneListBox.SelectedIndex == -1)
                return;
            if (_save == null)
                NavigationService.Navigate(new Uri("/SettingsPage.xaml?saved=" + tunes[TuneListBox.SelectedIndex], UriKind.Relative));
            else if (_save.Length < 10)
            NavigationService.Navigate(new Uri("/EditAutomaticAlarm.xaml?saved=" + (_save + "split" + tunes[TuneListBox.SelectedIndex]), UriKind.Relative));
            else
            NavigationService.Navigate(new Uri("/EditAlarm.xaml?saved=" + (_save + "split" + tunes[TuneListBox.SelectedIndex]), UriKind.Relative));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tuneplayer.Source = (App.tuneList.getTune(((string)((Button)sender).Tag)));
        }
    }
}