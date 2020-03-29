namespace IntegrationTests.Client

open System
open NUnit.Framework
open FsUnit

open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies
open utils

[<Category("Client")>]
module GetBalance =

         
    //[<Test>]
    //let ``GetBalance when keys are not defined`` () =

    //    let client = Client() :> IClient
    //    (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>

    [<Test; Category("REQUIRES_API_KEY")>]
    let ``GetBalance()`` () =   
        
        let client = Client(public_key, secret_key) :> IClient

        let balance = client.GetBalance()        

        balance |> should not' (be null)

        balance.HasCurrency("xrp") |> should be True
        let (hasBalance, amount) = balance.TryGetCurrency("xrp")
        hasBalance |> should be True

        balance.HasCurrency("gbp") |> should be True
        


    //[<Test; Category("REQUIRES_API_KEY")>]
    //let ``GetBalance(currency[])`` () =      
       
    //    let client = Client(public_key, secret_key) :> IClient

    //    let response = client.GetBalance( [|Currency("xrp"); Currency("eur")|])        

    //    response |> should not' (be null)
    //    if not response.IsSuccess then failwith response.Error
    //    response.IsSuccess |> should be True
    //    response.Error |> should be null
    //    response.CurrenciesBalance |> should not' (be null)    
        
    //    response.CurrenciesBalance.Keys |> should contain (Currency("xrp"))
    //    response.CurrenciesBalance.Keys |> should contain (Currency("eur"))
    //    //response.CurrenciesBalance.Keys |> should contain (Currency("btc"))