using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class HistoricSales : DataAggregateBase
    {
        public int FiscalYear { get; set; }
        
    }
}
