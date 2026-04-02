namespace Alex75.KrakenApiClient

open System
open System.Collections.Generic
open System.Threading.Tasks
open System.Net.Http
open Flurl.Http
open Alex75.Cryptocurrencies
open Alex75.Cryptocurrencies.Exchanges
open utils

type public Client (publicKey:string, secretKey:string) =

    let base_url = "https://api.kraken.com"
    let cache = new Cache()
    // TODO: move to Constants.fs
    let assets_cache_time = TimeSpan.FromHours 6.0
    let ticker_cache_time = TimeSpan.FromSeconds 10.0

    let client = new HttpClient(BaseAddress = Uri(base_url))

    do
        if String.IsNullOrWhiteSpace(publicKey) then failwith "Public key is empty"
        if String.IsNullOrWhiteSpace(secretKey) then failwith "Secret key is empty"

    let GET path = client.GetAsyncWithSignature path publicKey secretKey
    let POST path = client.PostAsyncWithSignature path publicKey secretKey


    let create_content (properties:IDictionary<string, string>) =
        let nonce = DateTime.UtcNow.Ticks.ToString()
        let content = properties
                        |> Seq.map (fun kv -> sprintf "&%s=%s" kv.Key kv.Value)
                        |> Seq.fold (+) ("nonce=" + nonce)

        let nonce_content = nonce + content
        (nonce_content, content)

    do
        currency_mapper.startMapping().GetAwaiter().GetResult() // await... so test it will be simple


    new () = Client(null, null)

    member this.CreateMarketOrder (pair:CurrencyPair, side:OrderSide, buyAmount:decimal) =

        let url = $"{base_url}/0/private/AddOrder"
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
        let responseMessage = (url.WithApi "/0/private/AddOrder" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
        let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

        let struct (orderIds, amount) = parser.parseCreateOrder(json)

        CreateMarketOrderResponse(true, null, orderIds, amount)


    member this.GetBalance(): AccountBalance =
        let url = $"{base_url}/private/Balance"
        let nonce_content, content = create_content (dict [])
        let balances =
            (url.WithApi "/0/private/Balance" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
                .EnsureSuccessStatusCode()
                |> fun msg -> msg.Content.ReadAsStringAsync().Result
                |> parser.parseBalance <| currency_mapper.getCurrency
        balances

    interface IClient with

        member this.ListPairs() =
            match cache.GetPairs assets_cache_time with
            | Some pairs -> Task.FromResult pairs
            | _ -> task {
                let! response = client.GetAsync "/0/public/AssetPairs"
                let! content = response.Content.ReadAsStringAsync()
                match response.IsSuccessStatusCode with
                | false -> return failwithf $"Response status is not success. {response.StatusCode} {response.ReasonPhrase} | {content[..500]}"
                | true -> 
                    let pairs = parser.parsePairs content
                    cache.SetPairs pairs
                    return pairs :> ICollection<CurrencyPair>
            }

        member this.GetTicker(pair: CurrencyPair): Task<Ticker> = task {
            let cached_ticker = cache.GetTicker pair ticker_cache_time
            match cached_ticker with
                | Some ticker -> return ticker
                | _ ->
                    let kraken_pair = currency_mapper.getKrakenPair pair
                    let! response = client.GetAsync $"/0/public/Ticker?pair={kraken_pair}"
                    let! content = response.Content.ReadAsStringAsync()
                    match response.IsSuccessStatusCode with
                    | false -> return failwithf $"Response status is not success. {response.StatusCode} {response.ReasonPhrase} | {content[..500]}"
                    | true -> 
                        let ticker = parser.parseTicker(pair, content)
                        cache.SetTicker ticker
                        return ticker
        }

        member this.GetBalance(): Task<AccountBalance> = task {
            let! response = POST "/0/private/Balance"
            let! content = response.Content.ReadAsStringAsync()
            match response.IsSuccessStatusCode with 
            | false -> return failwith $"Failed to call GetBalance. {response.StatusCode} {response.ReasonPhrase} {content}"
            | true -> return parser.parseBalance content <| currency_mapper.getCurrency
        }

        member this.ListOpenOrdersIsAvailable = true
        member this.ListOpenOrders () =

            let url = $"{base_url}/0/private/OpenOrders"

            //to try
                // inputs
                // trades = whether or not to include trades in output (optional.  default = false)
                // userref = restrict results to given user reference id (optional)

            let nonce_content, content = create_content (dict [])
            let responseMessage = (url.WithApi "/0/private/OpenOrders" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
            let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result
            parser.parseOpenOrders(json, currency_mapper.parseAltPair)

        member this.ListOpenOrdersOfCurrenciesIsAvailable = true
        member this.ListOpenOrdersOfCurrencies(pairs: CurrencyPair[]) =
            (this :> IApiClientListOrders).ListOpenOrders()
            |> Array.filter (fun order -> Array.contains order.Pair pairs)

        member this.ListClosedOrdersIsAvailable = true
        member this.ListClosedOrders() =

            let url = $"{base_url}/0/private/ClosedOrders"

            // inputs
            // trades = whether or not to include trades in output (optional.  default = false)
            // userref = restrict results to given user reference id (optional)

            let nonce_content, content = create_content (dict [])
            let responseMessage = (url.WithApi "/0/private/ClosedOrders" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
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
            let url = $"{base_url}/0/private/AddOrder"
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
            let responseMessage = (url.WithApi "/0/private/AddOrder" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
            let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

            //{"error":["EOrder:Invalid price:XXRPZEUR price can only be specified up to 5 decimals."]}

            let struct (orderIds, _) = parser.parseCreateOrder(json)
            String.Join(", ", orderIds)

        member this.Withdraw (currency:Currency, amount:decimal, walletName:string) =
            let url = $"{base_url}/0/private/Withdraw"

            try
                let values = dict([
                    //("aclass") WTF is "aclass" (asset class) ??
                    ("asset", currency.LowerCase)
                    ("amount", amount.ToString(System.Globalization.CultureInfo.InvariantCulture))
                    ("key", walletName)
                ])

                let nonce_content, content = create_content values
                let responseMessage = (url.WithApi "/0/private/Withdraw" nonce_content publicKey secretKey).PostUrlEncodedAsync(content).Result
                let json = responseMessage.EnsureSuccessStatusCode().Content.ReadAsStringAsync().Result

                let operationId = parser.parseWithdrawal(json)

                WithdrawalResponse(true, null, operationId)

            with e -> WithdrawalResponse(false, e.Message, null)