namespace IntegrationTests.Client

open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

module CreateLimitOrder =

    [<Test; Category("REQUIRES_API_KEY"); Category("AFFECTS_BALANCE")>]
    let ``CreateBuyLimitOrder`` () =
        let pair = CurrencyPair("xrp", "eur")
        let buyAmount = 15m
        let limitPrice = 1.20m

        let ordererence = client.CreateLimitOrder(CreateOrderRequest.Limit(OrderSide.Buy, pair, buyAmount, limitPrice))

        ordererence |> should not' (be NullOrEmptyString)

    [<Test; Category("REQUIRES_API_KEY"); Category("AFFECTS_BALANCE")>]
    let ``CreateSellLimitOrder`` () =
        let pair = CurrencyPair("xrp", "eur")
        let sellAmount = 15m
        let limitPrice = 1.30m

        let ordererence = client.CreateLimitOrder(CreateOrderRequest.Limit(OrderSide.Sell, pair, sellAmount, limitPrice))

        ordererence |> should not' (be NullOrEmptyString)