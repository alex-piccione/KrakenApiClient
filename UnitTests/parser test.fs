﻿[<NUnit.Framework.Category("parsing")>]
module UnitTests.parser

open System
open System.IO
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies



let loadApiResponse fileName =
    File.ReadAllText(Path.Combine( "data", fileName))



[<Test>]
let parsePairs () =
    let json = loadApiResponse "AssetPairs response.json"
    let pairs = parser.parsePairs json

    pairs |> should not' (be Null)
    pairs |> should contain (CurrencyPair("ada", "eth"))
    pairs |> should contain CurrencyPair.BTC_USD


[<Test>]
let parseTicker () =

    let pair = CurrencyPair.XRP_USD
    let json = loadApiResponse "GET ticker response.json"
    let ticker = parser.parseTicker (pair, json)

    ticker.Pair |> should equal pair
    ticker.Ask |> should equal 0.26076000
    ticker.Bid |> should equal 0.26075000


[<Test>]
let ``parseBalance when is error``() =

    let json = loadApiResponse "Balance response - error.json"

    (fun () -> parser.parseBalance(json) |> ignore) |> should throw typeof<Exception>



[<Test>]
let ``parseBalance`` () =

    let json = loadApiResponse "Balance response.json"
    
    let balance = parser.parseBalance(json)
    
    balance |> should not' (be null)
    balance.HasCurrency("usd") |> should be True
    balance.HasCurrency("eur") |> should be True
    balance.HasCurrency("xrp") |> should be True
    balance.HasCurrency("ltc") |> should be True

    balance.[Currency("USD")].OwnedAmount |> should equal 0m
    balance.[Currency("EUR")].OwnedAmount |> should equal 501
    balance.[Currency("XRP")].OwnedAmount |> should equal 0.68765056
    balance.[Currency("LTC")].OwnedAmount |> should equal 0.0000042500


[<Test>]
let ``parseOrder`` () =

    let json = loadApiResponse "create market order response.json"
    
    let struct (orderIds, amount) = parser.parseOrder(json)
    
    orderIds |> should contain "O5PWAY-435NAD-6NAI7P"
    amount |> should equal 100.00000000



[<Test>]
let ``parseOpenOrders when list is empty`` () =

    let json = loadApiResponse "list Open Orders response (empty list).json"
    
    let orders = parser.parseOpenOrders(json)
    
    orders |> should not' (be null)
    orders |> should be Empty 


[<Test>]
let ``parseOpenOrders`` () =

    let json = loadApiResponse "list Open Orders response (one limit order untouched).json"
    
    let orders = parser.parseOpenOrders(json)
    
    orders |> should not' (be null)
    orders |> should not' (be Empty) 

    orders.Length |> should equal 1
    let order = orders.[0]

    //order.CreationDate |> should equalWithin (new DateTime(2019,12,04 18,37,00) TimeSpan.FromSeconds(1))
    order.Id |> should equal "OGD4S7-IISGH-2BS2QI"
    order.MainCurrency |> should equal (Currency("Xrp"))
    order.OtherCurrency |> should equal (Currency("eur"))
    order.Type |> should equal OrderType.Limit
    order.Side |> should equal OrderSide.Sell
    order.Price |> should equal 0.30m


[<Test>]
let ``parseWithdrawal`` () =

    let json = loadApiResponse "Withdraw Funds response.json"
    
    let operationId = parser.parseWithdrawal(json)
    
    operationId |> should not' (be null)
    operationId |> should equal "AIBKMGZ-7GSRPZ-2TDTMQ"