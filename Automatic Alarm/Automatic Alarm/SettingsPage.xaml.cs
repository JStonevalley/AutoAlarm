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

namespace Automatic_Alarm
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        PeriodicTask periodicTask;

        string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = false;

        public SettingsPage()
        {
            InitializeComponent();
            
        }
        private void tone_select_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TunePicker.xaml?", UriKind.Relative));
        }
        private void time_value_Changed(object sender, DateTimeValueChangedEventArgs e)
        {
            App.settings.DefaultLateTimeSetting = (DateTime)e.NewDateTime;
            if (ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString())) != null)
                EnableBackgroundAgent();
        }
        
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string save = "";

            if (NavigationContext.QueryString.TryGetValue("saved", out save))
            {
                App.settings.DefaultTuneSetting = save;
                NavigationContext.QueryString.Remove("save");
                NavigationService.RemoveBackEntry();
                NavigationService.RemoveBackEntry();
            }
            default_tone_btn.Content = App.settings.DefaultTuneSetting;
            default_time_picker.Value = App.settings.DefaultLateTimeSetting;
            if (ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString())) != null)
                LiveTileCheckbox.IsChecked = true;
        }
        private void EnableBackgroundAgent()
        {
            agentsAreEnabled = true;
            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            periodicTask = new PeriodicTask(periodicTaskName);
            periodicTask.Description = App.settings.DefaultTimeAutoSetting.Hour + "split" + App.settings.DefaultTimeAutoSetting.Minute + "split" + App.settings.DefaultLateTimeSetting.Hour + "split" + App.settings.DefaultLateTimeSetting.Minute + "split" + App.settings.DefaultTuneSetting;
            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);
                //ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(10));
                // If debugging is enabled, use LaunchForTest to launch the agent in one minute.
#if(DEBUG_AGENT)
    ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                    LiveTileCheckbox.IsChecked = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.

                }
                LiveTileCheckbox.IsChecked = false;
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
                LiveTileCheckbox.IsChecked = false;
            }
        }
        private void DissableBackgroundAgent()
        {
            agentsAreEnabled = false;
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }
        }
        private void LiveTileCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            EnableBackgroundAgent();
            IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();
            DateTime now = DateTime.Now;
            ScheduledNotification next = null;
            if (notifications.Count() != 0)
            {
                next = (Alarm)notifications.ElementAt(0);
                for (int i = 0; i < notifications.Count(); i++)
                {
                    Alarm alarm = (Alarm)notifications.ElementAt(i);
                    if (alarm.ExpirationTime < now)
                        ScheduledActionService.Remove(alarm.Name);
                    else if (alarm.BeginTime < next.BeginTime)
                        next = alarm;
                }
            }
            String DisplayHour = "";
            String DisplayMinute = "";
            String DisplayDay = "";

            if (next != null)
            {
                DisplayDay = next.BeginTime.DayOfWeek.ToString() + "\n";
                if (next.BeginTime.Hour > 9)
                    DisplayHour = next.BeginTime.Hour.ToString() + ":";
                else
                    DisplayHour = "0" + next.BeginTime.Hour.ToString() + ":";

                if (next.BeginTime.Minute > 9)
                    DisplayMinute = next.BeginTime.Minute.ToString();
                else
                    DisplayMinute = "0" + next.BeginTime.Minute.ToString();
            }
            else
                DisplayHour = "No alarms set.\nSleep well!";

            ShellTile oTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));

            if (oTile != null && oTile.NavigationUri.ToString().Contains("flip"))
            {
                FlipTileData oFliptile = new FlipTileData();
                oFliptile.Title = "AutoAlarm";
                oFliptile.Count = 0;
                oFliptile.BackTitle = "AutoAlarm";

                oFliptile.BackContent = DisplayDay + DisplayHour + DisplayMinute;
                oFliptile.WideBackContent = DisplayDay + DisplayHour + DisplayMinute;

                oFliptile.SmallBackgroundImage = new Uri("Icons/alarms300x300v3.png", UriKind.Relative);
                oFliptile.BackgroundImage = new Uri("Icons/alarms300x300v3.png", UriKind.Relative);
                oFliptile.BackBackgroundImage = new Uri("Icons/alarms300x300medium.png", UriKind.Relative);
                oFliptile.WideBackgroundImage = new Uri("Icons/alarms691x300.png", UriKind.Relative);
                oFliptile.WideBackBackgroundImage = new Uri("Icons/alarms691x300.png", UriKind.Relative);
 
                oTile.Update(oFliptile);
            }
            else
            {
                // once it is created flip tile
                Uri tileUri = new Uri("/EditAutomaticAlarm.xaml?tile=flip", UriKind.Relative);
                ShellTileData tileData = this.CreateFlipTileData(DisplayDay, DisplayHour, DisplayMinute);
                ShellTile.Create(tileUri, tileData, true);
            }

        }

        private void LiveTileCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            DissableBackgroundAgent();
            ShellTile oTile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString()));
            if (oTile != null)
                oTile.Delete();
        }
        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }
        private ShellTileData CreateFlipTileData(String DisplayDay, String DisplayHour, String DisplayMinute)
        {
            return new FlipTileData()
            {
                Title = "AutoAlarm",
                BackTitle = "AutoAlarm",
                BackContent = DisplayDay + DisplayHour + DisplayMinute,
                WideBackContent = DisplayDay + DisplayHour + DisplayMinute,
                Count = 0,
                SmallBackgroundImage = new Uri("Icons/alarms300x300v3.png", UriKind.Relative),
                BackgroundImage = new Uri("Icons/alarms300x300v3.png", UriKind.Relative),
                BackBackgroundImage = new Uri("Icons/alarms300x300medium.png", UriKind.Relative),
                WideBackgroundImage = new Uri("Icons/alarms691x300.png", UriKind.Relative),
                WideBackBackgroundImage = new Uri("Icons/alarms691x300.png", UriKind.Relative),
            };
        }
    }
}