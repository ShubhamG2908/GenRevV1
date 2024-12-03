using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Genrev.Domain.Accounts
{
    public class ApiIpWhitelistItem
    {

        public int ID { get; set; }

        public int AccountID { get; set; }

        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }

        public IPAddress GetIPFromString(string ipString) {
            string[] s = ipString.Split('.');
            byte[] b = new byte[3];
            b[0] = byte.Parse(s[0]);
            b[1] = byte.Parse(s[1]);
            b[2] = byte.Parse(s[2]);
            b[3] = byte.Parse(s[3]);
            return new IPAddress(b);
        }
                   

        public virtual Account Account { get; set; }

        public bool IsInRange(string rangeTest) {
            
            try {

                IPAddress test = GetIPFromString(rangeTest);

                IPAddressRange range = new IPAddressRange(GetIPFromString(RangeStart), GetIPFromString(RangeEnd));

                return range.IsInRange(test);

            } catch {
                
                return false;
            }

        }

    }
}
