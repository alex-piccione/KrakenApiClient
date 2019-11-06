namespace Alex75.KrakenApiClient

open System
open Alex75.Cryptocurrencies
open Flurl.Http
open utils


//[<Interface>]
type IClient =
    abstract member GetTicker: main:Currency * other:Currency -> TickerResponse


// URL: https://api.kraken.com/0/public/AssetPairs

type public Client () =
  
    
    interface IClient with
        

        member __.GetTicker (main, other) =            
           
            let kraken_pair = utils.get_kraken_pair main other

            let url = f"https://api.kraken.com/0/public/Ticker?pair=%s" kraken_pair.AAABBB
                       
            try
                let responseMessage = url.GetAsync().Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                let ticker = parser.parseTicker(CurrencyPair(main, other), json)
                TickerResponse(true, null, Some ticker)

            with e -> TickerResponse(false, e.Message, None)