namespace Alex75.KrakenApiClient

open System
open Alex75.Cryptocurrencies
open Flurl.Http
open utils
open System.Collections.Generic
open System.Threading
open Flurl.Http


//[<Interface>]
type IClient =
    abstract member GetTicker: main:Currency * other:Currency -> TickerResponse
    abstract member GetBalance: currencies:Currency[] -> BalanceResponse


type public Client (public_key:string, secret_key:string) =
    
    let ensure_keys () = if String.IsNullOrWhiteSpace(public_key) || String.IsNullOrWhiteSpace(secret_key) then failwith "This method require public and secret ekys"

    new () = Client(null, null)
       

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

        
        member __.GetBalance(currencies:Currency[]) =            
            ensure_keys()
            let url = "https://api.kraken.com/0/private/Balance"

            let get_balance (currency:Currency) = 
                let body = {|aclass=currency.UpperCase|}
                
                let responseMessage = (url.WithApi "/0/private/Balance"  public_key secret_key).PostUrlEncodedAsync(body).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                let balance = parser.parse_balance(json)
                balance.Amount

            try

                let balance_list = Dictionary<Currency, decimal>()      
                let list = System.Collections.Concurrent.ConcurrentDictionary<Currency, decimal>()
                
                System.Threading.Tasks.Parallel.ForEach(currencies, fun c -> list.TryAdd(c, get_balance(c)) |> ignore) |> ignore

                BalanceResponse(true, null, balance_list)

            with e -> BalanceResponse(false, e.Message, null)