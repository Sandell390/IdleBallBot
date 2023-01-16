using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdleBallBot
{
    public class MoneyFormat
    {
        public float Digits { get; set; }
        
        public MoneyType Type { get; set; }

        public enum MoneyType
        {
            Undefined,
            None,
            K,
            M,
            B,
            T
        }


        public MoneyFormat(string MoneyString)
        {
            MoneyString = MoneyString.Replace("$", "");
            MoneyString = MoneyString.Replace(".",",");

            string moneyType = "Error";
            if (!int.TryParse(MoneyString[MoneyString.Length - 1].ToString(), out int res))
            {
                moneyType = MoneyString[MoneyString.Length - 1].ToString();
                MoneyString = MoneyString.Remove(MoneyString.Length - 1, 1);
            }

            float.TryParse(MoneyString, out float res1);
            


            Digits = res1;
            switch (moneyType.ToLower())
            {
                case "":
                    Type = MoneyType.None;
                    break;
                case "k":
                    Type = MoneyType.K;
                    break;
                case "m":
                    Type = MoneyType.M;
                    break;
                case "b":
                    Type = MoneyType.B;
                    break;
                case "t":
                    Type = MoneyType.T;
                    break;
                default:
                    Type = MoneyType.Undefined;
                    break;
            }
        }

        public override string ToString()
        {
            string _string = Digits.ToString();

            switch (Type)
            {
                case MoneyType.Undefined:
                    return _string + "Error";
                case MoneyType.None:
                    return _string;
                case MoneyType.K:
                    return _string + " K";
                case MoneyType.M:
                    return _string + " M";
                case MoneyType.B:
                    return _string + " B";
                case MoneyType.T:
                    return _string + " T";
            }

            return _string;
        }
    }
}
