module IntegrationTests.Client.ListPairs

open System.Linq
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

    let ewt_eur = pairs.SingleOrDefault( fun p -> p = CurrencyPair("EWT", "EUR") )
    ewt_eur |> should not' (be Null)
    ewt_eur.OrderDecimals.IsSome |> should be True
    ewt_eur.OrderDecimals.Value |> should equal 3
