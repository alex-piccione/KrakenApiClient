namespace Alex75.KrakenApiClient

open System
open System.Linq
open System.Collections.Generic

open Flurl.Http
open Alex75.Cryptocurrencies
open utils



type public Client (public_key:string, secret_key:string) =  
        
    let base_url = "https://api.kraken.com/0"
    let ticker_cache_time = TimeSpan.FromSeconds 5.0
    let balance_cache_time = TimeSpan.FromSeconds 10.0

    let ensure_keys () = if String.IsNullOrWhiteSpace(public_key) || String.IsNullOrWhiteSpace(secret_key) then failwith "This method require public and secret keys"

    let create_props (values:IDictionary<string, string>) = 

        // to refactor
        let props = System.Text.StringBuilder()
        for kv in values do
            props.AppendFormat("&{0}={1}", kv.Key, kv.Value) |> ignore 
        props.ToString()           
    
    let get_ticker (main:Currency, other:Currency) =    
        
        let pair:CurrencyPair = CurrencyPair(main, other)
        let cached_ticker = cache.getTicker pair ticker_cache_time
        
        match cached_ticker.IsSome with 
        | true -> TickerResponse(true, null, Some(cached_ticker.Value))
        | _ -> 
        
             let kraken_pair = utils.get_kraken_pair main other

             let url = f"%s/public/Ticker?pair=%s" base_url kraken_pair.AAABBB
                    
             try
                 let responseMessage = url.GetAsync().Result
                 let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                 let ticker = parser.parseTicker(pair, json)
                 TickerResponse(true, null, Some ticker)

             with e -> TickerResponse(false, e.Message, None)


    new () = Client(null, null)


    interface IClient with        

        member __.GetTicker (main, other) = get_ticker(main, other)  
        member __.GetTicker (pair) = get_ticker(pair.Main, pair.Other)

        
        member __.GetBalance(currencies:Currency[]) =            
            ensure_keys()
            let url = f"%s/private/Balance" base_url                           

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

            try
                let cached_balance = cache.getBalance balance_cache_time
                let all_balances = 
                    match cached_balance.IsSome with
                    | true -> cached_balance.Value
                    | _ ->
                        let mutable nonce:int64 = DateTime.Now.Ticks
                        let props = null (* &key=value *)                
                        let content = f"nonce=%i%s" nonce props
                        let responseMessage = (url.WithApi "/0/private/Balance" nonce props public_key secret_key).PostUrlEncodedAsync(content).Result
                        let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                        let balance = parser.parse_balance(json) 
                        cache.setBalance balance balance_cache_time
                        balance

                let wanted_balances = Dictionary<Currency, decimal>()
                for item in all_balances do 
                    if currencies.Contains(item.Key) then wanted_balances.Add(item.Key, item.Value)    
   
                BalanceResponse(true, null, wanted_balances)

            with e -> BalanceResponse(false, e.Message, null)


        member __.CreateMarketOrder (pair:CurrencyPair, action:OrderSide, buyAmount:decimal) =
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


        member __.ListOpenOrders () =
            ensure_keys()

            let url = f"%s/private/OpenOrders" base_url   
            
            try
                let nonce = DateTime.Now.Ticks
                let props = null   
                // inputs
                // trades = whether or not to include trades in output (optional.  default = false)
                // userref = restrict results to given user reference id (optional)

                let content = f"nonce=%i%s" nonce props
                let responseMessage = (url.WithApi "/0/private/OpenOrders" nonce props public_key secret_key).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                let orders = parser.parse_open_orders(json)                 

                OpenOrdersResponse(true, null, orders)

            with e -> OpenOrdersResponse(false, e.Message, null)



        member __.Withdraw (currency:Currency, amount:decimal, walletName:string) =
            ensure_keys()

            let url = f"%s/private/Withdraw" base_url

            try

                let nonce = DateTime.Now.Ticks
                let values = dict([
                    //("aclass") WTF is "aclass" (asset class) ??
                    ("asset", currency.LowerCase)
                    ("amount", amount.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    ("key", walletName)
                ])

                let props:string = create_props values
                let content = f"nonce=%i%s" nonce props
                let responseMessage = (url.WithApi "/0/private/Withdraw" nonce props public_key secret_key).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

                let operationId = parser.parse_withdrawal(json)

                WithdrawalResponse(true, null, operationId)

            with e -> WithdrawalResponse(false, e.Message, null)