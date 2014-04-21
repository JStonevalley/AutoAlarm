using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Alarm
{
    public class Tunes
    {
        private Dictionary<string, Uri> tunes = new Dictionary<string, Uri>();

        public Tunes()
        {
            tunes.Add("Bird Song", new Uri("Tunes/BirdAlarm.wma", UriKind.Relative));
            tunes.Add("Extreme Mix", new Uri("Tunes/ExtremeAlarm.wma", UriKind.Relative));
            tunes.Add("Good Morning", new Uri("Tunes/GoodMorning.wma", UriKind.Relative));
            tunes.Add("Happy", new Uri("Tunes/Happy.wma", UriKind.Relative));
            tunes.Add("Magical Bells", new Uri("Tunes/MagicalBells.wma", UriKind.Relative));
            tunes.Add("Military Trumpet", new Uri("Tunes/MilitaryTrumpet.wma", UriKind.Relative));
            tunes.Add("Nature", new Uri("Tunes/Nature.wma", UriKind.Relative));
            tunes.Add("Piano Twinkle", new Uri("Tunes/PianoTwinkle.wma", UriKind.Relative));
            tunes.Add("Pink Panther", new Uri("Tunes/PinkPanther.wma", UriKind.Relative));
            tunes.Add("River Flows in You", new Uri("Tunes/RiverFlowsInYou.wma", UriKind.Relative));
            tunes.Add("Slow Morning", new Uri("Tunes/SlowMorning.wma", UriKind.Relative));
            tunes.Add("Sweet", new Uri("Tunes/Sweet.wma", UriKind.Relative));
            tunes.Add("Titanic", new Uri("Tunes/Titanic.wma", UriKind.Relative));
        }
    public Uri getTune (string tune)
    {
        Uri retTune;
        tunes.TryGetValue(tune, out retTune);
        return retTune;
    }
    public string getKey(Uri source)
    {
        string key = "";
        var tune = tunes.Where(p => p.Value == source).Select(p => p.Key);
        foreach (var keyCandidate in tune)
            key = keyCandidate;
        return key;
    }
    public string[] getKeys()
    {
        return tunes.Keys.ToArray();
    }
    }
}
