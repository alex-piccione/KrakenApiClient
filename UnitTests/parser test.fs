module UnitTests.parser

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