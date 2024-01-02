module internal currency_mapper

[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")>] do()

(*
A map of currencies and pairs will be updated continuosly in the background.
Every time the map is updated the "lastUpdate" field is updated with current date and time.
If it fails the first time it raises an error, not on successive failures.
*)

open System
open FSharp.Data
open Flurl.Http
open utils
open Alex75.Cryptocurrencies
open System.Collections.Generic

module internal mapper =

    [<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")>] do()

    type internal Asset = { Name:string; AltName:string}

    /// pairs: Seq of (krakenSymbol, krakenPairAltName, base, quote)
    /// assets: Seq of (name, altname)
    type Mapping (pairs:seq<(string * string * string * string)>, assets:seq<Asset>) =

        let krakenCurrenciesMap = Map(assets |> Seq.map (fun asset ->
            match asset.Name with
            | "XXBT" -> (asset.Name, "BTC")
            | _ -> (asset.Name, asset.AltName) // ZEUR, EUR
        ) |> Seq.append( ["ETH", "ETH"])) // to manage ETH2 currency in stacking, that is manages as XETH in other calls

        // create a key:value map where the key is the kraken pair altname (used in order list) and the value is the standard currency pair
        let krakenAltNamePairMap =
            Map(pairs
                |> Seq.map(fun (krakenPair, krakenAltName, krakenBase_, krakenQuote) ->
                            (krakenAltName, CurrencyPair(krakenCurrenciesMap[krakenBase_], krakenCurrenciesMap[krakenQuote]))
            ))

        // create a key:value map where the key is the standard pair and the value is the Kraken symbol
        let krakenPairMap =
            Map( pairs
            |> Seq.where (fun (krakenPair, _, _, _) -> not(krakenPair.EndsWith(".d")))
            |> Seq.map( fun (krakenPair, krakenAltName, krakenBase_, krakenQuote) ->
                            (CurrencyPair(krakenCurrenciesMap[krakenBase_], krakenCurrenciesMap[krakenQuote]).AAA_BBB, krakenPair)
            ))

        /// given the standard pair returns the Kraken symbol (pair) (make order). XRP/EUR -> XXRPZEUR
        member this.getKrakenPair (pair:CurrencyPair) = krakenPairMap[pair.AAA_BBB]

        /// given the Kraken currency returns the standard currency (for balance). ZEUR -> EUR
        member this.getCurrency (krakenCurrency:string) =
            if not(krakenCurrenciesMap.ContainsKey krakenCurrency) then failwithf "Kraken currencies map does not contain \"%s\"" krakenCurrency
            Currency(krakenCurrenciesMap[krakenCurrency])

        /// given an "altname" (orders list) returns the standard pair. XRPEUR -> XRP/EUR
        member this.getPairFromAltName altName = krakenAltNamePairMap.[altName]

    let mapTTL = TimeSpan.FromHours(6.0)  // 6 hours
    let LOCK = Object()
    let mutable lastError: Option<Exception> = Some(Exception("Mapper is not initialized"))
    let mutable lastUpdate:Option<DateTime> = None
    let mutable mapping:Option<Mapping> = None

    // returns XETHXXBT, ETHXBT, XETH, XXBT
    let fetchPairsAsync baseUrl =
        async {
            return parser.load_result_and_check_errors( (f"%s/public/AssetPairs" baseUrl).GetStringAsync().Result )
                    .Properties()
                    // skip derivatives, technically "XXBTZUSD.d" is exactly the same as "XXBTZUSD"
                    //|> Seq.where (fun (name,json)-> not(name.EndsWith(".d")))
                    |> Seq.map( fun (name,json) ->  name, json["altname"].AsString(), json["base"].AsString(), json["quote"].AsString() )
        }

    // returns ZEUR, EUR
    let fetchAssetsAsync baseUrl =
        async {
            let json = (f"%s/public/Assets" baseUrl).GetStringAsync().Result
            return parser.load_result_and_check_errors(json)
                .Properties()
                |> Seq.map(fun (name, json) -> { Name=name; AltName=json["altname"].AsString()}:Asset )
        }

    let updateMap baseUrl =
        lock LOCK ( fun () ->
            async {
                if lastUpdate.IsNone || (DateTime.Now - lastUpdate.Value) > TimeSpan.FromMinutes(10.) then
                    let mutable run = 0
                    while run = 0 || run < 10 do
                        try
                            let! pairs = fetchPairsAsync baseUrl
                            let! assets = fetchAssetsAsync baseUrl
                            mapping <- Some(Mapping(pairs, assets))
                            lastUpdate <- Some(DateTime.Now)
                            run <- 10 // stop
                            lastError <- None
                        with e ->
                            run <- run + 1
                            lastError <- Some(e)
            }
        )

let startMapping baseUrl =
    let timer = new System.Timers.Timer(mapper.mapTTL.TotalMilliseconds)
    timer.AutoReset <- true
    timer.Elapsed.Add (fun e -> mapper.updateMap(baseUrl) |> Async.RunSynchronously)
    mapper.updateMap(baseUrl) |> Async.RunSynchronously // initial call

/// return the Kraken pair from a common pair
let getKrakenPair pair =
    try
        match mapper.mapping, mapper.lastError with
        | Some m, _ -> m.getKrakenPair pair
        | None, Some e -> raise (Exception("Assets and Pairs mapping not ready", e))
        | None, None -> raise (Exception("Assets and Pairs mapping not ready"))
    with | :? KeyNotFoundException -> failwithf "The pair \"%O\" is not valid" pair

let getCurrency krakenCurrency =
    match mapper.mapping with
    | Some m -> m.getCurrency krakenCurrency
    | None -> raise (Exception("Assets and Pairs mapping not ready", mapper.lastError.Value))

/// Returns the standard pair from the Kraken pair "alt name" (orders list use this)
let parseAltPair pairAltName =
    match mapper.mapping with
    | Some m -> m.getPairFromAltName pairAltName
    | None -> raise (Exception("Assets and Pairs mapping not ready", mapper.lastError.Value)) 