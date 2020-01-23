namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module ListOpenOrders =

         
    [<Test>]
    let ``GetBalance when keys are not defined`` () =

        let client = Client() :> IClient
        (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test; Category("REQUIRE_API_KEY")>]
    let ``List Open Orders`` () =        
        
        let client = Client(public_key, secret_key) :> IClient

        let response = client.ListOpenOrders() 

        response |> should not' (be null)
        if not response.IsSuccess then failwith response.Error
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.Orders |> should not' (be null)    
        //response.Orders |> should not' (be Empty)    