using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using Services;

namespace PetroineosChallenge.Server
{
    /// <summary>
    /// Provides methods to call the PowerService.
    /// </summary>
    public class Service
    {
        private readonly PowerService _powerService;

        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Service()
        {
            _powerService = new PowerService();
        }
        /// <summary>
        /// Fetches trades from the PowerService.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<PowerTrade> GetTrades(DateTime date)
        {
            try
            {
                return _powerService.GetTrades(date);
            }
            catch (PowerServiceException e)
            {
                _log.Error(e);
                return null;
            }            
        }
        /// <summary>
        /// Fetches trades from the PowerService asynchronously.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PowerTrade>> GetTradesAsync(DateTime date)
        {
            try
            {
                return await Task.Run(() => _powerService.GetTrades(date));
            }
            catch (PowerServiceException e)
            {
                _log.Error(e);
                return null;
            }
        }
    }
}
