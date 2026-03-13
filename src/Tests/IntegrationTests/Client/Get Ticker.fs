module Client.GetTicker

open System
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open utils

[<TestCase("xrp", "eur")>]
[<TestCase("xrp", "usd")>]
[<TestCase("ETH", "usd")>]
let GetTicker (main:string, other:string) =
    let pair = CurrencyPair(main, other)
    let ticker = client.GetTicker(pair)
    ticker |> should not' (be null)
    ticker.Pair |> should equal (pair)

[<Test>]
let ``GetTicker when asset does not exists`` () =
    (fun () -> client.GetTicker(CurrencyPair("usd", "eth")) |> ignore ) |> should throw typeof<Exception>

[<TestCase("xrp", "eur")>]
[<TestCase("xrp", "usd")>]
[<TestCase("ETH", "usd")>]
let GetTickerAsync (main:string, other:string) = task {
    let pair = CurrencyPair(main, other)
    let! ticker = client.GetTickerAsync(pair)
    ticker |> should not' (be null)
    ticker.Pair |> should equal (pair)
}