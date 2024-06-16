namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

module CreateLimitOrder =

    [<Test; Category("AFFECTS_BALANCE"); Category("REQUIRES_API_KEY")>]
    // todo: write custom Ignore rule, example : https://amido.com/blog/conditional-ignore-nunit-and-the-ability-to-conditionally-ignore-a-test/
    // [<IgnoreIf("payment involved")>]
    let ``CreateLimitOrder`` () =
        let pair = CurrencyPair("xrp", "eur")
        let buyAmount = 15m
        let limitPrice = 0.75m

        let ordererence = client.CreateLimitOrder(CreateOrderRequest.Limit(OrderSide.Sell, pair, buyAmount, limitPrice))

        ordererence |> should not' (be NullOrEmptyString)
