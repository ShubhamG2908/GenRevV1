using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.Importing
{
    public class ComparisonResults<T>
    {
        
        public List<T> NoChanges { get; set; }
        public List<T> Updates { get; set; }
        public List<T> Inserts { get; set; }
        public List<T> Deletes { get; set; }
        public List<T> Inconclusive { get; set; }

    }
}
