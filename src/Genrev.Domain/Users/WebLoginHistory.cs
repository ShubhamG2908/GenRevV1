using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebLoginHistory
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int UserID { get; set; }
        public string Activity { get; set; }
        public DateTime DateOfAction { get; set; }
        public string Note { get; set; }

        public User User { get; set; }
    }
}
