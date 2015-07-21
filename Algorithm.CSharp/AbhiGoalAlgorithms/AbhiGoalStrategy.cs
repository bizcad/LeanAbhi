using System;
using System.Linq;
using QuantConnect.Data.Market;
using QuantConnect.Orders;

namespace QuantConnect.Algorithm
{
    /// <summary>
    /// A user defined strategy to be executed on a TradeBar
    /// </summary>
    public class AbhiGoalStrategy
    {
        private readonly AbhiGoalAlgorithm _algorithm;

        /// <summary>
        /// A strategy to be executed by a custom algorithm
        /// </summary>
        /// <param name="algorithm">the custom algorithm object.  Used for accessing it's Portfolio</param>
        public AbhiGoalStrategy(AbhiGoalAlgorithm algorithm)
        {
            _algorithm = algorithm;
        }

        /// <summary>
        /// Executes this custom strategy
        /// </summary>
        /// <param name="data">AbhiGoalCustomData - the custom data</param>
        /// <param name="transactionSize">decimal - the standard transaction size as defined in the algorithm</param>
        /// <param name="status">string - a status report on the execution for tracing</param>
        /// <param name="quantity">int - the number of shares purchased/sold</param>
        /// <param name="t">TradeBar - the current trade bar when the OnData was fired</param>
        /// <returns>int - orderID the id of the order if it is available. 
        /// 0 = no order placed 
        /// -1 = order not filled, such as a limit order that did not fill right away</returns>
        public int ExecuteStrategy(AbhiGoalCustomData data, TradeBar t, decimal transactionSize, out string status, out int quantity)
        {
            var s = data.symbol;
            status = "";
            int orderId = 0;
            quantity = 0;
            OrderTicket orderTicket = null;
            try
            {
                switch (data.kno)
                {
                    case 41:
                        quantity = Convert.ToInt32(CalculateBuyQuantity(data.pct, data.next_close, transactionSize));
                        orderTicket = _algorithm.Buy(data.symbol, quantity);
                        
                        status = "Buy";
                        if (orderId == -1)
                            status = "No trade history";
                        break;

                    case 31:
                        if (_algorithm.Portfolio[data.symbol].HoldStock)
                        {
                            quantity = Convert.ToInt32(CalculateSellQuantity(data.pct, _algorithm.Portfolio[s].Quantity));
                            orderTicket = _algorithm.Sell(data.symbol, quantity);
                            status = "Sell";
                            if (orderId == -1)
                                status = "No trade history";
                        }
                        else
                        {
                            status = "Not in portfolio";
                        }
                        break;

                    default:
                        throw new Exception("bad kno");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return orderId;
        }

        /// <summary>
        /// Calculates the quantity to Buy
        /// </summary>
        /// <param name="pct">the pct column from the external indicators</param>
        /// <param name="price">the price used to calculate the number of shares</param>
        /// <param name="transactionSize">the standard transaction size as defined in the algorithm</param>
        /// <returns>decimal - the number of shares to sell</returns>
        /// <remarks>Note that this value is calculated as a decimal.  When the order is placed,
        /// QuantConnect will truncate any partial shares and make it and int.</remarks>
        private decimal CalculateBuyQuantity(decimal pct, decimal price, decimal transactionSize)
        {
            decimal buyqty = 0;
            var p = (int)pct;
            switch (p)
            {
                case 83:
                    buyqty = transactionSize * .25m / price;
                    break;

                case 92:
                    buyqty = transactionSize * .75m / price;
                    break;
                case 100:
                    buyqty = transactionSize / price;
                    break;
            }
            return buyqty;
        }
        /// <summary>
        /// Calculates the quantity to sell
        /// </summary>
        /// <param name="pct">the pct column from the external indicators</param>
        /// <param name="sharesHeld">the number of shares of the symbol in the portfolio</param>
        /// <returns>decimal - the number of shares to sell</returns>
        /// <remarks>Note that this value is calculated as a decimal.  When the order is placed,
        /// QuantConnect will truncate any partial shares and make it and int.</remarks>
        private decimal CalculateSellQuantity(decimal pct, int sharesHeld)
        {
            decimal sellqty = 0;
            var p = (int)pct;
            switch (p)
            {
                case 83:
                    sellqty = sharesHeld * .25m;
                    break;
                case 92:
                    sellqty = sharesHeld * .75m;
                    break;
                case 100:
                    sellqty = sharesHeld;
                    break;
            }
            return sellqty;
        }

    }
}