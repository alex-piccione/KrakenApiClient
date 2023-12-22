using System;

using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;

namespace Example_of_use
{
    class Trader
    {
        IClient client;

        public Trader(IClient client) {
            this.client = client;
        }

        internal void CreateLimitOrder_Buy_Sell(CurrencyPair pair, decimal payAmount, decimal percentageToRemoveFromAsk, decimal percentageToAddForSell)
        {
            Console.WriteLine("\n# Buy & Sell #\n");

            Console.WriteLine($"Amount: {payAmount} {pair.Other}");

            var ticker = client.GetTicker(pair);
            Console.WriteLine(ticker);

            // BUY

            var askPrice = ticker.Ask; // market sell price
            var buyPrice = askPrice - (askPrice * percentageToRemoveFromAsk / 100); 
            Console.WriteLine($"Market ASK price: {askPrice} - Buy price: {buyPrice}");
            var orderQuantity = payAmount / buyPrice;

            var buyOrder = CreateOrderRequest.Limit(OrderSide.Buy, pair, orderQuantity, buyPrice);
            //Console.WriteLine(buyOrder);

            var buyOrderId = client.CreateLimitOrder(buyOrder);
            Console.WriteLine($"Buy Order: {buyOrderId}");


            // SELL
            var bidPrice = ticker.Bid;  // market buy price
            var sellPrice = buyPrice + (buyPrice * percentageToAddForSell / 100);
            Console.WriteLine($"Market BID price: {bidPrice} - Sell price: {sellPrice}");            

            var sellOrder = CreateOrderRequest.Limit(OrderSide.Sell, pair, orderQuantity, sellPrice);
            //Console.WriteLine(sellOrder.);

            var sellOrderId = client.CreateLimitOrder(sellOrder);
            Console.WriteLine($"Sell Order: {sellOrderId}");
        }

    }
}
