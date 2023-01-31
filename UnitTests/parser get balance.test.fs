module UnitTests.parser.get_balance

open System
open System.IO
open System.Reflection
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open UnitTests.parser.main

let shouldHaveCurrency currency ownedAmount (balance:AccountBalance)  =
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

    balance |> shouldHaveCurrency Currency.XRP 100m
