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
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.UserData;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Automatic_Alarm
{
    public partial class EditAutomaticAlarm : PhoneApplicationPage
    {
        private int hour;
        private int min;
        LongListMultiSelector ViolatingAlarms;
        private string save;
        private MessagePrompt messagePrompt;
        public ObservableCollection<ListItemAlarm> lateAlarmsDisplay = new ObservableCollection<ListItemAlarm>();
        public EditAutomaticAlarm()
        {
            InitializeComponent();

            mediaplayer2.Source = App.tuneList.getTune(App.settings.DefaultTuneSetting);
            tune_select.Content = App.tuneList.getKey(mediaplayer2.Source);
            timepick.Value = App.settings.DefaultTimeAutoSetting;
        }
        private void Set_alarm_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();
            for (int i = 0; i < notifications.Count(); i++)
            {
                if (notifications.ElementAt(i).ExpirationTime.TimeOfDay > notifications.ElementAt(i).BeginTime.AddSeconds(1).TimeOfDay &&notifications.ElementAt(i).ExpirationTime.TimeOfDay < notifications.ElementAt(i).BeginTime.AddSeconds(5).TimeOfDay)
                {
                    ScheduledActionService.Remove(notifications.ElementAt(i).Name);
                }
            }
            
            Appointments appts = new Appointments();

            appts.SearchCompleted += new EventHandler<AppointmentsSearchEventArgs>(Appointments_SearchCompleted);

            DateTime start = DateTime.Now.AddDays(1);
            start = start.AddHours(-start.Hour);
            start = start.AddMinutes(-start.Minute);
            DateTime end = start.AddDays(7);

            appts.SearchAsync(start, end, "Appointments Test #1");
        }
        void Appointments_SearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {

            try
            {
                List<Appointment> appointments = new List<Appointment>(e.Results);
                List<String> show = new List<String>();
                int i = 1;
                List<DateTime> alarmtimes = new List<DateTime>();
                DateTime temp = DateTime.Now;


                while (appointments.Count >= i || 8 > i)
                {
                    temp = temp.AddDays(1);

                    Appointment app = appointments.Find(item => item.StartTime.Hour != 0 && temp.Day == item.StartTime.Day);
                    if (app != null)
                    {
                        DateTime alarmtime = app.StartTime;
                        alarmtime = alarmtime.AddHours(-hour);
                        alarmtime = alarmtime.AddMinutes(-min);
                        alarmtimes.Add(alarmtime);
                        show.Add(alarmtime.ToString());

                        String AlarmName = System.Guid.NewGuid().ToString();
                        Alarm alarm = new Alarm(AlarmName);
                        alarm.Content = alarmtime.DayOfWeek + "! " + App.quotes.getQuote();
                        alarm.Sound = mediaplayer2.Source;
                        alarm.BeginTime = alarmtime;
                        alarm.ExpirationTime = alarmtime.AddSeconds(2);
                        alarm.RecurrenceType = RecurrenceInterval.None;
                        if (alarm.BeginTime.TimeOfDay > App.settings.DefaultLateTimeSetting.TimeOfDay)
                            lateAlarmsDisplay.Add(new ListItemAlarm(alarm.Name, alarm.BeginTime, alarm.Sound));
                        ScheduledActionService.Add(alarm);

                    }
                    i++;
                }
            }
            catch (System.Exception)
            {
                //That's okay, no results
            }
            if (lateAlarmsDisplay.Count() > 0)
            {
                messagePrompt = new MessagePrompt();
                messagePrompt.Background = (Brush)Application.Current.Resources["PhoneBackgroundBrush"];
                messagePrompt.IsCancelVisible = false;
                messagePrompt.Title = "Late Alarms";
                UserControl lateAlarms = new LateAlarmsPopup();
                ViolatingAlarms = lateAlarms.GetFirstLogicalChildByType<LongListMultiSelector>(true);
                lateAlarmsDisplay = new ObservableCollection<ListItemAlarm>(lateAlarmsDisplay.OrderBy(x => x.Date));
                ViolatingAlarms.ItemsSource = lateAlarmsDisplay;
                var result2 = lateAlarms.GetFirstLogicalChildByType<TextBlock>(true);
                result2.Text = "Alarms after " + App.settings.DefaultLateTimeSetting.ToShortTimeString();
                messagePrompt.Body = lateAlarms;
                RoundButton remove_btn = new RoundButton() {ImageSource = new BitmapImage(new Uri("Icons/appbar.delete.rest.png", UriKind.Relative)), Tag="remove"};
                remove_btn.Click += removeLate_Click;
                messagePrompt.ActionPopUpButtons.Add(remove_btn);
                messagePrompt.ActionPopUpButtons.ElementAt(0).Click += confirm_Click;
                messagePrompt.ActionPopUpButtons.ElementAt(0).Tag = "confirm";
                messagePrompt.ActionPopUpButtons.ElementAt(1).Tag = "cancel";
                ViolatingAlarms.SelectionChanged += ViolatingAlarms_SelectionChanged;
                foreach (ListItemAlarm alarm in lateAlarmsDisplay)
                {
                    ViolatingAlarms.SelectedItems.Add(alarm);
                }
                messagePrompt.Show();
            }
            else
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        private void timepick_ValueChanged_1(object sender, DateTimeValueChangedEventArgs e)
        {
            DateTime TimeAhead = (DateTime)e.NewDateTime;
            hour = TimeAhead.Hour;
            min = TimeAhead.Minute;
            App.settings.DefaultTimeAutoSetting = (DateTime)e.NewDateTime;
        }
        private void set_source(string source)
        {
            mediaplayer2.Source = App.tuneList.getTune(source);
        }
        private void help_btn_Click(object sender, RoutedEventArgs e)
        {
            help_btn.Visibility = System.Windows.Visibility.Collapsed;
            border_1.Visibility = System.Windows.Visibility.Collapsed;
            border_2.Visibility = System.Windows.Visibility.Collapsed;
            border_3.Visibility = System.Windows.Visibility.Collapsed;
            border_4.Visibility = System.Windows.Visibility.Collapsed;
            help_text.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            save = "";
            timepick.Value = App.settings.DefaultTimeAutoSetting;
            if (NavigationContext.QueryString.TryGetValue("saved", out save))
            {
                string[] _save = save.Split(new string[] { "split" }, StringSplitOptions.None);
                tune_select.Content = _save[2];
                set_source(_save[2]);
                hour = Convert.ToInt32(_save[0]);
                min = Convert.ToInt32(_save[1]);
                DateTime savedtime = DateTime.Now;
                savedtime = savedtime.AddHours(hour - savedtime.Hour);
                savedtime = savedtime.AddMinutes(min - savedtime.Minute);
                timepick.Value = savedtime;
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
                NavigationContext.QueryString.Remove("saved");
            }
        }

        private void tune_select_Click_1(object sender, RoutedEventArgs e)
        {
            String[] saves = new String[] { hour.ToString(), "split", min.ToString() };
            string savestring = String.Concat(saves);
            NavigationService.Navigate(new Uri("/TunePicker.xaml?save=" + savestring, UriKind.Relative));
        }
        
        private void removeLate_Click(object sender,EventArgs e)
        {
            foreach (ListItemAlarm alarm in ViolatingAlarms.SelectedItems)
            {
                ScheduledActionService.Remove(alarm.Name);
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        private void confirm_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
        private void ViolatingAlarms_SelectionChanged(object sender, EventArgs e)
        {
            if (((LongListMultiSelector)sender).SelectedItems.Count == 0)
            {
                messagePrompt.ActionPopUpButtons.First(x => x.Tag.Equals("remove")).Visibility = System.Windows.Visibility.Collapsed;
                messagePrompt.ActionPopUpButtons.First(x => x.Tag.Equals("confirm")).Visibility = System.Windows.Visibility.Visible;
            }
            else if (((LongListMultiSelector)sender).SelectedItems.Count > 0)
            {
                messagePrompt.ActionPopUpButtons.First(x => x.Tag.Equals("remove")).Visibility = System.Windows.Visibility.Visible;
                messagePrompt.ActionPopUpButtons.First(x => x.Tag.Equals("confirm")).Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}