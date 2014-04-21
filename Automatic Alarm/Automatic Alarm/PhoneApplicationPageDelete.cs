using System;
using System.Linq;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.UserData;

namespace Automatic_Alarm
{
    public abstract class PhoneApplicationPageDelete : PhoneApplicationPage
    {
        public abstract void delete();

        public abstract bool checkboxVisibility();

        public abstract void settings();
    }
}
