using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;

namespace Automatic_Alarm
{
    public partial class EditAlarm : PhoneApplicationPage
    {
        private DateTime time;
        private DateTime date;
        private string removeString;
        private string name;
        public EditAlarm()
        {
            InitializeComponent();

            time = DateTime.Now;
            date = DateTime.Now;
            removeString = null;
            mediaplayer.Source = App.tuneList.getTune(App.settings.DefaultTuneSetting);
            tune_select.Content = App.tuneList.getKey(mediaplayer.Source);
        }

        private void setAlarm_btn_Click(object sender, RoutedEventArgs e)
        {
            
            String AlarmName = System.Guid.NewGuid().ToString();
            date = date.AddHours(-date.Hour);
            date = date.AddMinutes(-date.Minute);
            date = date.AddSeconds(-date.Second);
            DateTime AlarmTime = date.AddHours(time.Hour);
            AlarmTime = AlarmTime.AddMinutes(time.Minute);
            if (AlarmTime > DateTime.Now)
            {
                Alarm alarm = new Alarm(AlarmName);
                alarm.Sound = mediaplayer.Source;
                alarm.BeginTime = AlarmTime;
                if (RecurranceCheckbox.IsChecked == true)
                {
                    alarm.RecurrenceType = RecurrenceInterval.Daily;
                    alarm.Content = App.quotes.getQuote();
                    alarm.ExpirationTime = AlarmTime.AddYears(10);
                }
                else
                {
                    alarm.RecurrenceType = RecurrenceInterval.None;
                    alarm.Content = AlarmTime.DayOfWeek + "! " + App.quotes.getQuote();
                    alarm.ExpirationTime = AlarmTime;
                }

                ScheduledActionService.Add(alarm);
                if (removeString != null && removeString != "")
                {
                    ScheduledActionService.Remove(removeString);
                    removeString = null;
                }
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
            else
                MessageBox.Show("Alarm time has to be in the future");
        }

        private void set_source(string source)
        {
            mediaplayer.Source = App.tuneList.getTune(source);
        }

        private void date_picker_ValueChanged_1(object sender, DateTimeValueChangedEventArgs e)
        {
            date = (DateTime)e.NewDateTime;
        }

        private void time_picker_ValueChanged_1(object sender, DateTimeValueChangedEventArgs e)
        {
            time = (DateTime)e.NewDateTime;

        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            name = "";

            if (NavigationContext.QueryString.TryGetValue("edit", out name))
            {
                Alarm editAlarm = (Alarm)ScheduledActionService.Find(name);
                time_picker.Value = editAlarm.BeginTime;
                date_picker.Value = editAlarm.BeginTime;
                mediaplayer.Source = editAlarm.Sound;
                tune_select.Content = App.tuneList.getKey(editAlarm.Sound);
                removeString = editAlarm.Name;
                setAlarm_btn.Content = "Save Changes";
                if (editAlarm.RecurrenceType == RecurrenceInterval.Daily)
                    RecurranceCheckbox.IsChecked = true;
                NavigationContext.QueryString.Remove("edit");
            }
            else if (NavigationContext.QueryString.TryGetValue("saved", out name))
            {
                string[] _save = name.Split(new string[] { "split" }, StringSplitOptions.None);
                tune_select.Content = _save[6];
                set_source(_save[6]);
                if (_save[5].Equals("True"))
                    RecurranceCheckbox.IsChecked = true;
                setAlarm_btn.Content = _save[4];
                DateTime saved_date = DateTime.Parse(_save[2]);
                date_picker.Value = saved_date;
                DateTime saved_time = DateTime.Now;
                int hour = Convert.ToInt32(_save[0]);
                int min = Convert.ToInt32(_save[1]);
                saved_time = saved_time.AddHours(hour - saved_time.Hour);
                saved_time = saved_time.AddMinutes(min - saved_time.Minute);
                time_picker.Value = saved_time;
                removeString = _save[3];
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
                NavigationContext.QueryString.Remove("saved");
            }
            else if (NavigationContext.QueryString.TryGetValue("start", out name))
            {
                if (time > ((DateTime.Now.AddHours(6 - DateTime.Now.Hour)).AddMinutes(-DateTime.Now.Minute)).AddSeconds(-DateTime.Now.Second))
                {
                    time_picker.Value = (((time.AddHours(8 - time.Hour)).AddMinutes(-time.Minute)).AddSeconds(-time.Second)).AddMilliseconds(-time.Millisecond);
                    date_picker.Value = date.AddDays(1);
                }
                else
                {
                    time_picker.Value = (((time.AddHours(8 - time.Hour)).AddMinutes(-time.Minute)).AddSeconds(-time.Second)).AddMilliseconds(-time.Millisecond);
                }
                NavigationContext.QueryString.Remove("start");
            }
        }

        private void tune_select_Click_1(object sender, RoutedEventArgs e)
        {
            String[] saves = new String[] { time.Hour.ToString(), "split", time.Minute.ToString(), "split", date_picker.ValueString, "split", removeString, "split", (string)setAlarm_btn.Content, "split", RecurranceCheckbox.IsChecked.ToString()};
            string savestring = String.Concat(saves);
            NavigationService.Navigate(new Uri("/TunePicker.xaml?save=" + savestring, UriKind.Relative));
        }
    }
}