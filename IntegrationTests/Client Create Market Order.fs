namespace IntegrationTests.Client

open System
open System.IO
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open Test_base


module CreateMarketOrder =

    let apiKeys = ApiKeys()

    let public_key = apiKeys.``public key``
    let secret_key = apiKeys.``secret key``

         
    [<Test>]
    let ``CreateMarketOrder when keys are not defined`` () =

        let client = Client() :> IClient
        (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test>][<Ignore("payment involved")>]
    let ``CreateMarketOrder `` () =

        let client = Client(public_key, secret_key) :> IClient

        let pair = CurrencyPair("xrp", "eur")
        let payAmount = 5m

        let response = client.CreateMarketOrder(pair, payAmount)

        response |> should not' (be null)

        