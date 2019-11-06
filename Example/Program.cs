using System;

using Alex75.Cryptocurrencies;
using Alex75.KrakenApiClient;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IClient client = new Client();

            // get ticker
            GetTicker(client);

        }

        private static void GetTicker(IClient client)
        {
            var response = client.GetTicker(new Currency("xrp"), new Currency("eur"));
            if (response.IsSuccess)
                Console.WriteLine($"Ticker: {response.Ticker.Value}");
            else
                Console.WriteLine($"Error: {response.Error}");
        }
    }
}
