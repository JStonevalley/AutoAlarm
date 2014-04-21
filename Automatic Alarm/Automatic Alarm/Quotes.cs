using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic_Alarm
{
    public class Quotes
    {
        Random random = new Random();
        private string[] messages = new String[12];
        public Quotes()
        {
            messages[0] = "\"Have a nice day!\"";
            messages[1] = "\"Lose an hour in the morning,\nand you will be all day hunting for it.\"";
            messages[2] = "\"The average, healthy, well-adjusted adult\ngets up at seven-thirty in the morning\nfeeling just plain terrible.\"";
            messages[3] = "\"One key to success is to have lunch\nat the time of day most people have breakfast.\"";
            messages[4] = "\"Life is too short!\"\n\"Wake up earlier.\"";
            messages[5] = "\"Morning is wonderful.\nIts only drawback is that it comes\nat such an inconvenient time of day.\"";
            messages[6] = "\"You're not awake until you're actually\n out of bed and standing up.\"";
            messages[7] = "\"There is nowhere morning does not go.\"";
            messages[8] = "\"Some people dream of success while others\nwake up and work hard at it.\"";
            messages[9] = "\"Perseverance is a great element of success.\"";
            messages[10] = "\"When you rise in the morning,\nform a resolution to make the day\na happy one for a fellow creature.\"";
            messages[11] = "\"Don't wait for extraordinary opportunities. Seize\ncommon occasions and make them great.\"";
        }
        public string getQuote()
        {
            return messages[random.Next(12)];
        }

    }
}
