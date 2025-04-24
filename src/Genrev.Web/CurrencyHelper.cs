using System;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Genrev.Web
{
    public static class CurrencyHelper
    {
        public static string SetGlobalCulture(string countryCode = "US")
        {
            // Get all specific cultures
            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            var currentCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();



            // Try to find a matching culture for the given country code
            CultureInfo newCulture = cultures
                .FirstOrDefault(c =>
                {
                    try
                    {
                        var region = new RegionInfo(c.Name);
                        return string.Equals(region.TwoLetterISORegionName, countryCode, StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                });

            // Set the currency symbol from the region info
            var regionInfo = new RegionInfo(newCulture.Name);
            // Update only the currency symbol
            currentCulture.NumberFormat.CurrencySymbol = regionInfo.CurrencySymbol;

            // Set the global culture
            //Thread.CurrentThread.CurrentCulture = currentCulture;
            //Thread.CurrentThread.CurrentUICulture = currentCulture;

            return regionInfo.CurrencySymbol;
        }
    }
}