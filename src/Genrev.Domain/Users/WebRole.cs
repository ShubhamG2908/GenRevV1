using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebRole
    {

        public string Name { get; set; }
        public string GroupName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual WebPermissionGroup PermissionGroup { get; set; }
    }
}
