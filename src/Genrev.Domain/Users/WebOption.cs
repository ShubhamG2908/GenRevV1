using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebOption
    {

        public string Name { get; set; }
        public string GroupName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DefaultValueRaw { get; set; }

        public virtual WebOptionGroup OptionGroup { get; set; }
        public virtual ICollection<WebUserOption> UserOptions { get; set; }

    }
}
