module internal parser

[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")>] do()

open System
open System.Collections.Generic
open FSharp.Data
open Alex75.Cryptocurrencies

let load_result_and_check_errors jsonString =
    let json = JsonValue.Parse(jsonString)
    let errors = json.["error"].AsArray()
    if errors.Length > 0 then failwith (errors.[0].AsString())
    json.["result"]

let parseUnixTime_old (unixTime) = new DateTime(1970, 1,1) + TimeSpan.FromSeconds(float(unixTime))
let parseUnixTime unixTime = DateTimeOffset.FromUnixTimeMilliseconds(int64(unixTime*1000m)).DateTime

let mapBTC currency = if currency = "XBT" then "BTC" else currency

/// Creates a map <Kraken currency>:<currency>
let parseAssets (jsonString) =
    let result = load_result_and_check_errors jsonString
    result.Properties()
    |> Seq.map (fun (name, json) -> (name, json.["altname"].AsString()) )

let parsePairs (content:string) =
    let result = load_result_and_check_errors content

    let pairs = new List<CurrencyPair>()
    for key, record in result.Properties() |> Seq.where( fun (key, record) -> not(key.EndsWith(".d"))) // skip "Derivatives"
        do

        // must be used with a continuosly updated list
        //let _base = currency_mapping.get_currency (record.["base"].AsString())
        //let quote = currency_mapping.get_currency (record.["quote"].AsString())

        let wsname = record.["wsname"].AsString().Split('/')  // WebSocket pair name (if available)
        let base_ = mapBTC wsname.[0]
        let quote = mapBTC wsname.[1]

        let orderDecimals = Some( record.["pair_decimals"].AsInteger())

        pairs.Add(CurrencyPair(base_, quote, orderDecimals))

    pairs

let parseTicker (pair:CurrencyPair, data:string) =
    let result = load_result_and_check_errors data

    let (name, values) = result.Properties().[0]

    let ask = values.["a"].[0].AsDecimal()
    let bid = values.["b"].[0].AsDecimal()

    Ticker(pair, bid, ask, None, None, None)

let parseBalance jsonString normalizeCurrency =
    let result = load_result_and_check_errors jsonString

    let parse (kraken_currency, amountJson) =
        let amount = (amountJson:JsonValue).AsDecimal()
        match (kraken_currency:string).Split('.') with
        | [|code|] -> "", normalizeCurrency kraken_currency , amount
        | [|code;"S"|] -> "stacking", normalizeCurrency code , amount   // TODO: manage XXX28 bounded tickers
        | _ -> failwithf "Unmanaged Kraken currency symbol: \"%s\"" kraken_currency

    let parsed = result.Properties() |> Seq.map parse

    let stackingMap = Map( parsed
                           |> Seq.filter (fun (kind,_,_) -> kind = "stacking")
                           |> Seq.map (fun (kind,currency,amount) -> currency, amount)
    )

    let balances = parsed
                   |> Seq.choose (fun (kind,currency:Currency,amount) ->
                                      match kind with
                                      | "" -> Some(
                                                let balance = CurrencyBalance(currency, amount, amount)
                                                if stackingMap.ContainsKey currency then balance.AddStacking stackingMap.[currency]
                                                else balance
                                                )
                                      | _ -> None )

    let getStackingBalance (currency:Currency, amount) =
        if balances |> Seq.exists (fun balance -> balance.Currency = currency)
        then None
        else Some(CurrencyBalance.Zero(currency).AddStacking amount)

    let stackingBalances =
        stackingMap
        |> Map.toSeq
        |> Seq.choose getStackingBalance

    new AccountBalance(Seq.append balances stackingBalances)

let parseOrder(jsonString:string) =
    let result = load_result_and_check_errors(jsonString)

    let order = result.["descr"].["order"].ToString()
    let amount = Decimal.Parse(order.Split(' ').[1])
    let orderIds = result.["txid"].AsArray() |> Array.map (fun v -> v.AsString())

    struct (orderIds, amount)

let parseOrderType value =  match value with
                            | "limit" -> OrderType.Limit
                            | "market" -> OrderType.Market
                            | "stop-loss" -> OrderType.StopLoss
                            | "stop-loss-limit" -> OrderType.StopLossLimit
                            | "take-profit" -> OrderType.TakeProfit
                            | "take-profit-limit" -> OrderType.TakeProfitLimit
                            | x -> failwithf "Order type not recognized: %s" x

let parseOrderSide value = match value with
                           | "sell" -> OrderSide.Sell
                           | "buy" -> OrderSide.Buy
                           | x -> failwithf "Order side not recognized: %s" x

let parseOpenOrders(jsonString:string, normalizePair) =
    let result = load_result_and_check_errors(jsonString)

    let readOrder (id, json:JsonValue) =
        let descr = json.["descr"]
        let orderSide = parseOrderSide(descr.["type"].AsString())
        let orderType = parseOrderType(descr.["ordertype"].AsString())

        let openTime = parseUnixTime(json.["opentm"].AsDecimal()) // 1575484650.7296,
        let pair = normalizePair(descr.["pair"].AsString())    // this is (illogically) the altname !!!
        let vol = json.["vol"].AsDecimal()   // ???
        //let vol_exec = json.["vol_exec"].AsDecimal()   // ???

        let limitPrice = if orderType = OrderType.Limit then descr.["price"].AsDecimal() else 0m

        OpenOrder(id, orderType, orderSide, openTime, pair, vol, limitPrice)

    result.["open"].Properties() |> Array.map readOrder

//refid = Referral order transaction id that created this order
//userref = user reference id
//status = status of order:
//    pending = order pending book entry
//    open = open order
//    closed = closed order
//    canceled = order canceled
//    expired = order expired
//opentm = unix timestamp of when order was placed
//starttm = unix timestamp of order start time (or 0 if not set)
//expiretm = unix timestamp of order end time (or 0 if not set)
//descr = order description info
//    pair = asset pair
//    type = type of order (buy/sell)
//    ordertype = order type (See Add standard order)
//    price = primary price
//    price2 = secondary price
//    leverage = amount of leverage
//    order = order description
//    close = conditional close order description (if conditional close set)
//vol = volume of order (base currency unless viqc set in oflags)
//vol_exec = volume executed (base currency unless viqc set in oflags)
//cost = total cost (quote currency unless unless viqc set in oflags)
//fee = total fee (quote currency)
//price = average price (quote currency unless viqc set in oflags)
//stopprice = stop price (quote currency, for trailing stops)
//limitprice = triggered limit price (quote currency, when limit based order type triggered)
//misc = comma delimited list of miscellaneous info
//    stopped = triggered by stop price
//    touched = triggered by touch price
//    liquidated = liquidation
//    partial = partial fill
//oflags = comma delimited list of order flags
//    viqc = volume in quote currency
//    fcib = prefer fee in base currency (default if selling)
//    fciq = prefer fee in quote currency (default if buying)
//    nompp = no market price protection

let startDate = DateTime(1970, 1, 1)
let parseDate dateNumber = startDate + TimeSpan.FromSeconds(float(dateNumber))
//DateTimeOffset.FromUnixTimeSeconds

let parseClosedOrders (jsonString:string) normalizePair =
    let result = load_result_and_check_errors(jsonString)

    let readOrder (name, json:JsonValue) =
        let id = name
        let descr = json.["descr"]
        let orderSide = parseOrderSide(descr.["type"].AsString())
        let orderType = parseOrderType(descr.["ordertype"].AsString())

        let openTime = parseDate(json.["opentm"].AsDecimal())
        let closeTime = parseDate(json.["closetm"].AsDecimal())
        let status = json.["status"].AsString()
        let reason = json.["reason"].AsString()

        let amount = 0m
        let price = json.["price"].AsDecimal()

        let pair = normalizePair(descr.["pair"].AsString())
        let vol = json.["vol"].AsDecimal()
        //let vol_exec = json.["vol_exec"].AsDecimal()  // not used
        let buyQuantity = vol
        //let buyQuantity = Math.Min(vol, vol_exec)

        let payQuantity = json.["cost"].AsDecimal()
        let fee = json.["fee"].AsDecimal()

        let note = sprintf "Status: %s, Reason: %s" status reason

        ClosedOrder(id, orderType, orderSide, openTime, closeTime, status, reason, pair, buyQuantity, payQuantity, price, fee, note)

    result.["closed"].Properties() |> Array.map readOrder

    //let orders = List<ClosedOrder>()

    //orders

    (*
    "AAA5CK-GKYF6-HEMAAA": {
           "refid": null,
           "userref": 0,
           "status": "closed",
           "reason": null,
           "opentm": 1585496914.1998,
           "closetm": 1585496914.2097,
           "starttm": 0,
           "expiretm": 0,
           "descr": {
             "pair": "XRPEUR",
             "type": "buy",
             "ordertype": "market",
             "price": "0",
             "price2": "0",
             "leverage": "none",
             "order": "buy 2321.93000000 XRPEUR @ market",
             "close": ""
           },
           "vol": "2321.93000000",
           "vol_exec": "2321.93000000",
           "cost": "383.52689",
           "fee": "1.02316",
           "price": "0.15604",
           "stopprice": "0.00000000",
           "limitprice": "0.00000000",
           "misc": "",
           "oflags": "fciq"
         },
    *)

let parseWithdrawal(jsonString:string) =
    let result = load_result_and_check_errors(jsonString)
    result.["refid"].AsString()