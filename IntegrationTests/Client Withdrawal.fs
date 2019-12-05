namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

module Withdrawal =

         
    //[<Test>]
    //let ``Withdrawal when keys are not defined`` () =

    //    let client = Client() :> IClient
    //    (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test>]
    let ``Withdrawal`` () =        
        
        let client = Client(public_key, secret_key) :> IClient

        let response = client.WithdrawFunds(Currency.XRP, 30m, "Binance") 

        response |> should not' (be null)
        if not response.IsSuccess then failwith response.Error
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.OperationId |> should not' (be NullOrEmptyString)    
