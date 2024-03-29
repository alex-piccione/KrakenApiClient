<<<<<<<< HEAD:src/Tests/UnitTests/parser.test.fs
﻿module UnitTests.parser.main
========
﻿module UnitTests.parser
>>>>>>>> master:src/Tests/UnitTests/parser test.fs

open System
open System.IO
open System.Reflection
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies

let assembly = Assembly.GetExecutingAssembly()

let readResource resourceName =
    let resourceFullName = $"{assembly.GetName().Name}.data.{resourceName}"
    let names = assembly.GetManifestResourceNames()
    if not(Array.contains resourceFullName names) then failwith $@"Cannot find ""{resourceName}"" in the embedded resources"

    use reader = new StreamReader(assembly.GetManifestResourceStream(resourceFullName))
    reader.ReadToEnd()

let loadApiResponse fileName =
    let path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
    File.ReadAllText(Path.Combine(path, "data", fileName))


[<Test>]
let parsePairs () =
    let json = readResource "AssetPairs response.json"
    let pairs = parser.parsePairs json

    pairs |> should not' (be Null)
    pairs |> should contain (CurrencyPair("ada", "eth"))
    pairs |> should contain CurrencyPair.BTC_USD
    pairs |> should contain CurrencyPair.XRP_USD

    pairs |> should contain (CurrencyPair("ewt", "eur"))
    let ewt_eur = pairs.Find( fun p -> p = CurrencyPair("ewt", "eur") )
    ewt_eur.OrderDecimals.IsSome |> should be True
    ewt_eur.OrderDecimals.Value |> should equal 3

[<Test>]
let parseTicker () =

    let pair = CurrencyPair.XRP_USD
    let json = loadApiResponse "GET ticker response.json"
    let ticker = parser.parseTicker (pair, json)

    ticker.Pair |> should equal pair
    ticker.Ask |> should equal 0.26076000
    ticker.Bid |> should equal 0.26075000

[<Test>]
let ``parseOrder`` () =

    let json = readResource "create market order response.json"

    let struct (orderIds, amount) = parser.parseOrder(json)

    orderIds |> should contain "O5PWAY-435NAD-6NAI7P"

    amount |> should equal 100.00000000

//[<Test>]
//let ``parseOpenOrders when list is empty`` () =

//    let json = loadApiResponse "list Open Orders response (empty list).json"

//    let orders = parser.parseOpenOrders (json, (fun k -> CurrencyPair(k,k)))

//    orders |> should not' (be null)
//    orders |> should be Empty

[<Test>]
let ``parseOpenOrders`` () =

    let json = loadApiResponse "list Open Orders response (one limit order untouched).json"

    let normalizePair = fun _ -> CurrencyPair.XRP_EUR

    let orders = parser.parseOpenOrders (json, normalizePair)

    orders |> should not' (be null)
    orders |> should not' (be Empty)

    orders.Length |> should equal 1
    let order = orders.[0]

    //order.CreationDate |> should equalWithin (new DateTime(2019,12,04 18,37,00) TimeSpan.FromSeconds(1))
    order.Id |> should equal "OGD4S7-IISGH-2BS2QI"
    order.Pair |> should equal CurrencyPair.XRP_EUR
    order.Type |> should equal OrderType.Limit
    order.Side |> should equal OrderSide.Sell
    order.BuyOrSellQuantity |> should equal 250.00000000m
    order.LimitPrice |> should equal 0.30m

[<Test>]
let ``parseClosedOrders`` () =

    let json = loadApiResponse "list Closed Orders.json"

    let orders = parser.parseClosedOrders json (fun k -> CurrencyPair(k,k))

    orders |> should not' (be Null)
    orders.Length |> should equal 4

    let order = orders.[0]
    order.Id |> should equal "AAA5CK-GKYF6-HEMAAA"
    order.Type |> should equal OrderType.Market
    order.Side |> should equal OrderSide.Buy

    let tolerance = TimeSpan.FromMilliseconds(1.)

    order.OpenTime |> should (equalWithin tolerance) (DateTime(2020, 03, 29, 15, 48, 34).AddSeconds(0.1998))  // 199 ms
    order.CloseTime |> should (equalWithin tolerance) (DateTime(2020, 03, 29, 15, 48, 34).AddSeconds(0.2100))  // 209 ms
    order.Status |> should equal "closed"
    order.Reason |> should equal ""

    order.BuyOrSellQuantity |> should equal 2321.93000000
    order.PaidOrReceivedQuantity |> should equal 383.52689
    order.Price |> should equal 0.15604
    order.Fee |> should equal 1.02316

    let limitOrder = orders.[3]
    limitOrder.Id |> should equal "RRRV24-S3RFZ-USXDDD"
    limitOrder.Fee |> should equal 4.7m
    limitOrder.PaidOrReceivedQuantity |> should equal 2958.5

[<Test>]
let ``parseWithdrawal`` () =

    let json = loadApiResponse "Withdraw Funds response.json"

    let operationId = parser.parseWithdrawal(json)

    operationId |> should not' (be null)
    operationId |> should equal "AIBKMGZ-7GSRPZ-2TDTMQ"