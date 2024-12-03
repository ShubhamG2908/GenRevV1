using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Email
{
    public class SMTPAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string FromAddress { get; set; }
    }
}
