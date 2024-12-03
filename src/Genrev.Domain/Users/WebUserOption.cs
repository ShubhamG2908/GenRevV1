using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebUserOption
    {

        public int ID { get; set; }
        public int UserID { get; set; }
        public string OptionName { get; set; }
        public string ValueRaw { get; set; }

        //public virtual WebOption Option { get; set; }
        //public virtual User User { get; set; }


    }
}
