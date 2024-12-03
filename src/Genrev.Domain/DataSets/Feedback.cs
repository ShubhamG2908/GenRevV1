using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class Feedback
    {
        public int ID { get; set; }
        public int SubmittedBy { get; set; }
        public DateTime SubmittedTime { get; set; }
        public string MessageText { get; set; }

    }
}
