using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain
{

    public enum Month
    {
        January = 1,
        Feburary = 2,
        March = 3,
        April = 4,
        May = 5,
        June = 6,
        July = 7,
        August = 8,
        September = 9,
        October = 10,
        November = 11,
        December = 12
    }


    public static class General
    {

        public static decimal? FixDollars(decimal? input) {

            if (!input.HasValue) {
                return null;
            }

            var d = decimal.Parse(string.Format("{0:0.0000}", input));
            return d;
        }

    }




}
