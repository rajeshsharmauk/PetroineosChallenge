using System;
using System.Collections.Generic;
using System.Linq;
using Services;
using log4net;

namespace PetroineosChallenge.Server
{  
    /// <summary>
    /// Provides the main processing for retrieving data from
    /// the PowerService and writing to a csv file.
    /// </summary>
    public class Processor
    {
        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Storage for time periods.
        /// </summary>
        private readonly Dictionary<int, string> _timePeriods;

        /// <summary>
        /// File to be written to.
        /// </summary>
        private OutputFile _outputFile;

        /// <summary>
        /// Path of the output file.
        /// </summary>
        public string CsvFilePath { get; set; }

        public Processor()
        {
            //_service = service;
            _timePeriods = CreateTimePeriods();
        }

        /// <summary>
        /// Fetch totals from the PowerService.
        /// </summary>
        /// <returns>A list of TotalPerTimePeriod objects.</returns>
        public List<TotalPerTimePeriod> GetTotals(IEnumerable<PowerTrade> powerTrades)
        {
            try
            {
                // Flatten the result to get the data in periods.
                IEnumerable<PowerPeriod> periods = powerTrades.SelectMany(x => x.Periods);

                // Obtain the sum of volumes per period.
                var result = periods.GroupBy(x => x.Period).Select(y => new
                {
                    Period = y.Key,
                    Total = y.Sum(z => z.Volume)
                });

                var totalsPerTimePeriod = new List<TotalPerTimePeriod>();

                foreach (var item in result)
                {
                    string timePeriod = string.Empty;

                    // Fetch a time period from a numerical value.
                    if (!_timePeriods.TryGetValue(item.Period, out timePeriod))
                    {
                        _log.Error($"Time period {item.Period} not found in dictionary");
                    }

                    // Create an object to store the time period and it's total volume.
                    TotalPerTimePeriod totalPerTimePeriod = new TotalPerTimePeriod
                    {
                        TimePeriod = timePeriod,
                        Total = Math.Round(item.Total, 2)
                    };

                    // Add to list.
                    totalsPerTimePeriod.Add(totalPerTimePeriod);
                }

                return totalsPerTimePeriod;
            }
            catch (PowerServiceException e)
            {
                throw new Exception("Error in PowerService.GetTrades", e);
            }
            catch (Exception e)
            {
                throw new Exception("Unknown error in PowerService.GetTrades", e);
            }
        }
        /// <summary>
        /// Initiates the writing of totals to a file.
        /// </summary>
        /// <param name="totals">A list of totals per time period.</param>
        public void WriteToFile(IEnumerable<TotalPerTimePeriod> totals)
        {
            string filename = $"{CsvFilePath}PowerPosition_{DateTime.Today.ToString("yyyyMMdd")}_{DateTime.Now:HHmm}.csv";

            _outputFile = new OutputFile(filename);
            _outputFile.Write(totals);
        }
        /// <summary>
        /// Creates a dictionary with a mapping between a numerical time period
        /// and it's string representation.
        /// </summary>
        /// <returns>Dictionary of integer-to-string mappings.</returns>
        private Dictionary<int, string> CreateTimePeriods()
        {
            Dictionary<int, string> timePeriods = new Dictionary<int, string>
            {
                {1, "23:00"},
                {2, "24:00"},
                {3, "01:00"},
                {4, "02:00"},
                {5, "03:00"},
                {6, "04:00"},
                {7, "05:00"},
                {8, "06:00"},
                {9, "07:00"},
                {10, "08:00"},
                {11, "09:00"},
                {12, "10:00"},
                {13, "11:00"},
                {14, "12:00"},
                {15, "13:00"},
                {16, "14:00"},
                {17, "15:00"},
                {18, "16:00"},
                {19, "17:00"},
                {20, "18:00"},
                {21, "19:00"},
                {22, "20:00"},
                {23, "21:00"},
                {24, "22:00"}
            };

            return timePeriods;
        }
    }
}
