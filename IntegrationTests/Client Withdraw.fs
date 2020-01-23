namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module Withdraw =

    let registered_XRP_wallet_name = "Binance" 

         
    [<Test>]
    let ``Withdraw when amount is too low`` () =

        let client = Client(public_key, secret_key) :> IClient

        let response = client.Withdraw(Currency.XRP, 1m, registered_XRP_wallet_name) 
        
        response.IsSuccess |> should be False


    [<Test; Category("AFFECT_BALANCE"); Category("REQUIRE_API_KEY")>]
    let ``Withdraw`` () =        
        
        let client = Client(public_key, secret_key) :> IClient

        let response = client.Withdraw(Currency.XRP, 30m, "Binance") 

        response |> should not' (be null)
        if not response.IsSuccess then failwith response.Error
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.OperationId |> should not' (be NullOrEmptyString)    
