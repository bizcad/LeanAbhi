using System.Collections.Generic;
using System.IO;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// Depricated
    /// </summary>
    public class FileDataImport
    {
        /// <summary>
        /// 
        /// </summary>
        public string _dataSourceUri { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string downloadedData { get; set; }
        /// <summary>
        /// Empty constructor
        /// </summary>
        public FileDataImport()
        {
            // Find the executing directory and look for the External Indicators file
            string dir = AssemblyLocator.ExecutingDirectory();
            _dataSourceUri = dir + "file.csv";
            if (!File.Exists(_dataSourceUri))
            {
                throw new FileNotFoundException("External Indicators file not found: " + _dataSourceUri);
            }
        }
        /// <summary>
        /// Parameter constructor supplying the dataSourceUri
        /// </summary>
        /// <param name="dataSourceUri">the path to the External Indicators csv file</param>
        public FileDataImport(string dataSourceUri)
        {
            _dataSourceUri = dataSourceUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<AbhiGoalCustomData> GetData()
        {
            int count = 0;
            var list = new List<AbhiGoalCustomData>();
            using (StreamReader inputStream = new StreamReader(_dataSourceUri))
            {
                downloadedData = inputStream.ReadToEnd();
                var inputProcessor = new AhiGoalLineParser();
                list = inputProcessor.ProcessStreamIntoList(downloadedData);
            }
            // For Development just get a few symbols
            // return list.Where(s => s.symbol == "FAST" || s.symbol == "DNB" || s.symbol == "JNJ" || s.symbol == "LLL").Cast<AbhiGoalCustomData>().ToList(); 
            return list;
        }
    }
}
