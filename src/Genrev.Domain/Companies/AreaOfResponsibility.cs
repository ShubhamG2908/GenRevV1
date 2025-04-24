using System;

namespace Genrev.Domain.Companies
{
    public class AreaOfResponsibility
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CompanyID { get; set; }
        public string Name { get; set; }
        public string ClientID { get; set; }
        public virtual Company Company { get; set; }
    }
}