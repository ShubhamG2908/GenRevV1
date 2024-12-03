using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebOptionGroup
    {

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<WebOption> Options { get; set; }

    }
}
