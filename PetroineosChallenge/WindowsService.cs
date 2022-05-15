using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Timers;
using System.IO;
using log4net;
using PetroineosChallenge.Server;
using Services;

namespace PetroineosChallenge
{
    /// <summary>
    /// Implementation of the windows service.
    /// Author: Rajesh Sharma
    /// Date: May 2022
    /// </summary>
    public partial class WindowsService : ServiceBase
    {
        private static readonly ILog _log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Processor _processor;
        private Service _service = new Service();
        private Timer _timer;

        public WindowsService()
        {
            InitializeComponent();
            ServiceName = "PetroineosChallenge";
        }

        private async void FetchAndWriteTotals()
        {
            // Asychronously fetch trades from the power service.
            IEnumerable<PowerTrade> powerTrades = await _service.GetTradesAsync(DateTime.Today);

            if (powerTrades != null)
            {
                // Retrieve totals.
                List<TotalPerTimePeriod> totals = _processor.GetTotals(powerTrades);

                // Write totals to file.
                if (totals.Any())
                {
                    _processor.WriteToFile(totals);
                }
            }
        }
        /// <summary>
        /// Creates and starts the timer.
        /// </summary>
        private void SetTimer()
        {
            _timer = new Timer()
            {
                Enabled = true,
                Interval = Properties.Settings.Default.TimerInterval * 1000
            };

            _timer.Elapsed += (sender, args) =>
            {
                try
                {
                    FetchAndWriteTotals();

                    // Get timer interval from app.config.
                    //_timer.Interval = Properties.Settings.Default.TimerInterval * 1000;
                }
                catch (Exception e)
                {
                    // Log an error and carry on.
                    _log.Error(e);
                }
            };
        }
        /// <summary>
        /// Handles startup of the windows service.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            _log.Info("Service starting...");

            _service = new Service();

            // Retrieve file path from app.config.
            string directory = Properties.Settings.Default.CsvFilePath;

            // Create directory if it doesn't already exist.
            Directory.CreateDirectory(directory);

            // Instantiate processor to process the power trades.
            _processor = new Processor()
            {
                CsvFilePath = directory
            };

            FetchAndWriteTotals();

            SetTimer();
        }

        protected override void OnStop()
        {
            _log.Info("Service stopping...");
            _timer.Enabled = false;
        }

        /// <summary>
        /// The main entry point for the application. To run in
        /// debug mode, specify /debug as a command parameter.
        /// </summary>
        static void Main(string[] args)
        {
            // Capture any unhandled exceptions.
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;

                if (ex == null)
                {
                    _log.Info($"Unsupported exception detected: {e.ExceptionObject}");
                }
                else
                {
                    _log.Info($"Exception detected: {e.ExceptionObject}");
                }
            };

            var service = new WindowsService();

            // Parse command line args
            if (args.Length > 0 && args[0].ToLower() == "/debug")
            {
                // Run in non-service mode
                string[] remainingArgs = new string[args.Length - 1];

                if (args.Length > 1)
                {
                    Array.Copy(args, 1, remainingArgs, 0, args.Length - 1);
                }

                service.OnStart(args);
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.\n");
                Console.ReadLine();
                service.OnStop();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
