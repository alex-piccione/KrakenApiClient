namespace IntegrationTests.Client

open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module GetBalance =

    [<Test; Category("REQUIRES_API_KEY")>]
    let ``GetBalance()`` () =
        let balance = client.GetBalance()
        balance |> should not' (be null)

        balance.HasCurrency(Currency.XRP) |> should be True
        balance.HasCurrency(Currency.GBP) |> should be True
        balance.HasCurrency(Currency.BTC) |> should be True
        balance.HasCurrency(Currency.DOT) |> should be True
        //balance[Currency.DOT].Total |> should be (greaterThan 0m)
        //balance[Currency.DOT].Stacking |> should be (greaterThan 0m)
        balance[Currency.ADA].Total |> should be (greaterThan 0m)
        balance[Currency.ADA].Stacking |> should be (greaterThan 0m)