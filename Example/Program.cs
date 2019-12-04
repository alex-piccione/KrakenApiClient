using System;

using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;

namespace Example
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hello World!");

            //IClient client = new Client(); // only public methods
            string publicKey = ""; // use your key
            string privateKey = ""; // use your key

            IClient client = new Client(publicKey, privateKey);  // private methods      


            // get ticker
            GetTicker(client);

            // get balance
            //GetBalance(client);

            // buy a precise amount of XRP paying in EUR
            Buy_250_XRP_with_EUR(client);

            // buy XRP with 50 EUR
            //BuyXRP_with_50_EUR(client);
        }

        private static void Buy_250_XRP_with_EUR(IClient client)
        {
            var buyAmount = 250; // XRP to buy

            var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderAction.Buy, buyAmount);

            if (orderResponse.IsSuccess)
            {
                Console.WriteLine($"Order: {orderResponse.OrderIds}");
            }
            else
            {
                Console.WriteLine($"Order fail: {orderResponse.Error}");
            }
        }

        private static void BuyXRP_with_50_EUR(IClient client)
        {
            var ticker = client.GetTicker(Currency.XRP, Currency.EUR);

            if (!ticker.IsSuccess)
            {
                Console.WriteLine("Error: " + ticker.Error);
                return;
            }

            // Kraken API does not offer a way to pay a precise amount of "base currency" (EUR) 
            // so we need to calculate the amount of "quote currency" (EUR) based on the current best market ask price

            var askPrice = ticker.Ticker.Value.Ask;

            decimal payAmount = 50; // EUR
            decimal buyAmount = payAmount / askPrice;
            var orderResponse = client.CreateMarketOrder(CurrencyPair.XRP_EUR, OrderAction.Buy, buyAmount);

            if (orderResponse.IsSuccess)
            {
                Console.WriteLine($"Order: {orderResponse.OrderIds}");
            }
            else {
                Console.WriteLine($"Order fail: {orderResponse.Error}");
            }
        }

        private static void GetTicker(IClient client)
        {
            var response = client.GetTicker(new Currency("xrp"), new Currency("eur"));
            if (response.IsSuccess)
                Console.WriteLine($"Ticker: {response.Ticker.Value}");
            else
                Console.WriteLine($"Error: {response.Error}");
        }

        private static void GetBalance(IClient client)
        {
            var xrp = new Currency("xrp");
            var eur = new Currency("eur");

            var response = client.GetBalance(new Currency[] { xrp, eur});
            if (response.IsSuccess) 
                Console.WriteLine($"Balance. \n\tXRP: {response.CurrenciesBalance[xrp]}  \n\tEUR: {response.CurrenciesBalance[eur]} ");
            else
                Console.WriteLine($"Error: {response.Error}");
        }
    }
}
