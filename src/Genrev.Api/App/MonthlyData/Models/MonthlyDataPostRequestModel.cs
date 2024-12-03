using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Genrev.Api.App.MonthlyData.Models
{

    [XmlRoot(ElementName = "Request")]
    public class MonthlyDataPostRequestModel
    {
        public List<DataEntry> DataEntries { get; set; }
    }

    public class DataEntry
    {
        public DateTime Period { get; set; }
        public string CustomerID { get; set; }
        public string PersonnelID { get; set; }
        public decimal? ActualSales { get; set; }
        public decimal? ActualGPP { get; set; }
        public decimal? ActualCalls { get; set; }
    }
}