using QuantConnect.Logging;
using QuantConnect.Orders;
using QuantConnect.Util;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// Logs an order Event after it is filled.
    /// </summary>
    public class OrderReporter
    {
        private QCAlgorithm _algorithm;
        private ILogHandler _logHandler;
        private string columnHeader = @"Symbol,Quantity,Price,Direction,Order Date,Settlement Date, Amount,Commission,Net,Nothing,Description,Action Id,Order Id,RecordType,TaxLotNumber";

        public OrderReporter(QCAlgorithm algorithm)
        {
            _algorithm = algorithm;
            _logHandler = Composer.Instance.GetExportedValueByTypeName<ILogHandler>("TransactionFileLogHandler");
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
        public string ReportTransaction(OrderEvent orderEvent)
        {
            #region Print

            string transmsg = string.Format("Order {0} on not found", orderEvent.OrderId);
            Order order = _algorithm.Transactions.GetOrderById(orderEvent.OrderId);
            decimal orderValue = orderEvent.FillQuantity*orderEvent.FillPrice;

            //if (order != null)
            //{

            //var orderDateTime = order.Time;
            decimal orderFees = 0;
            orderFees = _algorithm.Securities[order.Symbol].TransactionModel.GetOrderFee(_algorithm.Securities[order.Symbol], order);
            int actionid = orderEvent.Direction.ToString() == "Buy" ? 1 : 13;
            transmsg = string.Format(
                "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}",
                orderEvent.Symbol,
                orderEvent.FillQuantity,
                orderEvent.FillPrice,
                orderEvent.Direction.ToString(),
                order.Time,
                order.Time.AddDays(4),
                orderValue,
                orderFees,
                orderValue + orderFees,
                "",
                orderEvent.Direction + " share of " + orderEvent.Symbol + "at $" + orderEvent.FillPrice.ToString(),
                actionid,
                order.Id,
                "Trade",
                "taxlot",
                ""
                );
            //  }
            _logHandler.Debug(transmsg);
            return transmsg;

            #endregion
        }
    }
}