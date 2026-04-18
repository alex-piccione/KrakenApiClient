namespace Client

open System
open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

module CreateMarketOrder =

    // OBSOLETE
    [<Test; Category("AFFECTS_BALANCE"); Category("REQUIRES_API_KEY")>]
    // todo: write custom Ignore rule, example : https://amido.com/blog/conditional-ignore-nunit-and-the-ability-to-conditionally-ignore-a-test/
    // [<IgnoreIf("payment involved")>]
    let ``CreateMarketOrder`` () =
        let pair = CurrencyPair("xrp", "eur")
        let buyAmount = 15m

        let order = client.CreateMarketOrder(CreateOrderRequest.Market(OrderSide.Sell, pair, buyAmount))

        order |> should not' (be null)
        order.Reference |> should not' (be NullOrEmptyString)

    [<Test; Category("AFFECTS_BALANCE"); Category("REQUIRES_API_KEY")>]
    let ``CreateMarketOrder_new`` () = task {
        let pair = CurrencyPair("xrp", "eur")
        let buyAmount = 15m

        let! order = client.CreateMarketOrder_new(CreateOrderRequest.Market(OrderSide.Sell, pair, buyAmount))

        order |> should not' (be null)
        order.Reference |> should not' (be NullOrEmptyString)
    }