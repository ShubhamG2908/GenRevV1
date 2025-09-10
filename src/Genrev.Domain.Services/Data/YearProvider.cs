using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Genrev.DomainServices.Data
{
    public class YearProvider
    {
		public IEnumerable<int> GetDefaultYears()
		{
			int current = DateTime.Now.Year;
			int futureYears = 5;
			int start = current;
			int count =  futureYears + 1	; // inclusive of current
			return Enumerable.Range(start, count);
		}

		public int GetDefaultYear()
		{
			return DateTime.UtcNow.Year;
		}

		// Optional overload if callers want custom ranges
		public IEnumerable<int> GetDefaultYears(int pastYears, int futureYears)
		{
			int current = DateTime.Now.Year;
			int start = current - Math.Max(0, pastYears);
			int count = Math.Max(1, pastYears + futureYears + 1);
			return Enumerable.Range(start, count);
		}

	}
}
