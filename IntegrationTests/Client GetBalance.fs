namespace IntegrationTests.Client

open System
open System.IO
open NUnit.Framework
open FsUnit
open Alex75.KrakenApiClient
open Alex75.Cryptocurrencies

module GetBalance =

    let mutable public_key:string  = null
    let mutable secret_key:string  = null

    [<OneTimeSetUp>]
    let oneTimeSetup () =
        let secrets_file = match File.Exists("api keys.secret.txt") with
                           | true -> "api keys.secret.txt"
                           | false -> "api keys.txt"
        
        if not (File.Exists secrets_file) then failwith "API keys text file not found"
        for line in File.ReadAllLines(secrets_file) do
            let values = line.Split("=")
            match values.[0] with 
            | "public key" -> public_key <- values.[1]
            | "secret key" -> secret_key <- values.[1]
            | _ -> failwithf "unknown key: %s" values.[0]
           


    [<Test>]
    let ``GetBalance when keys are not defined`` () =

        let client = Client() :> IClient
        (fun () -> client.GetBalance([|Currency.BTC|]) |> ignore) |> should throw typeof<Exception>


    [<Test>]
    let ``GetBalance`` () =

        let client = Client(public_key, secret_key) :> IClient

        let response = client.GetBalance( [|Currency("xrp"); Currency("eur")|])        

        response |> should not' (be null)
        response.IsSuccess |> should be True
        response.Error |> should be null
        response.CurrenciesBalance |> should not' (be null)        
        response.CurrenciesBalance.ContainsKey(Currency("xrp")) |> should be True
        response.CurrenciesBalance.ContainsKey(Currency("eur")) |> should be True
        response.CurrenciesBalance.ContainsKey(Currency("btc")) |> should be False