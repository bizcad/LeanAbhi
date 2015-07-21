using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Securities;
using QuantConnect.Util;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// A custom algorithm for Abhi Goal to apply a trading plan to a list of custom indicators
    /// </summary>
    public class AbhiGoalAlgorithm : QCAlgorithm
    {
        private DateTime _startDate = new DateTime(2010, 1, 5);
        private DateTime _endDate = new DateTime(2015, 7, 1);
        private decimal _portfolioAmount = 10000000;
        private decimal _transactionSize = 100000;

        // Symbols
        List<string> symbols;
        private IQueryable<string> _queryableSymbols;

        // The list of indicators imported from the url Abhi sent.
        private IQueryable<AbhiGoalCustomData> externalIndicators;

        // Custom Logging
        private bool IsRunningLocal = true;

        private ILogHandler mylog = Composer.Instance.GetExportedValueByTypeName<ILogHandler>("CustomFileLogHandler");
//        private ILogHandler dailylog = Composer.Instance.GetExportedValueByTypeName<ILogHandler>("DailyFileLogHandler");
//        private ILogHandler transactionlog = Composer.Instance.GetExportedValueByTypeName<ILogHandler>("TransactionFileLogHandler");
        private string ondataheader = @"Date,Next Date,Next Close,Symbol,Close,kno,w,h,e,n,rd,pct,cr,bsval1,bsval2,bsval3,bsval4";
//        private string dailyheader = @"Date of Bar, date,next_dDate,next_close,symbol,Close,kno,pct, Status, Quantity, Open, High, Low, Close,,Hour, Minute, Second, Millisecond";
//        private string transactionheader = @"Symbol,Quantity,Price,Direction,Order Date,Settlement Date, Amount,Commission,Net,Nothing,Description,Action Id,Order Id,RecordType,TaxLotNumber";
        private int orderId;
        private decimal _lasttotalFees;

        private OrderReporter orderReporter;
        private CustomDataBarReporter _customDataBarReporter;
        //private TradeBarReporter _tradeBarReporter;

        //private ConcurrentQueue<AbhiGoalCustomData> _tradeList;
        private readonly AbhiGoalStrategy _abhiGoalStrategy;

        public AbhiGoalAlgorithm()
        {
            _abhiGoalStrategy = new AbhiGoalStrategy(this);
        }

        /// <summary>
        /// Initialise the data and resolution required, as well as the cash and start-end dates for your algorithm. All algorithms must initialized.
        /// </summary>
        /// <seealso cref="QCAlgorithm.SetStartDate(System.DateTime)"/>
        /// <seealso cref="QCAlgorithm.SetEndDate(System.DateTime)"/>
        /// <seealso cref="QCAlgorithm.SetCash(decimal)"/>
        public override void Initialize()
        {
            if (IsRunningLocal)
            {

                // Init custom logs
                var algoname = this.GetType().Name;
                mylog.Debug(algoname);
                mylog.Debug(ondataheader);      

                _customDataBarReporter = new CustomDataBarReporter((QCAlgorithm)this);
                _customDataBarReporter.ReportHeading(algoname);
                orderReporter = new OrderReporter((QCAlgorithm)this);
                orderReporter.ReportHeading(algoname);
                

                //    // use a data file for testing
                //FileDataImport fi = new FileDataImport();
                //externalIndicators = fi.GetData().AsQueryable();
           
                // use the web address for production
                WebDataImport di = new WebDataImport();
                externalIndicators = di.GetData().AsQueryable();
            }

            symbols = new List<string>();
            foreach (var indicator in externalIndicators)
            {
                if (!symbols.Contains(indicator.symbol))
                    symbols.Add(indicator.symbol);
            }
            // Sort the symbols and make it queryable
            _queryableSymbols = symbols.OrderBy(s => s).AsQueryable();


            //_tradeList = new ConcurrentQueue<AbhiGoalCustomData>();
            // Add a security for each symbol in the externalIndicators list
            foreach (string symbol in _queryableSymbols)
            {
                AddSecurity(SecurityType.Equity, symbol, Resolution.Daily);
            }

            // export a list of the indicators for comparison with the file.csv Abhi sent me
            if (IsRunningLocal)
                ReportExternalIndicators();

            //Initialize dates and cash
            SetStartDate(_startDate);
            SetEndDate(_endDate);
            SetCash(_portfolioAmount);

            AddData<AbhiGoalCustomData>("ABHIGOAL", Resolution.Daily);

        }


        ///// <summary>
        ///// OnData event is the primary entry point for your algorithm. Each new data point will be pumped in here.
        ///// </summary>
        ///// <param name="data">TradeBars IDictionary object with your stock data</param>
        //public void OnData(TradeBars data)
        //{
        //}

        /// <summary>
        /// Handle the OnData event when a custom data item is read
        /// </summary>
        /// <param name="data">Custom data object with execution instructions</param>
        public void OnData(AbhiGoalCustomData data)
        {
            //_tradeList.Enqueue(data);
            
            string status;
            int quantity;
            TradeBar tradeBar = new TradeBar();
            orderId = _abhiGoalStrategy.ExecuteStrategy(data, tradeBar, _transactionSize, out status, out quantity);
            _customDataBarReporter.ReportDailyBar(data, tradeBar, status, quantity);
        }



        /// <summary>
        /// Handle the OnOrder Event by logging the order
        /// </summary>
        /// <param name="orderEvent">the orderEvent shows information about the order</param>
        public override void OnOrderEvent(OrderEvent orderEvent)
        {
            base.OnOrderEvent(orderEvent);
            if (orderEvent.Status == OrderStatus.Filled)
            {
                orderReporter.ReportTransaction(orderEvent);
            }
        }

        #region Logging
        private void ReportExternalIndicators()
        {
            #region "Reporting"
            foreach (var externalIndicator in externalIndicators)
            {
                string msg = string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}",
                    externalIndicator.Time,
                    externalIndicator.next_date,
                    externalIndicator.next_close,
                    externalIndicator.symbol,
                    externalIndicator.close,
                    externalIndicator.kno,
                    externalIndicator.w,
                    externalIndicator.h,
                    externalIndicator.e,
                    externalIndicator.n,
                    externalIndicator.rd,
                    externalIndicator.pct,
                    externalIndicator.cr,
                    externalIndicator.bsval1,
                    externalIndicator.bsval2,
                    externalIndicator.bsval3,
                    externalIndicator.bsval4,
                    "",
                    "",
                    "",
                    ""
                    );
                mylog.Debug(msg);
            }
            #endregion
        }


        #endregion
    }
    /// <summary>
    /// Locates the directory where the application is executing.  This is used to locate the
    /// External Indicators file when testing.  It is not used on QuantConnect.
    /// </summary>
    public static class AssemblyLocator
    {
        /// <summary>
        /// Returns the folder where the application is executing including the following backslash
        /// </summary>
        /// <returns>string - the name of the directory where the application is executing</returns>
        public static string ExecutingDirectory()
        {
            var assem = Assembly.GetExecutingAssembly();
            FileInfo info = new FileInfo(assem.Location);
            string directory = info.Directory.FullName;
            if (!directory.EndsWith(@"\"))
            {
                directory += @"\";
            }
            return directory;
        }
    }
}
