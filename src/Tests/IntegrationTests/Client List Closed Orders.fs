namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open utils

[<Category("Client"); Category("REQUIRES_API_KEY")>]
module ListClosedOrders =

    [<Test>]
    let ``List Closed Orders`` () =        
        let orders = client.ListClosedOrders() 

        orders |> should not' (be null) 
        orders |> should not' (be Empty)  

        orders.[0].Status |> should not' (be Empty)
    