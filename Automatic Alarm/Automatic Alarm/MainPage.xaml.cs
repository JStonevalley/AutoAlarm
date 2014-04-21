using System;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Phone.Scheduler;

namespace Automatic_Alarm
{
    public partial class MainPage : Automatic_Alarm.PhoneApplicationPageDelete
    {
        private IEnumerable<ScheduledNotification> notifications;
        public ObservableCollection<ListItemAlarm> alarmsDisplay = new ObservableCollection<ListItemAlarm>();
        public ObservableCollection<ListItemAlarm> RecurringAlarmsDisplay = new ObservableCollection<ListItemAlarm>();

        public MainPage()
        {
            InitializeComponent();
        }
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    HubTileService.UnfreezeGroup("alarmHubs");
                    ApplicationBar = ((ApplicationBar)Application.Current.Resources["AppBarMain"]);
                    break;

                case 1:
                    ApplicationBar = ((ApplicationBar)Application.Current.Resources["AppBarList"]);
                    HubTileService.FreezeGroup("alarmHubs");
                    break;
            }
        }
        private void ResetItemsList()
        {
            notifications = ScheduledActionService.GetActions<ScheduledNotification>();

            alarmsDisplay.Clear();
            RecurringAlarmsDisplay.Clear();

            DateTime now = DateTime.Now;
            for (int i = 0; i < notifications.Count(); i++)
            {
                Alarm alarm = (Alarm)notifications.ElementAt(i);
                if (alarm.ExpirationTime < now)
                    ScheduledActionService.Remove(alarm.Name);
                else if (alarm.RecurrenceType == RecurrenceInterval.None)
                    alarmsDisplay.Add(new ListItemAlarm(alarm.Name, alarm.BeginTime, alarm.Sound));
                else if (alarm.RecurrenceType == RecurrenceInterval.Daily)
                    RecurringAlarmsDisplay.Add(new ListItemAlarm(alarm.Name, alarm.BeginTime, alarm.Sound));
            }
            if (alarmsDisplay.Count() == 0 && RecurringAlarmsDisplay.Count() == 0)
            {
                EmptyStackPanel.Visibility = System.Windows.Visibility.Visible;
                NotificationListBox1.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox2.Visibility = System.Windows.Visibility.Collapsed;
                RecurringAlarmsLabel.Visibility = System.Windows.Visibility.Collapsed;
                AlarmsLabel.Visibility = System.Windows.Visibility.Collapsed;
            }
            else if (alarmsDisplay.Count() == 0)
            {
                EmptyStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox1.Visibility = System.Windows.Visibility.Visible;
                RecurringAlarmsLabel.Visibility = System.Windows.Visibility.Visible;
                AlarmsLabel.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox1.Height = double.NaN;
            }
            else if (RecurringAlarmsDisplay.Count() == 0)
            {
                EmptyStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox2.Visibility = System.Windows.Visibility.Visible;
                AlarmsLabel.Visibility = System.Windows.Visibility.Visible;
                RecurringAlarmsLabel.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox1.Height = 0;
            }
            else
            {
                EmptyStackPanel.Visibility = System.Windows.Visibility.Collapsed;
                NotificationListBox1.Visibility = System.Windows.Visibility.Visible;
                NotificationListBox2.Visibility = System.Windows.Visibility.Visible;
                RecurringAlarmsLabel.Visibility = System.Windows.Visibility.Visible;
                AlarmsLabel.Visibility = System.Windows.Visibility.Visible;
                NotificationListBox1.Height = double.NaN;

            }
            alarmsDisplay = new ObservableCollection<ListItemAlarm>(alarmsDisplay.OrderBy(x => x.Date));
            RecurringAlarmsDisplay = new ObservableCollection<ListItemAlarm>(RecurringAlarmsDisplay.OrderBy(x => x.Date));
            NotificationListBox1.ItemsSource = RecurringAlarmsDisplay;
            NotificationListBox2.ItemsSource = alarmsDisplay;
        }
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("flip".ToString())) == null)
            {
                string periodicTaskName = "PeriodicAgent";
                PeriodicTask periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
                if (periodicTask != null)
                {
                    RemoveAgent(periodicTaskName);
                }
            }
            QuoteHolder.Text = App.quotes.getQuote();
            ResetItemsList();
            setNextAlarmHolder();
            HubTileService.UnfreezeGroup("alarmHubs");
            NavigationService.RemoveBackEntry();
        }
        private void setNextAlarmHolder()
        {
            ListItemAlarm next = null; ;
            if (alarmsDisplay.Count == 0 && RecurringAlarmsDisplay.Count == 0)
            {
                NextAlarmHolder1.Text = "No alarm set.";
                NextAlarmHolder2.Text = "Sleep well!";
            }
            else if (alarmsDisplay.Count == 0)
            {
                next = RecurringAlarmsDisplay.ElementAt(0);
                foreach (ListItemAlarm tempNext in RecurringAlarmsDisplay)
                {
                    if (tempNext.Date < next.Date)
                        next = tempNext;
                }
                nextAlarmText(next);
            }
            else if (RecurringAlarmsDisplay.Count == 0)
            {
                next = alarmsDisplay.ElementAt(0);
                foreach (ListItemAlarm tempNext in alarmsDisplay)
                {
                    if (tempNext.Date < next.Date)
                        next = tempNext;
                }
                nextAlarmText(next);
            }
            else
            {
                next = alarmsDisplay.ElementAt(0);
                foreach (ListItemAlarm tempNext in alarmsDisplay)
                {
                    if (tempNext.Date < next.Date)
                        next = tempNext;
                }
                foreach (ListItemAlarm tempNext in RecurringAlarmsDisplay)
                {
                    if (tempNext.Date < next.Date)
                        next = tempNext;
                }
                nextAlarmText(next);
            }
                
        }
        public void nextAlarmText(ListItemAlarm next)
        {
            NextAlarmHolder1.Text = "Next alarm";
            if (next.Date.Day == DateTime.Now.Day)
                NextAlarmHolder2.Text = "Today at " + next.Time;
            else if (next.Date.Day == DateTime.Now.AddDays(1).Day)
                NextAlarmHolder2.Text = "Tomorrow at " + next.Time;
            else
                NextAlarmHolder2.Text = next.Day + " at " + next.Time;
        }
        public override void delete()
        {
            foreach (ListItemAlarm alarm in NotificationListBox2.SelectedItems)
            {
                ScheduledActionService.Remove(alarm.Name);
            }
            foreach (ListItemAlarm alarm in NotificationListBox1.SelectedItems)
            {
                ScheduledActionService.Remove(alarm.Name);
            }
            NotificationListBox1.IsSelectionEnabled = false;
            NotificationListBox2.IsSelectionEnabled = false;
            ResetItemsList();
            setNextAlarmHolder();
        }
        public override bool checkboxVisibility()
        {
            if (alarmsDisplay.Count() == 0 && RecurringAlarmsDisplay.Count() == 0)
                return false;
            return NotificationListBox2.IsSelectionEnabled = !NotificationListBox2.IsSelectionEnabled;
        }
        private void regular_alarm_Click(object sender, RoutedEventArgs e)
        {
            HubTileService.FreezeGroup("alarmHubs");
            NavigationService.Navigate(new Uri("/EditAlarm.xaml?start=start", UriKind.Relative));
        }

        private void automatic_alarm_Click(object sender, RoutedEventArgs e)
        {
            HubTileService.FreezeGroup("alarmHubs");
            NavigationService.Navigate(new Uri("/EditAutomaticAlarm.xaml", UriKind.Relative));
        }

        private void edit_btn_Click(object sender, RoutedEventArgs e)
        {
            HubTileService.FreezeGroup("alarmHubs");
            NavigationService.Navigate(new Uri("/EditAlarm.xaml?edit=" + ((Button)sender).Tag, UriKind.Relative));
        }
        public override void settings()
        {
            HubTileService.FreezeGroup("alarmHubs");
            NavigationService.Navigate(new Uri("/SettingsPage.xaml?" , UriKind.Relative));
        }
        public void setSettingsbuttnIsEnabled(bool enable)
        {
            ((ApplicationBarIconButton)(((ApplicationBar)Application.Current.Resources["AppBarList"]).Buttons[0])).IsEnabled = enable;
        }

        private void NotificationListBox2_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (NotificationListBox2.IsSelectionEnabled == false)
            {
                ((ApplicationBarIconButton)(((ApplicationBar)(Application.Current as App).Resources["AppBarList"]).Buttons[1])).IsEnabled = false;
                foreach (ListItemAlarm item in alarmsDisplay)
                {
                    item.CheckboxAdjustment = 220;
                }
                NotificationListBox1.IsSelectionEnabled = false;
            }
            else
            {
                ((ApplicationBarIconButton)(((ApplicationBar)(Application.Current as App).Resources["AppBarList"]).Buttons[1])).IsEnabled = true;
                foreach (ListItemAlarm item in alarmsDisplay)
                {
                    item.CheckboxAdjustment = 158;
                }
                NotificationListBox1.IsSelectionEnabled = true;
            }
        }
        private void NotificationListBox1_IsSelectionEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (NotificationListBox1.IsSelectionEnabled == false)
            {
                ((ApplicationBarIconButton)(((ApplicationBar)(Application.Current as App).Resources["AppBarList"]).Buttons[1])).IsEnabled = false;
                foreach (ListItemAlarm item in RecurringAlarmsDisplay)
                {
                    item.CheckboxAdjustment = 220;
                }
                NotificationListBox2.IsSelectionEnabled = false;
            }
            else
            {
                ((ApplicationBarIconButton)(((ApplicationBar)(Application.Current as App).Resources["AppBarList"]).Buttons[1])).IsEnabled = true;
                foreach (ListItemAlarm item in RecurringAlarmsDisplay)
                {
                    item.CheckboxAdjustment = 158;
                }
                NotificationListBox2.IsSelectionEnabled = true;
            }
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {            
           ListItemAlarm item = alarmsDisplay.First(x => x.Name.Equals((string)((Grid)sender).Tag));
           if (item.Visibility == System.Windows.Visibility.Collapsed)
           {
               foreach (ListItemAlarm a in alarmsDisplay)
               {
                   if (a.Visibility == System.Windows.Visibility.Visible)
                   {
                       a.Visibility = System.Windows.Visibility.Collapsed;
                       break;
                   }
               }
               foreach (ListItemAlarm a in RecurringAlarmsDisplay)
               {
                   if (a.Visibility == System.Windows.Visibility.Visible)
                   {
                       a.Visibility = System.Windows.Visibility.Collapsed;
                       break;
                   }
               }
               item.Visibility = System.Windows.Visibility.Visible;
           }
           else
               item.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ScheduledActionService.Remove((string)((Control)sender).Tag);
            NotificationListBox1.IsSelectionEnabled = false;
            NotificationListBox2.IsSelectionEnabled = false;
            ResetItemsList();
            setNextAlarmHolder();
        }
        
        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            HubTileService.FreezeGroup("alarmHubs");
            NavigationService.Navigate(new Uri("/EditAlarm.xaml?edit=" + ((Control)sender).Tag, UriKind.Relative));
        }

        private void Grid_Tap_Recurring(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListItemAlarm item = RecurringAlarmsDisplay.First(x => x.Name.Equals((string)((Grid)sender).Tag));
            if (item.Visibility == System.Windows.Visibility.Collapsed)
            {
                foreach (ListItemAlarm a in RecurringAlarmsDisplay)
                {
                    if (a.Visibility == System.Windows.Visibility.Visible)
                    {
                        a.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                }
                foreach (ListItemAlarm a in alarmsDisplay)
                {
                    if (a.Visibility == System.Windows.Visibility.Visible)
                    {
                        a.Visibility = System.Windows.Visibility.Collapsed;
                        break;
                    }
                }
                item.Visibility = System.Windows.Visibility.Visible;
            }
            else
                item.Visibility = System.Windows.Visibility.Collapsed;
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
    }
}