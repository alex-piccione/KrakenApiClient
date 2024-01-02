using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;

namespace Example
{
    internal class Program
    {
        private static void Main()
        {
            Console.WriteLine("Kraken API Client examples\n");

            var configuration = new ConfigurationBuilder()
                .AddUserSecrets("Alex75.KrakenApiClient-08ccac50-5aef-4bd5-b18a-707588558352")
                .Build();

            string publicKey = configuration["public key"];
            string privateKey = configuration["private key"];

            Guard.IsNotNullOrEmpty(publicKey, $@"configuration secret ""{nameof(publicKey)}""");
            Guard.IsNotNullOrEmpty(privateKey, $@"configuration secret ""{nameof(privateKey)}""");

            IClient client = new Client(publicKey, privateKey);

            // get ticker
            GetTicker(client);

            // get balance
            GetBalance(client);

            // see orders
            ListOpenOrders(client);
            ListClosedOrders(client);

            // buy a precise amount of XRP paying in EUR
            //Buy_250_XRP_with_EUR(client);

            //CreateLimitOrder(client);

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
            try
            {
                var ticker = client.GetTicker(new CurrencyPair("xrp", "eur"));
                Console.WriteLine($"Ticker: {ticker}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error: {exc}");
            }
        }

        private static void GetBalance(IClient client)
        {
            PrintSection("Get Balance");

            try
            {
                var balance = client.GetBalance();

                foreach (var item in balance)
                    Console.WriteLine($"Curreny: {item.Currency}, Amount: {item.Total}, Available: {item.Free}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error: {exc}");
            }
        }

        private static void PrintSection(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(text);
            Console.ForegroundColor = color;
        }

        private static void ListOpenOrders(IClient client)
        {
            try
            {
                var orders = client.ListOpenOrders();
                foreach (var order in orders)
                    Console.WriteLine($"Order: {order}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"ListOpenOrders failed: {exc}");
            }
        }

        private static void ListClosedOrders(IClient client)
        {
            try
            {
                var orders = client.ListClosedOrders();
                foreach (var order in orders)
                    Console.WriteLine($"Order: {order}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"ListClosedOrders failed: {exc}");
            }
        }

        private static void Buy_250_XRP_with_EUR(IClient client)
        {
            var buyAmount = 250; // XRP to buy

            //var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderSide.Buy, buyAmount);

            //if (orderResponse.IsSuccess)
            //{
            //    Console.WriteLine($"Order: {orderResponse.OrderIds}");
            //}
            //else
            //{
            //    Console.WriteLine($"Order failed: {orderResponse.Error}");
            //}

            var order = client.CreateMarketOrder(CreateOrderRequest.Market(OrderSide.Buy, CurrencyPair.XRP_EUR, buyAmount));

            Console.WriteLine($"Order: {order.Reference}");
        }

        private static void BuyXRP_with_50_EUR(IClient client)
        {
            var ticker = client.GetTicker(new CurrencyPair(Currency.XRP, Currency.EUR));

            // Kraken API does not offer a way to pay a precise amount of "base currency" (EUR)
            // so we need to calculate the amount of "quote currency" (EUR) based on the current best market ask price

            var askPrice = ticker.Ask;

            decimal payAmount = 50; // EUR
            decimal buyAmount = payAmount / askPrice;
            var order = client.CreateMarketOrder(CreateOrderRequest.Market(OrderSide.Buy, CurrencyPair.XRP_EUR, buyAmount));

            Console.WriteLine($"Order: {order.Reference}");
        }

        private static void CreateLimitOrder(IClient client)
        {
            var pair = CurrencyPair.XRP_EUR;
            var payAmount = 500; //500 EUR

            var marketPrice = client.GetTicker(pair).Bid;
            var price = marketPrice - (marketPrice * .04m); // -4%
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