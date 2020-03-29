module internal parser

[<assembly:System.Runtime.CompilerServices.InternalsVisibleTo("UnitTests")>] do()

open System
open System.Collections.Generic
open FSharp.Data
open Alex75.Cryptocurrencies


let private load_result_and_check_errors jsonString =
    let json = JsonValue.Parse(jsonString)    
    let errors = json.["error"].AsArray()    
    if errors.Length > 0 then failwith (errors.[0].AsString())
    json.["result"]

let parsePairs (content:string) =
    let result = load_result_and_check_errors content

    let pairs = new List<CurrencyPair>()
    for key, record in result.Properties() |> Seq.where( fun (key, record) -> not(key.EndsWith(".d"))) // skip "Derivatives"
        do        

        // fucking Kraken, only "some" currencies has "X" (crypto) or "Z" (fiat) as symbol prefix
        // other ("USDT") hasn't !
        // and other are expressed without this convention (XTZ) !
        // so cannot just use "base" and "quote"

        //let _base = record.["base"].AsString()        
        //let quote = record.["quote"].AsString()
         

        //let _base = match record.["base"].AsString() with
        //            | s when s.StartsWith("X") || s.StartsWith("Z") -> s.Substring(1)
        //            | s -> s

        //let quote = match record.["quote"].AsString() with
        //            | s when s.StartsWith("X") || s.StartsWith("Z") -> s.Substring(1)
        //            | s -> s    

        let wsname = record.["wsname"].AsString().Split('/')
        let _base = utils.normalize_symbol wsname.[0]
        let quote = utils.normalize_symbol wsname.[1]

        pairs.Add(CurrencyPair(_base, quote))    

    pairs

let parseTicker (pair:CurrencyPair, data:string) =
    let result = load_result_and_check_errors data            

    let (name, values) = result.Properties().[0]
    
    let ask = values.Item("a").[0].AsDecimal()
    let bid = values.Item("b").[0].AsDecimal()

    Ticker(pair, bid, ask, None, None, None)


//    (*
//    <pair_name> = pair name
//    a = ask array(<price>, <whole lot volume>, <lot volume>),
//    b = bid array(<price>, <whole lot volume>, <lot volume>),
//    c = last trade closed array(<price>, <lot volume>),
//    v = volume array(<today>, <last 24 hours>),
//    p = volume weighted average price array(<today>, <last 24 hours>),
//    t = number of trades array(<today>, <last 24 hours>),
//    l = low array(<today>, <last 24 hours>),
//    h = high array(<today>, <last 24 hours>),
//    o = today's opening price
//    *)

//{
//"error": [],
//"result": {
//  "XXRPZEUR": {
//    "a": [ "0.26076000", "17300", "17300.000" ],
//    "b": [ "0.26075000", "77", "77.000" ],
//    "c": [ "0.26075000", "1386.03215000" ],
//    "v": [ "985104.92724731", "2259841.10057655" ],
//    "p": [ "0.26212413", "0.26350512" ],
//    "t": [ 751, 1749 ],
//    "l": [ "0.25921000", "0.25921000" ],
//    "h": [ "0.26559000", "0.26700000" ],
//    "o": "0.26430000"
//  }
//}


let parseBalance(jsonString:string) =
    let result = load_result_and_check_errors(jsonString)

    let currenciesBalance =
        result.Properties() 
        |> Seq.map (fun (kraken_currency, amountJson) -> 
                        let currency = currency_mapping.get_currency kraken_currency
                        let ownedAmount = amountJson.AsDecimal()
                        CurrencyBalance(Currency(currency), ownedAmount, ownedAmount)
                    )

    new AccountBalance(currenciesBalance)

    

let parseOrder(jsonString:string) =    
    let result = load_result_and_check_errors(jsonString)

    let order = result.["descr"].["order"].ToString()
    let amount = Decimal.Parse(order.Split(' ').[1])
    let orderIds = result.["txid"].AsArray() |> Array.map (fun v -> v.AsString())

    struct (orderIds, amount)


let parseOpenOrders(jsonString:string) = 
    let result = load_result_and_check_errors(jsonString)

    let orders = List<Order>()
    let ordersJson = result.["open"].Properties()
    for (orderId, order) in ordersJson do
        //let status = order.["status"]
        let timestamp = order.["opentm"].AsDecimal() // 1575484650.7296,
        let creationDate = DateTimeOffset.FromUnixTimeSeconds(int64 timestamp).DateTime
        //let creationDate = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime
        let data = order.["descr"]
        //let pair = CurrencyPair.parse(data.["pair"].AsString())
        let orderSide = match data.["type"].AsString() with
                        | "sell" -> OrderSide.Sell
                        | "buy" -> OrderSide.Buy
                        | _ -> failwithf "Order side not recognized: %s" (data.["type"].AsString())
            
        let orderType = match data.["ordertype"].AsString() with
                        | "limit" -> OrderType.Limit
                        | "market" -> OrderType.Market
                        | _ -> failwithf "Order type not recognized: %s" (data.["ordertype"].AsString())
        let orderAmount = Decimal.Parse(order.["vol"].AsString())
        let priceString = data.["price"].AsString()
        let price:Nullable<decimal> = Nullable<decimal>(Decimal.Parse(priceString)) // if limit orders      
     
        orders.Add (Order(orderId, creationDate, orderType, orderSide, Currency("xrp"), Currency("eur"), orderAmount, price) )
   
    orders.ToArray()
     

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



let parseClosedOrders (jsonString:string) = 
    raise (NotImplementedException())
    
    //let result = load_result_and_check_errors(jsonString)
       

    //let orders = List<ClosedOrder>()

    // orders

    
let parseWithdrawal(jsonString:string) =    
    let result = load_result_and_check_errors(jsonString)
    result.["refid"].AsString()