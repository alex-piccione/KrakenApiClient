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

            //IClient client = new Client(); // public methods
            string publicKey = "";
            string privateKey = "";
            IClient client = new Client(privateKey, publicKey);  // private methods      

            // get ticker
            GetTicker(client);

            // get balance
            GetBalance(client);

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
