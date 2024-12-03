using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class TopBottomMatrix
    {

        /// <summary>
        /// Primary Subject, e.g., 'Customer', 'Salesperson'
        /// </summary>
        public string Entity { get; set; }

        /// <summary>
        /// Metric Focus, e.g., 'Sales', 'Variance', 'Forecast'
        /// </summary>
        public string Factor { get; set; }
        
        /// <summary>
        /// Which series? E.g., 'Top', 'Bottom'
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// ID of the Entity
        /// </summary>
        public int EntityID { get; set; }

        /// <summary>
        /// Text name of the entity
        /// </summary>
        public string EntityName { get; set; }

        /// <summary>
        /// Value of the metric
        /// </summary>
        public decimal? EntityValue { get; set; }

    }
}
