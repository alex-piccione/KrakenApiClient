namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module CreateMarketOrder =             

    [<Test; Category("AFFECTS_BALANCE"); Category("REQUIRES_API_KEY")>]   
    // todo: write custom Ignore rule, example : https://amido.com/blog/conditional-ignore-nunit-and-the-ability-to-conditionally-ignore-a-test/
    // [<IgnoreIf("payment involved")>]
    let ``CreateMarketOrder`` () =
        let pair = CurrencyPair("xrp", "gbp")
        let buyAmount = 15m

        let order = client.CreateMarketOrder(CreateOrderRequest.Market(OrderSide.Sell, pair, buyAmount))

        order |> should not' (be null)
        order.Reference |> should not' (be NullOrEmptyString)

        