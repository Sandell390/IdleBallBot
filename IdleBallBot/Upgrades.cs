using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleBallBot
{
    public class Upgrades
    {
        public int Level { get; set; }
        public float LevelPower { get; set; }
        public MoneyFormat MoneyFormat { get; set; }

        public Upgrades(string[] strings)
        {
            Level = 0;
            LevelPower = 0;
            MoneyFormat = new MoneyFormat("$0");

            for (int i = 0; i < strings.Length; i++)
            {
                strings[i] = strings[i].ToLower();
                strings[i] = strings[i].Replace(".",",");
                if (strings[i].StartsWith("$"))
                {
                    MoneyFormat = new MoneyFormat(strings[i]);
                }
                else if (strings[i].ToLower().StartsWith("l"))
                {
                    string _ = strings[i];
                    _ = _.Replace("l","");
                    _ = _.Replace("v", "");
                    _ = _.Replace(".", "");
                    _ = _.Replace(",", "");
                    Level = int.Parse(_);
                }
                else if (float.TryParse(strings[i], out float res))
                {
                    LevelPower = res;
                }
            }
            

            
        }
    }
}
