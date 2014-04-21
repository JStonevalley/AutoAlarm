using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace Automatic_Alarm
{
    public class ListItemAlarm : INotifyPropertyChanged
    {
        private string name;
        private string day;
        private string time;
        private string tune;
        private DateTime date;
        private string european;
        private Visibility visibility;
        private Visibility checkBoxVisibility;
        private bool checktick;
        private int checkboxShowWidth;
        private int checkboxAdjustment;

        public ListItemAlarm(string inName, DateTime inDate, Uri sound)
        {
            string hour;
            string min;
            Date = inDate;
            Name = inName;
            Day = inDate.DayOfWeek.ToString();
            Tune = App.tuneList.getKey(sound);
            Visibility = System.Windows.Visibility.Collapsed;
            CheckBoxVisibility = System.Windows.Visibility.Collapsed;
            Checktick = false;
            CheckboxShowWidth = 20;
            CheckboxAdjustment = 220;

            if (inDate.Hour < 10)
            {
                hour = "0" + inDate.Hour.ToString();
            }
            else
            {
                hour = inDate.Hour.ToString();
            }
            if (inDate.Minute < 10)
            {
                min = "0" + inDate.Minute.ToString();
            }
            else
            {
                min = inDate.Minute.ToString();
            }
            Time = hour + ":" + min;

        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                NotifyPropertyChanged("Name");
            }
        }
        public string Day
        {
            get
            {
                return day;
            }
            set
            {
                day = value;
                NotifyPropertyChanged("Day");
            }

        }
        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                time = value;
                NotifyPropertyChanged("Time");
            }
        }
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                European = date.ToString("MM/dd/yyyy");
                NotifyPropertyChanged("Date");
            }
        }
        public Visibility Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                NotifyPropertyChanged("Visibility");
            }
        }
        public Visibility CheckBoxVisibility
        {
            get
            {
                return checkBoxVisibility;
            }
            set
            {
                if (value == System.Windows.Visibility.Visible)
                    CheckboxShowWidth = 0;
                else
                    CheckboxShowWidth = 20;
                checkBoxVisibility = value;
                NotifyPropertyChanged("CheckBoxVisibility");
            }
        }
        public string European
        {
            get
            {
                return european;
            }
            set
            {
                european = value;
                NotifyPropertyChanged("European");
            }
        }
        public string Tune
        {
            get
            {
                return tune;
            }
            set
            {
                tune = value;
                NotifyPropertyChanged("Tune");
            }
        }
        public bool Checktick
        {
            get
            {
                return checktick;
            }
            set
            {
                checktick = value;
                NotifyPropertyChanged("Checktick");
            }
        }
        public int CheckboxShowWidth
        {
            get
            {
                return checkboxShowWidth;
            }
            set
            {
                checkboxShowWidth = value;
                NotifyPropertyChanged("CheckboxShowWidth");
            }
        }
        public int CheckboxAdjustment
        {
            get
            {
                return checkboxAdjustment;
            }
            set
            {
                checkboxAdjustment = value;
                NotifyPropertyChanged("CheckboxAdjustment");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
