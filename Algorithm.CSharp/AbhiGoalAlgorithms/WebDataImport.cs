using System.Collections.Generic;
using System.IO;
using System.Net;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// Imports data from the web
    /// </summary>
    public class WebDataImport
    {
        private const string DATA_SOURCE_URI = @"http://54.69.46.205/filter_csv_withcache.php?sdate=1/1/2010&edate=7/6/2015&knowhen=31,41&rd_pct=83,92,100&pid=9";
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
        public WebDataImport()
        {
            _dataSourceUri = DATA_SOURCE_URI;
        }
        /// <summary>
        /// Constructor specifying the data source uri
        /// </summary>
        /// <param name="dataSourceUri"></param>
        public WebDataImport(string dataSourceUri)
        {
            _dataSourceUri = dataSourceUri;
        }

        /// <summary>
        /// Retrieves the data from the web
        /// </summary>
        /// <returns>a list of AbhiGoalCustomData objects, one for each row in the csv file</returns>
        public List<AbhiGoalCustomData> GetData()
        {
            int count = 0;
            var list = new List<AbhiGoalCustomData>();

            using (WebClient wClient = new WebClient())
            {
                downloadedData = wClient.DownloadString(_dataSourceUri);
            }
            var inputProcessor = new AhiGoalLineParser();
            list = inputProcessor.ProcessStreamIntoList(downloadedData);

            //return list.Where(s => s.symbol == "FAST" || s.symbol == "DNB" || s.symbol == "JNJ" || s.symbol == "LLL").Cast<AbhiGoalCustomData>().ToList(); 
            return list;
        }
    }
}
