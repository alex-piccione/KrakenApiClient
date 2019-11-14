namespace Alex75.KrakenApiClient

open System
open System.Linq
open Alex75.Cryptocurrencies
open Flurl.Http
open utils
open System.Collections.Generic
open System.Threading
open Flurl.Http
open System.Net
open System.Text


//[<Interface>]
type IClient =
    abstract member GetTicker: main:Currency * other:Currency -> TickerResponse
    abstract member GetBalance: currencies:Currency[] -> BalanceResponse


type public Client (public_key:string, secret_key:string) =
    
    let ensure_keys () = if String.IsNullOrWhiteSpace(public_key) || String.IsNullOrWhiteSpace(secret_key) then failwith "This method require public and secret keys"

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

            try
                (* this works 
                let request =  WebRequest.Create(url) :?> HttpWebRequest
                
                request.ContentType <- "application/x-www-form-urlencoded"
                request.Method <- "POST"
                request.Headers.Add("API-Key", public_key)
                request.Headers.Add("API-Sign", signature)       

                let stream = new System.IO.StreamWriter( request.GetRequestStream() )
                stream.Write(props)
                stream.Close() // !!! General unknown error wothout this !!!

                let response = request.GetResponse() :?> HttpWebResponse
                let statusCode = response.StatusCode
                let reader = new System.IO.StreamReader( response.GetResponseStream())
                let json = reader.ReadToEnd()
                //reader.Close
                *)
                let mutable nonce:int64 = DateTime.Now.Ticks
                let props = null (* &key=value *)                
                let content = f"nonce=%i%s" nonce props
                let responseMessage = (url.WithApi "/0/private/Balance" nonce props public_key secret_key).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                let balance = parser.parse_balance(json) 

                // to refactor
                let balance_list = Dictionary<Currency, decimal>()
                for v in balance do balance_list.Add(Currency(v.Key), v.Value)    

                BalanceResponse(true, null, balance_list)

            with e -> BalanceResponse(false, e.Message, null)