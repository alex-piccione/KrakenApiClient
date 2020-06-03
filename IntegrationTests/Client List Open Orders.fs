namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open utils

[<Category("Client"); Category("REQUIRES_API_KEY")>]
module ListOpenOrders =

    [<Test>]
    let ``List Open Orders`` () =      
        let orders = client.ListOpenOrders() 
        orders |> should not' (be null)     

