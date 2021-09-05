namespace Alex75.KrakenApiClient

open System
open System.Linq
open System.Collections.Generic

open Flurl.Http
open Alex75.Cryptocurrencies
open utils


type public Client (public_key:string, secret_key:string) =  

    let base_url = "https://api.kraken.com/0"
    let cache = new Cache()
    let assets_cache_time = TimeSpan.FromHours 6.0
    let ticker_cache_time = TimeSpan.FromSeconds 10.0
    let balance_cache_time = TimeSpan.FromSeconds 30.0

    let ensure_keys () = if String.IsNullOrWhiteSpace(public_key) || String.IsNullOrWhiteSpace(secret_key) then failwith "This method requires public and secret keys"

    let create_content (properties:IDictionary<string, string>) =
        let nonce = DateTime.UtcNow.Ticks.ToString()
        //properties.Add("nonce", nonce)
        let content = properties 
                        |> Seq.map (fun kv -> sprintf "&%s=%s" kv.Key kv.Value)
                        |> Seq.fold (+) ("nonce=" + nonce)
        //let content = f"nonce=%i%s" nonce content
        let nonce_content = f"%s%s" nonce content
        (nonce_content, content)    

    do
        currency_mapper.startMapping base_url    

    new () = Client(null, null)  


    member this.CreateMarketOrder (pair:CurrencyPair, side:OrderSide, buyAmount:decimal) =
        ensure_keys()

        let url = f"%s/private/AddOrder" base_url            
        let kraken_pair = currency_mapper.getKrakenPair pair

        let values = dict [
            "pair", kraken_pair
            "type", match side with 
                    | OrderSide.Buy -> "buy"
                    | OrderSide.Sell -> "sell"
            "ordertype", "market" 
            //("price")
            "volume", buyAmount.ToString(System.Globalization.CultureInfo.InvariantCulture) // ???? {"error":["EGeneral:Invalid arguments:volume"]}
            //("leverage")
            //("oflags", "viqc") // volume in quote currency   // no more available !
            //("validate", "true") // ANY value (also validate=true) will be a simulation, order id not returned
        ]
            
        let nonce_content, content = create_content values
        let responseMessage = (url.WithApi "/0/private/AddOrder" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
        let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

        let struct (orderIds, amount) = parser.parseOrder(json)
            
        CreateMarketOrderResponse(true, null, orderIds, amount)                

        //with e -> CreateMarketOrderResponse.Fail e.Message


    interface IClient with

        // public //

        member this.ListPairs() =
            match cache.GetPairs assets_cache_time with
            | Some pairs -> pairs
            | _ -> 
                let responseContent = (f"%s/public/AssetPairs" base_url).GetStringAsync().Result
                let pairs = parser.parsePairs responseContent
                cache.SetPairs pairs
                pairs :> ICollection<CurrencyPair>

        member this.GetTicker(pair: CurrencyPair): Ticker = 
            let cached_ticker = cache.GetTicker pair ticker_cache_time
            match cached_ticker with 
                | Some ticker -> ticker
                | _ ->
                     let kraken_pair = currency_mapper.getKrakenPair pair
                     let url = f"%s/public/Ticker?pair=%s" base_url kraken_pair
                     let responseMessage = url.GetAsync().Result
                     let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
                     let ticker = parser.parseTicker(pair, json)
                     cache.SetTicker ticker
                     ticker

        // private //

        member this.GetBalance(): AccountBalance = 
            ensure_keys()

            match cache.GetAccountBalance(balance_cache_time) with
            | Some balance -> balance
            | _ -> 
                let url = f"%s/private/Balance" base_url  
                let nonce_content, content = create_content (dict [])
                let balances = 
                    (url.WithApi "/0/private/Balance" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
                        .EnsureSuccessStatusCode()
                        |> fun msg -> msg.Content.ReadAsStringAsync().Result                        
                        |> parser.parseBalance <| currency_mapper.getCurrency

                cache.SetAccountBalance balances
                balances 


        member this.ListOpenOrdersIsAvailable = true
        member this.ListOpenOrders () =
            ensure_keys()

            let url = f"%s/private/OpenOrders" base_url
            
            //to try
                // inputs
                // trades = whether or not to include trades in output (optional.  default = false)
                // userref = restrict results to given user reference id (optional)

            let nonce_content, content = create_content (dict [])
            let responseMessage = (url.WithApi "/0/private/OpenOrders" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
            let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
            parser.parseOpenOrders(json, currency_mapper.parseAltPair)   


        member this.ListOpenOrdersOfCurrenciesIsAvailable = true
        member this.ListOpenOrdersOfCurrencies(pairs: CurrencyPair[]) =
            (this :> IApiClientListOrders).ListOpenOrders()
            |> Array.filter (fun order -> Array.contains order.Pair pairs)


        member this.ListClosedOrdersIsAvailable = true
        member this.ListClosedOrders() = 
            ensure_keys()

            let url = f"%s/private/ClosedOrders" base_url

            // inputs
            // trades = whether or not to include trades in output (optional.  default = false)
            // userref = restrict results to given user reference id (optional)

            let nonce_content, content = create_content (dict [])
            let responseMessage = (url.WithApi "/0/private/ClosedOrders" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
            let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
            parser.parseClosedOrders json currency_mapper.parseAltPair           
        
        // todo: add an override to accept the Kraken custom filter parameters

        member this.ListClosedOrdersOfCurrenciesIsAvailable = false
        member this.ListClosedOrdersOfCurrencies(pairs:CurrencyPair[]) = failwith "Use ListClosedOrders"


        // Place Order

        member this.CreateMarketOrder (request:CreateOrderRequest): CreateOrderResult =
            let result = this.CreateMarketOrder(request.Pair, request.Side, request.BuyOrSellQuantity)
            if result.IsSuccess then CreateOrderResult(String.Join(",", result.OrderIds), 0m)
            else failwith result.Error


        member this.CreateLimitOrder(request: CreateOrderRequest): string = 
            ensure_keys()
            let url = f"%s/private/AddOrder" base_url            
            let kraken_pair = currency_mapper.getKrakenPair request.Pair


            let price = request.LimitPrice.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)
            let precision = 5

            let priceString = System.Math.Round( request.LimitPrice.Value, precision).ToString(System.Globalization.CultureInfo.InvariantCulture)

            let values = dict [
                "pair", kraken_pair
                "type", match request.Side with 
                        | OrderSide.Buy -> "buy"
                        | OrderSide.Sell -> "sell"
                "ordertype", "limit" 
                "price", priceString 
                "volume", request.BuyOrSellQuantity.ToString(System.Globalization.CultureInfo.InvariantCulture) // ???? {"error":["EGeneral:Invalid arguments:volume"]}
                //("leverage")
                //("oflags", "viqc") // volume in quote currency   // no more available !
                //("validate", "true") // ANY value (also validate=true) will be a simulation, order id not returned
            ]
                       
            let nonce_content, content = create_content values
            let responseMessage = (url.WithApi "/0/private/AddOrder" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
            let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

            //{"error":["EOrder:Invalid price:XXRPZEUR price can only be specified up to 5 decimals."]}

            "??" // todo: parser.parseLimitOrder(json)


        member this.Withdraw (currency:Currency, amount:decimal, walletName:string) =
            ensure_keys()

            let url = f"%s/private/Withdraw" base_url

            try
                let values = dict([
                    //("aclass") WTF is "aclass" (asset class) ??
                    ("asset", currency.LowerCase)
                    ("amount", amount.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    ("key", walletName)
                ])

                let nonce_content, content = create_content values
                let responseMessage = (url.WithApi "/0/private/Withdraw" nonce_content public_key secret_key).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

                let operationId = parser.parseWithdrawal(json)

                WithdrawalResponse(true, null, operationId)

            with e -> WithdrawalResponse(false, e.Message, null)