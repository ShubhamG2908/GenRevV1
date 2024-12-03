using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Domain.Collections;

namespace Genrev.Domain.Companies
{

    public class Person {
        
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        public virtual Company Company { get; set; }
        public virtual EntityCollection<Role> Roles { get; protected set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<DataSets.CustomerData> CustomerData { get; set; }
        public virtual ICollection<DataSets.CustomerDrilldown> CustomerDrilldowns { get; set; }
        public virtual ICollection<PersonnelAvailability> Availability { get; set; }
        

        public PersonnelAvailability GetAvailability(int year) {
            if (Availability == null) { return null; }
            return Availability.Where(x => x.AvailabilityYear == year).SingleOrDefault();
        }



        public string CommonName {
            get
            {
                if (FirstName != null && LastName != null) {
                    return FirstName + ' ' + LastName;
                }
                if (FirstName != null) {
                    return FirstName;
                }
                if (LastName != null) {
                    return LastName;
                }
                return null;                
            }
        }


        ///// <summary>
        ///// Adds the specified person as a next-level hierarchy person.
        ///// </summary>
        ///// <param name="person">Person to add as the next-level heirarchy</param>
        //public void AddHierarchyMember(Person person) {
        //    Hierarchy = EntityCollectionHelper.AddItem(Hierarchy, person);
        //}



        /// <summary>
        /// Add the specified role to this person.
        /// </summary>
        /// <param name="role">Role to add</param>
        public void AddRole(Role role) {
            Roles = EntityCollectionHelper.AddItem(Roles, role);
            // log role addition?
        }

        public void RemoveRole(Role role)
        {
            Roles = EntityCollectionHelper.RemoveItem(Roles, role);
        }

    }
}
