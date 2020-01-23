namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module GetBalance =

         
    [<Test>]
    let ``GetBalance when keys are not defined`` () =

        let client = Client() :> IClient
        (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test; Category("REQUIRE_API_KEY")>]
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