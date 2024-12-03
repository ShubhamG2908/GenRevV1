using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.Personnel.Models
{

    [XmlRoot(ElementName = "Request")]
    public class PersonnelPostRequestModel
    {

        public List<Person> Personnel { get; set; }

    }


    public class Person
    {
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}