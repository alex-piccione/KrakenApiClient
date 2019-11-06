module IntegrationTests.Client

open NUnit.Framework
open FsUnit
open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies

[<TestCase("xrp", "eur")>]
[<TestCase("xrp", "usd")>]
[<TestCase("ETH", "usd")>]
let GetTicker main other =

    let client = Client() :> IClient

    let response = client.GetTicker(Currency(main), Currency(other))

    response |> should not' (be null)
    response.IsSuccess |> should be True
    response.Error |> should be null
    response.Ticker.IsSome |> should be True
    response.Ticker.Value.Currencies |> should equal (CurrencyPair(main, other))



[<Test>]
let ``GetTicker for BTC``() =

    let client = Client() :> IClient

    let response = client.GetTicker(Currency("btc"), Currency("eur"))

    response |> should not' (be null)
    response.IsSuccess |> should be True
    response.Error |> should be null
    response.Ticker.IsSome |> should be True



[<Test>]
let ``GetTicker when asset does not exists`` ()  =
    
    let client = Client() :> IClient
    
    let response = client.GetTicker(Currency("usd"), Currency("eth"))  
    
    response |> should not' (be null)
    response.IsSuccess |> should be False
    response.Error |> should not' (be null)
    response.Ticker.IsNone |> should be True
    