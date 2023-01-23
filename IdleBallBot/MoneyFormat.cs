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
            None,
            K,
            M,
            B,
            T,
            q,
            Undefined,
        }


        public MoneyFormat(string MoneyString)
        {
            MoneyString = MoneyString.Replace("$", "");
            MoneyString = MoneyString.Replace(".",",");

            string moneyType = "Error";
            if (MoneyString.Count() == 0)
            {
                Digits= 0;
                Type = MoneyType.Undefined;
                return;
            }
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
                case "q":
                    Type = MoneyType.q;
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
                case MoneyType.q:
                    return _string + " q";
            }

            return _string;
        }

        public static bool Compare(MoneyFormat money1, MoneyFormat money2)
        {
            if (money1.Type == MoneyType.Undefined || money2.Type == MoneyType.Undefined)
            {
                return false;
            }

            if (money1.Type > money2.Type)
            {
                return true;
            }

            if (money1.Type == money2.Type && money1.Digits > money2.Digits)
            {
                return true;
            }

            return false;
        }
    }
}
