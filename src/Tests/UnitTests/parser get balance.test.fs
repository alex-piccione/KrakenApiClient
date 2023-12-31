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
    | _ -> Currency(k)

[<Test>]
let ``parseBalance`` () =
    let json = readResource "Balance response.json"
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
let ``parseBalance when is error``() =
    let json = readResource "Balance response - error.json"
    (fun () -> parser.parseBalance json (fun k -> Currency(k)) |> ignore) |> should throw typeof<Exception>

[<Test>]
let ``parseBalance with Stacking`` ()=
    let json = readResource "Balance response 2.json"
    let balance = parser.parseBalance json normalizeCurrency
    balance |> shouldHaveCurrency Currency.DOT (1.11m + 2.22m + 3.33m) // DOT + DOT.S + DOT28.S

[<Test>]
let ``parseBalance with Stacking (many cases)`` ()=
    let json = readResource "Balance response 3.json"

    let balance = parser.parseBalance json normalizeCurrency

    balance |> shouldHaveCurrency Currency.ETH 0.0001646140m
    balance |> shouldHaveCurrency Currency.USDC 2347.91604938m
    balance |> shouldHaveCurrency (Currency("FLOW")) (0.0000000000m + 77.1046636650m) // FLOW + FLOW.S
    balance |> shouldHaveCurrency Currency.SOL (0.0000089800m + 0.0052996500m) // SOL + SOL.S
    balance |> shouldHaveCurrency Currency.USDT 0.00008351m
    balance |> shouldHaveCurrency Currency.FIL 13.0448980000m
    balance |> shouldHaveCurrency (Currency("ETH2")) (0.0230299580m + 0.2500398160m) // ETH2 + ETH2.S
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
let ``parseBalanceEx`` () =
    let json = readResource "BalanceEx response.json"

    let balance = parser.parseBalanceEx json normalizeCurrency

    balance.HasCurrency "USD" |> should be True
    balance.HasCurrency "BTC" |> should be True

    balance[Currency("USD")].Total |> should equal 25435.21m
    balance[Currency("USD")].Free |> should equal (25435.21m - 8249.76m)

    balance[Currency("BTC")].Total |> should equal 1.2435m
    balance[Currency("BTC")].Free |> should equal (1.2435m - 0.8423m)


[<Test>]
let ``parseBalanceEx with Stacking`` () =
    let json = readResource "BalanceEx response 2.json"

    let balance = parser.parseBalanceEx json normalizeCurrency

    balance.HasCurrency "GBP" |> should be True
    balance.HasCurrency "EUR" |> should be True
    balance.HasCurrency "XTZ" |> should be True
    balance.HasCurrency "FLR" |> should be True

    balance[Currency("GBP")].Total |> should equal 6162.8347m
    balance[Currency("GBP")].InOrders |> should equal 6159.8251m
    balance[Currency("GBP")].Free |> should equal (6162.8347m - 6159.8251m)

    balance[Currency("EUR")].Total |> should equal 1437.7916m
    balance[Currency("EUR")].Free |> should equal (1437.7916m - 0m)

    balance[Currency("XTZ")].Total |> should equal 1.05277600m
    balance[Currency("XTZ")].Free |> should equal (1.05277600m - 0m)

    balance[Currency("FLR")].Total |> should equal 246.5735m
    balance[Currency("FLR")].Stacking |> should equal 246.5735m
    balance[Currency("FLR")].Free |> should equal 0m

[<Test>]
let ``parseBalanceEx with multiple stackings`` () =
    let json = readResource "BalanceEx response 3.json"
    
    let balance = parser.parseBalanceEx json normalizeCurrency
    
    balance.HasCurrency "DOT" |> should be True
    
    balance[Currency("DOT")].Total |> should equal 14m
    balance[Currency("DOT")].Free |> should equal (14m-4m-8m)
    balance[Currency("DOT")].InOrders |> should equal 0m
    balance[Currency("DOT")].Stacking |> should equal (4m + 8m)
 