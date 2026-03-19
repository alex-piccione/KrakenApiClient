module Client.GetTicker

open System
open NUnit.Framework
open FsUnit
open Swensen.Unquote
open Alex75.Cryptocurrencies
open utils

[<Category("REQUIRES_API_KEY")>]
[<TestCase("xrp", "eur")>]
[<TestCase("xrp", "usd")>]
[<TestCase("ETH", "usd")>]
let GetTicker (main:string, other:string) = task {
    let pair = CurrencyPair(main, other)
    let! ticker = client.GetTicker(pair)
    ticker |> should not' (be null)
    ticker.Pair |> should equal (pair)
}

[<Test>]
let ``GetTicker when asset does not exists`` () =
    raises<Exception> <@ client.GetTicker(CurrencyPair("usd", "eth")) |> Async.AwaitTask |> Async.RunSynchronously @>