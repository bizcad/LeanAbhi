using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using Microsoft.SqlServer.Server;
using QuantConnect.Data;
using QuantConnect.Logging;
using QuantConnect.Notifications;
using QuantConnect.Util;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// The class for an Abhi Goal's Custom Data with source and reader.
    /// </summary>
    public class AbhiGoalCustomData : BaseData
    {
        private const string DATA_SOURCE_URI =
            @"http://54.69.46.205/filter_csv_withcache.php?sdate={0}&edate={1}&knowhen=31,41&rd_pct=83,92,100&pid=9&cacheonly=1";

        private List<string> urls;


        /// <summary>
        /// Field for the named column
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public DateTime next_date { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public decimal next_close { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string symbol { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public decimal close { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public int kno { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string w { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string h { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string e { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string n { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string rd { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public decimal pct { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string cr { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string bsval1 { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string bsval2 { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string bsval3 { get; set; }
        /// <summary>
        /// Field for the named column
        /// </summary>
        public string bsval4 { get; set; }


        /// <summary>
        /// 1. DEFAULT CONSTRUCTOR: Custom data types need a default constructor.
        /// We search for a default constructor so please provide one here. It won't be used for data, just to generate the "Factory".
        /// </summary>
        public AbhiGoalCustomData()
        {
            Symbol = "ABHIGOAL";
            urls = new List<string>();
        }

        /// <summary>
        /// 2. RETURN THE STRING URL SOURCE LOCATION FOR YOUR DATA:
        /// This is a powerful and dynamic select source file method. If you have a large dataset, 10+mb we recommend you break it into smaller files. E.g. One zip per year.
        /// We can accept raw text or ZIP files. We read the file extension to determine if it is a zip file.
        /// </summary>
        /// <param name="config">Configuration object</param>
        /// <param name="date">Date of this source file</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>String URL of source file.</returns>
        public override SubscriptionDataSource GetSource(SubscriptionDataConfig config, DateTime currentDate, bool isLiveMode)
        {
            if (isLiveMode)
            {
                return new SubscriptionDataSource(string.Format(DATA_SOURCE_URI, currentDate.ToShortDateString(), currentDate.ToShortDateString()), SubscriptionTransportMedium.RemoteFile);
                //return new SubscriptionDataSource("https://www.bitstamp.net/api/ticker/", SubscriptionTransportMedium.Rest);
            }


            // OR simply return a fixed small data file. Large files will slow down your backtest
            FileDataImport fi = new FileDataImport();
            string uri = string.Format(DATA_SOURCE_URI, currentDate.ToShortDateString(), currentDate.ToShortDateString());
            string logentry = string.Format("{0}:{1}:{2}, {3}", DateTime.Now.Minute, DateTime.Now.Second,
                DateTime.Now.Millisecond, uri);
            //urls.Add(logentry);
            //WriteLog(logentry);
            var source = new SubscriptionDataSource(uri, SubscriptionTransportMedium.RemoteFile);
            return source;
            //return new SubscriptionDataSource(fi._dataSourceUri, SubscriptionTransportMedium.LocalFile);

        }

        /// <summary>
        /// 3. READER METHOD: Read 1 line from data source and convert it into Object.
        /// Each line of the CSV File is presented in here. The backend downloads your file, loads it into memory and then line by line
        /// feeds it into your algorithm
        /// </summary>
        /// <param name="line">string line from the data source file submitted above</param>
        /// <param name="config">Subscription data, symbol name, data type</param>
        /// <param name="currentDate">Current date we're requesting. This allows you to break up the data source into daily files.</param>
        /// <param name="isLiveMode">true if we're in live mode, false for backtesting mode</param>
        /// <returns>New Bitcoin Object which extends BaseData.</returns>
        public override BaseData Reader(SubscriptionDataConfig config, string line, DateTime currentDate, bool isLiveMode)
        {
            var instruction = new AbhiGoalCustomData();
            if (isLiveMode)
            {
                //Example Line Format:
                //{"high": "441.00", "last": "421.86", "timestamp": "1411606877", "bid": "421.96", "vwap": "428.58", "volume": "14120.40683975", "low": "418.83", "ask": "421.99"}
                try
                {
                    AhiGoalLineParser dp = new AhiGoalLineParser();
                    instruction = dp.ParceRowIntoObject(line);
                }
                catch { /* Do nothing, possible error in json decoding */ }
                return instruction;
            }

            //Example Line Format:
            //date,next_date,next_close,symbol,close,kno,w,h,e,n,rd,pct,cr,bsval1,bsval2,bsval3,bsval4
            //07/07/2015,12/31/1969,,CLF,3.53,415,1,3,1,4,R,83,5,B,U,Y,N
            try
            {
                AhiGoalLineParser dp = new AhiGoalLineParser();
                instruction = dp.ParceRowIntoObject(line);

            }
            catch { /* Do nothing, skip first title row */ }

            return instruction;
        }

        public void WriteLog(string logentry)
        {
            var assem = Assembly.GetExecutingAssembly();
            FileInfo info = new FileInfo(assem.Location);
            string directory = info.DirectoryName;
            if (!directory.EndsWith(@"\"))
            {
                directory += @"\";
            }
            string filepath = directory + @"urls.csv";
            //if (File.Exists(filepath))
            //    File.Delete(filepath);
            using (StreamWriter writer = new StreamWriter(filepath, true))
            {

                writer.WriteLine(logentry);

                writer.Flush();
            }


        }
    }

}
