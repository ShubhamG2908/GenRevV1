using System.Collections.Generic;

namespace Genrev.Web.App.Analysis.Models
{
    public class DrilldownVM
    {

        public List<DrilldownListItem> Items { get; set; }

        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }

        public bool ShowColumnGroupClassification { get; set; } = true;
        public bool ShowColumnGroupSales { get; set; } = true;
        public bool ShowColumnGroupGPD { get; set; } = true;
        public bool ShowColumnGroupGPP { get; set; } = true;
        public bool ShowColumnGroupCalls { get; set; } = true;
        public bool ShowColumnGroupMarketShare { get; set; } = true;

    }
}