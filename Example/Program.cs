using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;
using Example_of_use;
using System.Data;
using System.Runtime.InteropServices;

namespace Example
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Kraken API Client examples\n");

            var configuration = new ConfigurationBuilder()                
                //.AddUserSecrets("Kraken.fe116236-f58b-49a1-ae3b-8761bdbeb024")
                .AddUserSecrets("Alex75.KrakenApiClient-08ccac50-5aef-4bd5-b18a-707588558352")
                .Build();

            string publicKey = configuration["public key"];  
            string privateKey = configuration["secret key"];                  
            IClient client = new Client(publicKey, privateKey);

            var trader = new Trader(client);
            
            // get ticker
            GetTicker(client);

            // get balance
            GetBalance(client);

            //trader.CreateLimitOrder_Buy_Sell(CurrencyPair.XRP_GBP, 400, 0.5m, 2.5m);

            //Buy_withAmount(client, CurrencyPair.XRP_GBP, 600, 0.05m);

            // see orders
            ListOpenOrders(client);
            ListClosedOrders(client);



            // buy a precise amount of XRP paying in EUR
            //Buy_250_XRP_with_EUR(client);

            

            // buy XRP with 50 EUR
            //BuyXRP_with_50_EUR(client);

            // get open orders
            //ListOpenOrders(client);

            // withdraw 50 XRP to a registered wallet
            //WithdrawFunds(client);

            Console.ReadKey();
        }


        private static void GetTicker(IClient client)
        {
            Console.WriteLine("\n# Tickers #\n");

            var pairs = new CurrencyPair[] { 
                CurrencyPair.XRP_EUR, 
                CurrencyPair.XRP_GBP,
                CurrencyPair.ADA_EUR,
            };

            Console.WriteLine(" Pair       | Bid        | Ask        ");
            Console.WriteLine(" -----------+------------+------------");

            try
            {
                foreach (var pair in pairs)
                {
                    var ticker = client.GetTicker(pair);
                    Console.WriteLine($" {ticker.Pair.Slashed,-10} | {ticker.Bid,-10} | {ticker.Ask,-10} ");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"GetTicker Error: {exc}");
            }
        }

        private static void GetBalance(IClient client)
        {
            Console.WriteLine($"\n# Balance #\n");

            try
            {                
                var balance = client.GetBalance();
                Console.WriteLine(" Currency | Owned           | Available       ");
                Console.WriteLine(" ---------+-----------------+---------------- ");

                foreach (var item in balance)
                    Console.WriteLine($" {item.Currency,-8} | {item.OwnedAmount,+15} | {item.AvailableAmount,+15} ");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"GetBalance Error: {exc}");
            }
        }

        private static void ListOpenOrders(IClient client)
        {
            Console.WriteLine($"\n# Open Orders #\n");

            try
            {
                var orders = client.ListOpenOrders();
                
                Console.WriteLine(" Date             | Pair       | Type    | Side | Amount          | Note                       ");
                Console.WriteLine(" -----------------+------------+---------+------+-----------------+--------------------------- ");
                foreach (var order in orders)
                    Console.WriteLine($" {order.OpenTime:g} | {order.Pair.Dashed,-10} | {order.Type,-7} | {order.Side, -4} | {order.BuyOrSellQuantity,15} | {order.Id} Price:{order.LimitPrice} ");
            }
            catch(Exception exc)
            {
                Console.WriteLine($"ListOpenOrders Error: {exc}");
            }
        }

        private static void ListClosedOrders(IClient client)
        {
            Console.WriteLine($"\n# Closed Orders #\n");

            try
            {
                var orders = client.ListClosedOrders();

                Console.WriteLine(" Opened           | Closed           | Pair       | Type    | Side | Amount          | Note                ");
                Console.WriteLine(" ---------------- | ---------------- | ---------- | ------- | ---- | --------------- | ------------------- ");

                foreach (var order in orders.Take(10))
                    Console.WriteLine($" {order.OpenTime:g} | {order.CloseTime:g} | {order.Pair.Dashed,+10} | {order.Type,-7} | {order.Side,-4} | {order.BuyOrSellQuantity,15} | {order.Id} Price:{order.Price} ");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"ListClosedOrders failed: {exc}");
            }
        }


        private static void Buy(IClient client, CurrencyPair pair, decimal quantity)
        {
            //var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderSide.Buy, buyAmount);

            //if (orderResponse.IsSuccess)
            //{
            //    Console.WriteLine($"Order: {orderResponse.OrderIds}");
            //}
            //else
            //{
            //    Console.WriteLine($"Order failed: {orderResponse.Error}");
            //}

            var orderRequest = CreateOrderRequest.Market(OrderSide.Buy, pair, quantity);
            var order = client.CreateMarketOrder(orderRequest);

            Console.WriteLine($"Order Ref.: {order.Reference} Price: {order.Price}");      
        }

        private static void Buy_withAmount(IClient client, CurrencyPair pair, decimal payAmount, decimal addPercentage)
        {
            var ticker = client.GetTicker(pair);
            var availableAmount = client.GetBalance().GetCurrency(pair.Quote).AvailableAmount;

            if (payAmount > availableAmount)
                throw new Exception($"Available amount ({availableAmount}) is lower than order {payAmount}");

            // Kraken API does not offer a way to pay a precise amount of "base currency" (EUR) 
            // so we need to calculate the amount of "quote currency" (EUR) based on the current best market ask price

            var marketPrice = ticker.Bid;
            var orderPrice = marketPrice - (marketPrice * addPercentage / 100m);
            decimal buyAmount = payAmount / orderPrice;
            var orderRequest = CreateOrderRequest.Limit(OrderSide.Buy, pair, buyAmount, orderPrice);

            //var order = client.CreateMarketOrder(CreateOrderRequest.Market(OrderSide.Buy, pair, buyAmount));
            var order = client.CreateLimitOrder(orderRequest);

            Console.WriteLine($"Limit Order Ref.: {order}  Price: {orderPrice}");
        }


        private static void CreateLimitOrder(IClient client)
        {
            var pair = CurrencyPair.XRP_EUR;
            var payAmount = 500; //500 EUR

            var marketPrice = client.GetTicker(pair).Ask;
            var price = marketPrice - (marketPrice * .01m); // -4%
            var xrpQuantity = payAmount / price;

            //xrpQuantity = xrpQuantity / 10;

            var orderRequest = CreateOrderRequest.Limit(OrderSide.Buy, pair, xrpQuantity, price);

            var orderId = client.CreateLimitOrder(orderRequest);
            Console.WriteLine($"Order: {orderId}");
        }


        private static void WithdrawFunds(IClient client)
        {
            var response = client.Withdraw(Currency.XRP, 50, "Binance");

            if (response.IsSuccess)
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} competed. Operation ID: {response.OperationId}");
            }
            else
            {
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} failed: {response.Error}");
            }
        }

    }
}
