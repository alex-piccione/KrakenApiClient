module UnitTests.parser

open System
open System.IO
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies



let loadApiResponse fileName =
    File.ReadAllText(Path.Combine( "data", fileName))

[<Test>]
let parse_ticker () =

    let pair = CurrencyPair.XRP_USD
    let json = loadApiResponse "GET ticker response.json"
    let ticker = parser.parseTicker (pair, json)

    ticker.Currencies |> should equal pair
    ticker.Ask |> should equal 0.26076000
    ticker.Bid |> should equal 0.26075000


[<Test>]
let ``parse_balance when is error``() =

    let json = loadApiResponse "GetBalance response.json"

    (fun () -> parser.parse_balance(json) |> ignore) |> should throw typeof<Exception>
    //(parser.parse_balance(json) |> ignore) |> should throw typeof<Exception>

    
    //balance |> should not' (be null)
    //balance.Amount |> should equal 


