namespace Alex75.KrakenApiClient

open System
open System.Linq
open System.Collections.Generic

open Flurl.Http
open Alex75.Cryptocurrencies
open utils

//[<Enum>]
type OrderAction = | Buy | Sell


type IClient =
    abstract member GetTicker: main:Currency * other:Currency -> TickerResponse
    abstract member GetBalance: currencies:Currency[] -> BalanceResponse
    abstract member CreateMarketOrder: pair:CurrencyPair * action:OrderAction * buyAmount:decimal -> CreateMarketOrderResponse


type public Client (public_key:string, secret_key:string) =
    
    let ensure_keys () = if String.IsNullOrWhiteSpace(public_key) || String.IsNullOrWhiteSpace(secret_key) then failwith "This method require public and secret keys"
    let base_url = "https://api.kraken.com/0"

    let create_props (values:IDictionary<string, string>) = 

        // to refactor
        let props = System.Text.StringBuilder()
        for kv in values do
            props.AppendFormat("&{0}={1}", kv.Key, kv.Value) |> ignore 
        props.ToString()
        

    new () = Client(null, null)
       
    

    interface IClient with        

        member __.GetTicker (main, other) =            
           
            let kraken_pair = utils.get_kraken_pair main other

            let url = f"%s/public/Ticker?pair=%s" base_url kraken_pair.AAABBB
                       
            try
                let responseMessage = url.GetAsync().Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                let ticker = parser.parseTicker(CurrencyPair(main, other), json)
                TickerResponse(true, null, Some ticker)

            with e -> TickerResponse(false, e.Message, None)

        
        member __.GetBalance(currencies:Currency[]) =            
            ensure_keys()
            let url = f"%s/private/Balance" base_url

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

                //let balance_list_2 = List.collect ( fun (k,v) ->  (Currency(k), v) ) <| balance

                BalanceResponse(true, null, balance_list)

            with e -> BalanceResponse(false, e.Message, null)


        member __.CreateMarketOrder (pair:CurrencyPair, action:OrderAction, buyAmount:decimal) =
            ensure_keys()

            let url = f"%s/private/AddOrder" base_url            
            let kraken_pair = (utils.get_kraken_pair pair.Main pair.Other).AAABBB

            try

                let nonce = DateTime.Now.Ticks
                let values = dict ([
                    ("pair", kraken_pair)  
                    ("type", action.ToString().ToLower())
                    ("ordertype", "market")
                    //("price")
                    ("volume", buyAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)) // ???? {"error":["EGeneral:Invalid arguments:volume"]}
                    //("leverage")
                    //("oflags", "viqc") // volume in quote currency   // no more available !
                    //("validate", "true") // ANY value (also validate=true) will be a simulation, order id not returned
                ])
                
                let props:string = create_props values

                // string reqs = string.Format("&pair={0}&type={1}&ordertype={2}&volume={3}&leverage={4}", pair, type, ordertype, volume,leverage);
                                
                let content = f"nonce=%i%s" nonce props
                let responseMessage = (url.WithApi "/0/private/AddOrder" nonce props public_key secret_key).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

                let struct (orderIds, amount) = parser.parse_order(json)
                
                CreateMarketOrderResponse(true, null, orderIds, amount)                

            with e -> CreateMarketOrderResponse.Fail e.Message