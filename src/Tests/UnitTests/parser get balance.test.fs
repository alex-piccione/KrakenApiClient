module UnitTests.parser.parseBalance

open System
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open UnitTests.parser.main

let shouldHaveCurrency currency ownedAmount (balance:AccountBalance) =
    if balance.HasCurrency(currency:Currency) then
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
    | "ADA" -> Currency.ADA
    | _ -> Currency(k)

[<Test>]
let ``parseBalance when is error``() =
    let json = readResource "Balance Error.response.json"
    (fun () -> parser.parseBalance json (fun k -> Currency(k)) |> ignore) |> should throw typeof<Exception>

[<Test>]
let ``parseBalance`` () =
    let json = readResource "Balance.response.json"
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
let ``parseBalance with Stacking`` ()=
    let json = readResource "Balance with Stacking.response.json"
    let balance = parser.parseBalance json normalizeCurrency
    balance |> shouldHaveCurrency Currency.DOT (1.11m + 2.22m + 3.33m) // DOT + DOT.S + DOT28.S

[<Test>]
let ``parseBalance with many cases`` ()=
    let json = readResource "Balance with many assets.response.json"

    let balance = parser.parseBalance json normalizeCurrency

    balance |> shouldHaveCurrency Currency.ETH (0.0001646140m + 0.0230299580m + 0.2500398160m) //XETH +  ETH2 + ETH2.S
    balance |> shouldHaveCurrency Currency.USDC 2347.91604938m
    balance |> shouldHaveCurrency (Currency("FLOW")) (0.0000000000m + 77.1046636650m) // FLOW + FLOW.S
    balance |> shouldHaveCurrency Currency.SOL (0.0000089800m + 0.0052996500m) // SOL + SOL.S
    balance |> shouldHaveCurrency Currency.USDT 0.00008351m
    balance |> shouldHaveCurrency Currency.FIL 13.0448980000m
    balance |> shouldHaveCurrency Currency.ADA (0.00000005m + 0.79254500m) // ADA + ADA.S
    balance |> shouldHaveCurrency Currency.GBP 2009.8895m
    balance |> shouldHaveCurrency Currency.FLR (7833.3514M + 0.0000M) // FLR.S + FLR
    balance |> shouldHaveCurrency Currency.BCH 0.0000000000m
    balance |> shouldHaveCurrency Currency.MANA 0.0000000000M
    balance |> shouldHaveCurrency Currency.DOT (1.0571093100m + 325.2791016100m + 0.0000000000m) // DOT + DOT28.S + DOT.S
    balance |> shouldHaveCurrency Currency.USD 0.0000m
    balance |> shouldHaveCurrency Currency.XTZ (0.00000000m + 3062.28558100m) // XTZ + XTZ.S
    balance |> shouldHaveCurrency Currency.GRT (0.1922176300m + 103.2074529200m) // GRT + GRT28.S
    balance |> shouldHaveCurrency Currency.SGB (0.0000095062m) 
    balance |> shouldHaveCurrency Currency.BTC (0.0591452420m)
    balance |> shouldHaveCurrency Currency.EWT (0.0000000000m)
    balance |> shouldHaveCurrency Currency.EUR (4103.8420m)
    balance |> shouldHaveCurrency Currency.OCEAN (3032.6018400000m)
    balance |> shouldHaveCurrency Currency.STORJ (50.0000046000m)
    balance |> shouldHaveCurrency (Currency("UST")) (111.111111110m)
    balance |> shouldHaveCurrency (Currency("SC")) (440000.0000000000m)
    balance |> shouldHaveCurrency (Currency("LUNA2")) (0.92091819m)
    balance |> shouldHaveCurrency Currency.LTC (1.0000000000m)
    balance |> shouldHaveCurrency Currency.XRP (3973.29885459m)
    balance |> shouldHaveCurrency (Currency("ETHW")) (0.0000019m)

[<Test>]
let ``parseBalance with Flexible Stacking`` ()=
    let json = readResource "Balance with Flexible Stacking.response.json"
    let balance = parser.parseBalance json normalizeCurrency

    balance |> shouldHaveCurrency Currency.ADA (0.00000000m + 0.00050265m + 23.03690434m) // ADA + ADA.F + ADA.S
    balance |> shouldHaveCurrency Currency.DOT (0.00000000m + 458.7557891616m + 0.0000000000m + 0.0000000000m) // DOT + DOT.F + DOT.S + DOT28.S
    balance |> shouldHaveCurrency Currency.ETH (0.0000000000m + 0.0264431431m + 0.0000000000m + 0.2500398160m) // XETH + ETH.F + ETH2 + ETH2.S
    balance |> shouldHaveCurrency (Currency("ETHW")) (0.0000019m)

[<Test>]
let ``parseBalance with ETH2`` ()=
    let json = readResource "Balance with ETH2.response.json"
    let balance = parser.parseBalance json normalizeCurrency

    balance |> shouldHaveCurrency Currency.ETH (0.1m + 0.2m + 0.4m + + 0.8m) // XETH + ETH2 + ETH2.S + ETH.F