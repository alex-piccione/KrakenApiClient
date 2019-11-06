module parser

open System
open FSharp.Data
open Alex75.Cryptocurrencies

//type ApiResponse = {error:Object[]; result:Object}


//let parseTicker (data:Stream) =
//    JsonValue.Read()

//let extract_pair name =

//    (*
//    The X and Z in front of some pairs is a classification system, which will not be used for the newest coins, where X stands for cryptocurrency based assets while Z is for fiat based assets.
//    XRPUSD = XXRPZUSD  
//    *)

//    match name with 
//    | "XXRPZUSD" -> CurrencyPair.XRP_USD
//    | _ -> Curre



let parseTicker (pair:CurrencyPair, data:string) =

    let json = JsonValue.Parse(data)
    let errors =  json.["error"].AsArray()

    if errors.Length > 0 then
        let error = errors.[0].ToString()
        failwith error
        
    let result = json.["result"]
    let (name, values) = result.Properties().[0]
    
    let ask = values.Item("a").[0].AsDecimal()
    let bid = values.Item("b").[0].AsDecimal()

    Ticker(pair, bid, ask, None, None, None)


//    if data.Errors.Length <> 0 then
//        failwith (data.Errors.[0].ToString())

//    let res_1 = data.Result.[0]

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
