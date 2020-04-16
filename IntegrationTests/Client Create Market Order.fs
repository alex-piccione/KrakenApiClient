namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module CreateMarketOrder =
             

    [<Test; Category("AFFECTS_BALANCE"); Category("REQUIRES_API_KEY")>]   
    // todo: write custom Ignore rule, example : https://amido.com/blog/conditional-ignore-nunit-and-the-ability-to-conditionally-ignore-a-test/
    // [<IgnoreIf("payment involved")>]
    let ``CreateMarketOrder`` () =

        let client = Client(public_key, secret_key) :> IClient

        let pair = CurrencyPair("xrp", "eur")
        let buyAmount = 30m

        let response = client.CreateMarketOrder(pair, OrderSide.Sell, buyAmount)

        response |> should not' (be null)
        response.IsSuccess |> should be True
        response.Amount |> should equal buyAmount
        response.OrderIds |> should not' (be Empty)

        