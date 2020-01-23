module IntegrationTests.Client.ListPairs

open NUnit.Framework
open FsUnit
open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies

[<Test; Category("Client")>]
let ListPairs () =

    let client = Client() :> IClient

    let pairs = client.ListPairs()

    pairs |> should not' (be null)
    pairs |> should contain CurrencyPair.XRP_EUR
    pairs |> should contain CurrencyPair.BTC_USD
