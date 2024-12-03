using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Customers.Models
{

    [XmlRoot(ElementName = "Request")]
    public class CustomerPostRequestModel
    {
        public List<Customer> Customers { get; set; }
    }
    
    public class Customer
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string CustomerTypeID { get; set; }
        public string AccountTypeID { get; set; }
        public string IndustryTypeID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }
    }
}