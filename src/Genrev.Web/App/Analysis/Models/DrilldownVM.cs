using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Analysis.Models
{
    public class DrilldownVM
    {

        public List<DrilldownListItem> Items { get; set; }

        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }

        public bool ShowColumnGroupClassification { get; set; } = true;
        public bool ShowColumnGroupSales { get; set; } = true;
        public bool ShowColumnGroupGPD { get; set; } = false;
        public bool ShowColumnGroupGPP { get; set; } = true;
        public bool ShowColumnGroupCalls { get; set; } = true;

    }
}