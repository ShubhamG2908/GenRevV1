using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class OpportunitiesAggregate
    {

        public int GroupEntityID { get; set; }
        public string GroupEntityName { get; set; }
        public decimal Potential { get; set; }
        public decimal CurrentOpportunity { get; set; }
        public decimal FutureOpportunity { get; set; }



        public static List<OpportunitiesAggregate> TruncateList(IEnumerable<OpportunitiesAggregate> items, int maxItems, bool appendTruncatedAsOther = true) {

            if (items.Count() > maxItems) {

                var truncatedList = items.Take(maxItems);

                if (!appendTruncatedAsOther) {
                    return truncatedList.ToList();
                }

                var nonvisibleList = items.Skip(maxItems);

                var combinedElements = new OpportunitiesAggregate();
                combinedElements.Potential = nonvisibleList.Sum(x => x.Potential);
                combinedElements.CurrentOpportunity = nonvisibleList.Sum(x => x.CurrentOpportunity);
                combinedElements.FutureOpportunity = nonvisibleList.Sum(x => x.FutureOpportunity);

                var finalList = truncatedList.ToList();
                finalList.Add(combinedElements);

                return finalList;

            } else {

                return items.ToList();
            }

        }

    }



}
