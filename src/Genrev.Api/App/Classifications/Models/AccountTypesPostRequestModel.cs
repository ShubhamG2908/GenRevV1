using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Xml.Serialization;

namespace Genrev.Api.App.Classifications.Models
{
    [XmlRoot(ElementName = "Request")]
    public class AccountTypesPostRequestModel
    {
        public List<AccountType> AccountTypes { get; set; }
    }


    public class AccountType
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int? CallsPerMonthGoal { get; set; }
    }

}