using System;
using System.Linq;
using System.Net;
using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.UserData;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace ScheduledTaskAgent1
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        private int StartHour;
        private int StartMin;
        private int LateHour;
        private int LateMin;
        private string Tune;
        private string[] specifics;
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        int AlarmsSet = 0;
        static ScheduledAgent()
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            specifics = task.Description.Split(new string[] { "split" }, StringSplitOptions.None);
            StartHour = int.Parse(specifics[0]);
            StartMin = int.Parse(specifics[1]);
            LateHour = int.Parse(specifics[2]);
            LateMin = int.Parse(specifics[3]);
            Tune = specifics[4];
            DateTime LateAlarmTime = new DateTime(2013, 1, 1, LateHour, LateMin, 0);
            IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();
            AlarmsSet = 0;
            if (notifications != null)
            {
                for (int i = 0; i < notifications.Count(); i++)
                {
                    if (notifications.ElementAt(i).ExpirationTime.TimeOfDay > notifications.ElementAt(i).BeginTime.AddSeconds(1).TimeOfDay && notifications.ElementAt(i).ExpirationTime.TimeOfDay < notifications.ElementAt(i).BeginTime.AddSeconds(5).TimeOfDay && notifications.ElementAt(i).BeginTime.TimeOfDay < LateAlarmTime.TimeOfDay)
                    {
                        AlarmsSet++;
                    }
                }
            }

            Appointments appts = new Appointments();

            appts.SearchCompleted += new EventHandler<AppointmentsSearchEventArgs>(Appointments_SearchCompleted_Background);

            DateTime start = DateTime.Now;
            start = start.AddHours(-start.Hour);
            start = start.AddMinutes(-start.Minute);
            DateTime end = DateTime.Now.AddDays(7);

            appts.SearchAsync(start, end, "Appointments Test #1");

        }
        void Appointments_SearchCompleted_Background(object sender, AppointmentsSearchEventArgs e)
        {
            try
            {
                List<Appointment> appointments = new List<Appointment>(e.Results);
                List<String> show = new List<String>();
                int i = 1;
                List<DateTime> alarmtimes = new List<DateTime>();
                DateTime temp = DateTime.Now;
                DateTime LateAlarmTime = new DateTime(2013, 1, 1, LateHour, LateMin, 0);

                int NumberOfAlarms = 0;

                while (appointments.Count >= i || 8 > i)
                {
                    temp = temp.AddDays(1);
                    Appointment app = appointments.Find(item => item.StartTime.Hour != 0 && temp.Day == item.StartTime.Day);
                    if (app != null && app.StartTime.AddHours(-StartHour).AddMinutes(-StartMin).TimeOfDay < LateAlarmTime.TimeOfDay)
                    {
                        NumberOfAlarms++;
                    }
                    i++;
                }
                int counter;
                if (NumberOfAlarms - AlarmsSet < 0)
                    counter = 0;
                else counter = NumberOfAlarms - AlarmsSet;

                IEnumerable<ScheduledNotification> notifications = ScheduledActionService.GetActions<ScheduledNotification>();
                DateTime now = DateTime.Now;
                ScheduledNotification next = null;
                if (notifications.Count() != 0)
                {
                    next = (Alarm)notifications.ElementAt(0);
                    for (int j = 0; j < notifications.Count(); j++)
                    {
                        Alarm alarm = (Alarm)notifications.ElementAt(j);
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
                    oFliptile.Count = counter;
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
            }
            catch (System.Exception)
            {
                //That's okay, no results
            }
        }
    }
}