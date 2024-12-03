using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class FilterParams
    {

        public int[] PersonnelIDs { get; set; }
        public int[] CustomerTypeIDs { get; set; }
        public int[] IndustryIDs { get; set; }
        public int[] AccountTypeIDs { get; set; }
        public int[] ProductIDs { get; set; }
        public int[] CustomerIDs { get; set; }
        
    }
}
