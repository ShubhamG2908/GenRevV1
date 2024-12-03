using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Data;

namespace Genrev.Web.Infrastructure.Membership
{
    public interface IRoleRepository
    {
        string[] GetUserRoleNames(string username);

    }


    public class RoleRepository : IRoleRepository
    {

        GenrevContext dbContext;

        public RoleRepository() {
            dbContext = new GenrevContext();
        }
        
        public string[] GetUserRoleNames(string username) {

            var names = new List<string>();
            var ctx = new GenrevContext();
            var user = ctx.Users.Where(x => x.Email == username).FirstOrDefault();
            if(user == null) { return names.ToArray(); }

            var roles = ctx.Personnel.Where(x => x.ID == user.PersonnelID).FirstOrDefault()?.Roles;
            if(roles == null) { return names.ToArray(); }
            
            foreach (var role in roles) {
                names.Add(role.Name);
            }

            return names.ToArray();

        }

    }
}