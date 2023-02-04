namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.Cryptocurrencies
open utils

[<Category("Client"); Category("REQUIRES_API_KEY")>]
module Withdraw =

    let registered_XRP_wallet_name = "Binance" // must be registered on the website

    [<Test>]
    let ``Withdraw when amount is too low`` () =
        let response = client.Withdraw(Currency.XRP, 1m, registered_XRP_wallet_name)
        response.IsSuccess |> should be False

    [<Test; Category("AFFECTS_BALANCE"); >]
    let ``Withdraw`` () =

        let response = client.Withdraw(Currency.XRP, 5m, registered_XRP_wallet_name)

        response |> should not' (be null)
        if not response.IsSuccess then failwith response.Error
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.OperationId |> should not' (be NullOrEmptyString)