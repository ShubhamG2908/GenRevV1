using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;

using System.Web.Configuration;
using System.Web.Security;
    


namespace Genrev.Web.Infrastructure.Membership
{
    public sealed class DymengRoleProvider : RoleProvider
    {

        private IRoleRepository _roleRepository;

        public DymengRoleProvider() {
            _roleRepository = new RoleRepository();
        }

        public DymengRoleProvider(IRoleRepository roleRepository) {
            _roleRepository = roleRepository;
        }




        public override string ApplicationName {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles() {
            throw new NotImplementedException();    
        }

        public override string[] GetRolesForUser(string username) {
            return _roleRepository.GetUserRoleNames(username);
        }

        public override string[] GetUsersInRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName) {
            string[] roles = _roleRepository.GetUserRoleNames(username);
            return roles.Contains(roleName) ? true : false;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName) {
            throw new NotImplementedException();
        }
    }
}