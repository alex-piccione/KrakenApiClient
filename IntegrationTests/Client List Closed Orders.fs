namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open utils

[<Category("Client"); Category("REQUIRES_API_KEY")>]
module ListClosedOrders =

    [<Test>]
    let ``List Closed Orders`` () =        
    
        let client = Client(public_key, secret_key) :> IClient

        let orders = client.ListClosedOrders() 

        orders |> should not' (be null) 
        orders |> should not' (be Empty)  
    