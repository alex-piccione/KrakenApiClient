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
    | "ETH" -> Currency.ETH
    | "DOT" -> Currency.DOT
    | _ -> Currency(k)

 
[<Test>]
let ``parseBalanceEx when is error``() =
    let json = readResource "BalanceEx response - error.json"
    (fun () -> parser.parseBalanceEx json (fun k -> Currency(k)) |> ignore) |> should throw typeof<Exception>

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
 