using System;
using System.Reflection;
using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;
using Microsoft.Extensions.Configuration;

namespace Example
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Kraken API Client examples\n");

            //IClient client = new Client(); // only public methods (no keys required)

            var configuration = new ConfigurationBuilder()                
                .AddUserSecrets("Kraken.fe116236-f58b-49a1-ae3b-8761bdbeb024")
                .Build();


            string publicKey = configuration["public key"];  // use your key
            string privateKey = configuration["private key"]; // use your key                  
            IClient client = new Client(publicKey, privateKey);  // private methods      

            
            // get ticker
            GetTicker(client);

            // get balance
            GetBalance(client);

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
            try
            {
                var balance = client.GetBalance();

                foreach (var item in balance)
                    Console.WriteLine($"Curreny: {item.Currency}, Amount: {item.OwnedAmount}, Available: {item.AvailableAmount}");
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Error: {exc}");
            }
        }

        private static void ListOpenOrders(IClient client)
        {
            try
            {
                var orders = client.ListOpenOrders();
                foreach (var order in orders)
                    Console.WriteLine($"Order: {order}");
            }
            catch(Exception exc)
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

            var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderSide.Buy, buyAmount);

            if (orderResponse.IsSuccess)
            {
                Console.WriteLine($"Order: {orderResponse.OrderIds}");
            }
            else
            {
                Console.WriteLine($"Order failed: {orderResponse.Error}");
            }
        }

        private static void BuyXRP_with_50_EUR(IClient client)
        {
            var ticker = client.GetTicker(new CurrencyPair(Currency.XRP, Currency.EUR));

            // Kraken API does not offer a way to pay a precise amount of "base currency" (EUR) 
            // so we need to calculate the amount of "quote currency" (EUR) based on the current best market ask price

            var askPrice = ticker.Ask;

            decimal payAmount = 50; // EUR
            decimal buyAmount = payAmount / askPrice;
            var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderSide.Buy, buyAmount);

            if (orderResponse.IsSuccess)
            {
                Console.WriteLine($"Order: {orderResponse.OrderIds}");
            }
            else {
                Console.WriteLine($"Order failed: {orderResponse.Error}");
            }
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
