using System;
using QuantConnect.Data.Market;
using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Util;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// Logs an order Event after it is filled.
    /// </summary>
    public class CustomDataBarReporter
    {
        private QCAlgorithm _algorithm;
        private ILogHandler _logHandler;
        private string columnHeader = @"date,next_date,next_close,symbol,close,kno,w,h,e,n,rd,pct,cr,bsval1,bsval2,bsval3,bsval4, status,quantity,,Hour, Minute, Second, Millisecond";

        /// <summary>
        /// Parameter constructor to inject the algorithm to report from
        /// </summary>
        /// <param name="algorithm">the algorithm running</param>
        public CustomDataBarReporter(QCAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _logHandler = Composer.Instance.GetExportedValueByTypeName<ILogHandler>("DailyFileLogHandler");
        }
        public void ReportHeading(string heading)
        {
            _logHandler.Debug(heading);
            _logHandler.Debug(columnHeader);
        }
        /// <summary>
        /// Logs the OrderEvent Transaction
        /// </summary>
        /// <param name="orderEvent">the OrderEvent being logged</param>
        /// <param name="abhiGoalCustomData">the custom data bar</param>
        public void ReportDailyBar(AbhiGoalCustomData abhiGoalCustomData, TradeBar tradeBar, string status, decimal quantity)
        {
            #region "Print"
            string msg = (string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24}",
                abhiGoalCustomData.date,
                abhiGoalCustomData.next_date,
                abhiGoalCustomData.next_close,
                abhiGoalCustomData.symbol,
                abhiGoalCustomData.close,
                abhiGoalCustomData.kno,
                abhiGoalCustomData.w,
                abhiGoalCustomData.h,
                abhiGoalCustomData.e,
                abhiGoalCustomData.n,
                abhiGoalCustomData.rd,
                abhiGoalCustomData.pct,
                abhiGoalCustomData.cr,
                abhiGoalCustomData.bsval1,
                abhiGoalCustomData.bsval2,
                abhiGoalCustomData.bsval3,
                abhiGoalCustomData.bsval4,
                status,
                quantity,
                "",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                ""
                ));

            _logHandler.Debug(msg);

            #endregion
        }
    }
}