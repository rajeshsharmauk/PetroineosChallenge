using System.Collections.Generic;
using System.IO;

namespace PetroineosChallenge.Server
{
    /// <summary>
    /// Represents a file to be written to.
    /// </summary>
    public class OutputFile
    {
        private string _fileName;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">Name of file including the path</param>
        public OutputFile(string fileName)
        {
            _fileName = fileName;
        }
        /// <summary>
        /// Writes totals to the file.
        /// </summary>
        /// <param name="totals"></param>
        public void Write(IEnumerable<TotalPerTimePeriod> totals)
        {            
            using (var file = new StreamWriter(_fileName))
            {
                file.WriteLine("Local Time,Volume");

                foreach (var item in totals)
                {
                    file.WriteLine($"{item.TimePeriod},{item.Total}");
                }
            }
        }        
    }
}
