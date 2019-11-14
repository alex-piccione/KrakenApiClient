namespace IntegrationTests.Client

open System
open System.IO
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open Test_base

module GetBalance =

    let apiKeys = ApiKeys()

    let public_key = apiKeys.``public key``
    let secret_key = apiKeys.``secret key``

         
    [<Test>]
    let ``GetBalance when keys are not defined`` () =

        let client = Client() :> IClient
        (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test>]
    let ``GetBalance`` () =

        let client = Client(public_key, secret_key) :> IClient

        let response = client.GetBalance( [|Currency("xrp"); Currency("eur")|])        

        response |> should not' (be null)
        if not response.IsSuccess then failwith response.Error
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.CurrenciesBalance |> should not' (be null)    
        
        response.CurrenciesBalance.Keys |> should contain (Currency("xrp"))
        response.CurrenciesBalance.Keys |> should contain (Currency("eur"))
        //response.CurrenciesBalance.Keys |> should contain (Currency("btc"))