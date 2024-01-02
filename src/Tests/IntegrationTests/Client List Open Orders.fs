namespace IntegrationTests.Client

open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open utils

[<Category("Client"); Category("REQUIRES_API_KEY")>]
module ListOpenOrders =

    [<Test>]
    let ``List Open Orders`` () =
        let orders = client.ListOpenOrders()
        orders |> should not' (be null)

    [<Test>]
    let ``List Open Orders with spefified pairs`` () =
        let orders = client.ListOpenOrdersOfCurrencies([|CurrencyPair.ADA_XRP;CurrencyPair.XRP_EUR|])
        orders |> should not' (be null)