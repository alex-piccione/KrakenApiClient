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

    let execute_call (method: string -> Task<HttpResponseMessage>) endpoint = task {
        let! response = method endpoint
        let! content = response.Content.ReadAsStringAsync()
        match response.IsSuccessStatusCode with
        | false -> return failwithf $"Response status is not success. {response.StatusCode} {response.ReasonPhrase} | {content[..500]}"
        | true -> return content
    }

    do
        currency_mapper.startMapping().GetAwaiter().GetResult() // await... so testing it, will be simple


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

    interface IClient with

        member this.ListPairs() =
            match cache.GetPairs assets_cache_time with
            | Some pairs -> Task.FromResult pairs
            | _ -> task {
                let! content = execute_call GET "/0/public/AssetPairs"
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
                    let! content = execute_call GET $"/0/public/Ticker?pair={kraken_pair}"
                    let ticker = parser.parseTicker(pair, content)
                    cache.SetTicker ticker
                    return ticker
            }

        member this.GetBalance(): Task<AccountBalance> = task {
            let! content = execute_call POST "/0/private/Balance"
            return parser.parseBalance content <| currency_mapper.getCurrency
        }

        //member this.ListOpenOrdersIsAvailable = true
        member this.ListOpenOrders () = task {
            let! content = execute_call POST "/0/private/OpenOrders"
            return parser.parseOpenOrders content currency_mapper.parseAltPair
            //to try
                // inputs
                // trades = whether or not to include trades in output (optional.  default = false)
                // userref = restrict results to given user reference id (optional)
        }

        //member this.ListOpenOrdersOfCurrenciesIsAvailable = true
        //member this.ListOpenOrdersOfCurrencies(pairs: CurrencyPair[]) =
        //    (this :> IApiClientListOrders).ListOpenOrders()
        //    |> Array.filter (fun order -> Array.contains order.Pair pairs)

        //member this.ListClosedOrdersIsAvailable = true
        member this.ListClosedOrders() = task {
            let! content = execute_call POST "/0/private/ClosedOrders"
            return parser.parseClosedOrders content currency_mapper.parseAltPair
        }

        //member this.ListClosedOrdersOfCurrenciesIsAvailable = false
        //member this.ListClosedOrdersOfCurrencies(pairs:CurrencyPair[]) = failwith "Use ListClosedOrders"

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