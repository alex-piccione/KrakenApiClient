module UnitTests.parser.parseBalance

open System
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open UnitTests.parser.main

let shouldHaveCurrency currency ownedAmount (balance:AccountBalance) =
    if balance.HasCurrency(currency) then
        match balance.[currency].Total with
        | correct when correct = ownedAmount -> ()
        | wrong -> failwithf "Currency \"%O\" Owned amount is %f instead of %f" currency wrong ownedAmount
    else failwithf "Currency \"%O\" not found" currency

let normalizeCurrency = fun k ->
    match k with
    | "ZUSD" -> Currency.USD
    | "ZEUR" -> Currency.EUR
    | "ZGBP" -> Currency.GBP
    | "XXBT" -> Currency.BTC
    | "XXRP" -> Currency.XRP
    | "XLTC" -> Currency.LTC
    | "XETH" -> Currency.ETH
    | "DOT" -> Currency.DOT
    | _ -> Currency(k)

[<Test>]
let ``parseBalance when is error``() =
    let json = loadApiResponse "Balance response - error.json"
    (fun () -> parser.parseBalance json (fun k -> Currency(k)) |> ignore) |> should throw typeof<Exception>

[<Test>]
let ``parseBalance`` () =
    let json = loadApiResponse "Balance response.json"

    let balance = parser.parseBalance json normalizeCurrency

    balance |> should not' (be null)
    balance |> shouldHaveCurrency Currency.USD 0m
    balance |> shouldHaveCurrency Currency.EUR 778.9688m
    balance |> shouldHaveCurrency Currency.GBP 1108.5946m

    balance |> shouldHaveCurrency Currency.XRP 6457.14680403m
    balance |> shouldHaveCurrency Currency.BTC 0.4500000000m
    balance |> shouldHaveCurrency Currency.LTC 0.0000042500m
    balance |> shouldHaveCurrency Currency.ETH 0.0000000200m
    balance |> shouldHaveCurrency Currency.ADA 0.76461705m
    balance |> shouldHaveCurrency Currency.XTZ 0m
    balance |> shouldHaveCurrency Currency.DOT 662.24614826m

[<Test>]
let ``parseBalance [with] Stacking and BoundedStacking`` ()=
    let json = readResource("Balance response 2.json")

    let balance = parser.parseBalance json normalizeCurrency

    balance |> shouldHaveCurrency Currency.ETH 0.0001646140m
    balance |> shouldHaveCurrency Currency.USDC 2347.91604938m
    balance |> shouldHaveCurrency (Currency("FLOW")) 77.1046636650m // "FLOW.S"
    balance |> shouldHaveCurrency (Currency("SOL")) (0.0000089800m + 0.0052996500m)
    balance |> shouldHaveCurrency Currency.USDT 0.00008351m
    //"FIL": "13.0448980000",
    //"ETH2": "0.0230299580",
    //"ADA": "0.00000005",
    //"ADA.S": "0.79254500",
    //"ZGBP": "2009.8895",
    //"FLR.S": "7833.3514",
    //"BCH": "0.0000000000",
    //"MANA": "0.0000000000",
    //"DOT": "1.0571093100",
    //"ZUSD": "0.0000",
    //"XTZ": "0.00000000",
    //"XTZ.S": "3062.28558100",
    //"ETH2.S": "0.2500398160",
    //"GRT": "0.1922176300",
    //"SGB": "0.0000095062",
    //"GRT28.S": "103.2074529200",
    //"DOT28.S": "325.2791016100",
    //"XXBT": "0.0591452420",
    //"EWT": "0.0000000000",
    //"FLOW": "0.0000000000",
    //"ZEUR": "4103.8420",
    //"OCEAN": "3032.6018400000",
    //"STORJ": "50.0000046000",
    //"SOL.S": "0.0052996500",
    //"UST": "111.11111111",
    //"FLR": "0.0000",
    //"SC": "440000.0000000000",
    //"LUNA2": "0.92091819",
    //"DOT.S": "0.0000000000",
    //"XLTC": "1.0000000000",
    //"XXRP": "3973.29885459",
    //"ETHW": "0.0000019"

    
//    balance |> shouldHaveCurrency Currency.XRP 100m