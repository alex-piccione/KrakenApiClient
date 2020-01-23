module UnitTests.parser

open System
open System.IO
open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies



let loadApiResponse fileName =
    File.ReadAllText(Path.Combine( "data", fileName))



[<Test; Category("parsing")>]
let parse_pairs () =
    let json = loadApiResponse "AssetPairs response.json"
    let pairs = parser.parsePairs json

    pairs |> should not' (be Null)
    pairs |> should contain (CurrencyPair("ada", "eth"))
    pairs |> should contain CurrencyPair.BTC_USD


[<Test; Category("parsing")>]
let parse_ticker () =

    let pair = CurrencyPair.XRP_USD
    let json = loadApiResponse "GET ticker response.json"
    let ticker = parser.parseTicker (pair, json)

    ticker.Currencies |> should equal pair
    ticker.Ask |> should equal 0.26076000
    ticker.Bid |> should equal 0.26075000


[<Test; Category("parsing")>]
let ``parse_balance when is error``() =

    let json = loadApiResponse "Balance response - error.json"

    (fun () -> parser.parse_balance(json) |> ignore) |> should throw typeof<Exception>



[<Test; Category("parsing")>]
let ``parse_balance`` () =

    let json = loadApiResponse "Balance response.json"
    
    let balance = parser.parse_balance(json)
    
    balance |> should not' (be null)
    balance.Keys |> should contain (Currency("USD"))
    balance.Keys |> should contain (Currency("EUR"))
    balance.Keys |> should contain (Currency("XRP"))
    balance.Keys |> should contain (Currency("LTC"))

    balance.[Currency("USD")] |> should equal 0m
    balance.[Currency("EUR")] |> should equal 501
    balance.[Currency("XRP")] |> should equal 0.68765056
    balance.[Currency("LTC")] |> should equal 0.0000042500


[<Test>]
let ``parse_order`` () =

    let json = loadApiResponse "create market order response.json"
    
    let struct (orderIds, amount) = parser.parse_order(json)
    
    orderIds |> should contain "O5PWAY-435NAD-6NAI7P"
    amount |> should equal 100.00000000



[<Test>]
let ``parse_open_orders when list is empty`` () =

    let json = loadApiResponse "list Open Orders response (empty list).json"
    
    let orders = parser.parse_open_orders(json)
    
    orders |> should not' (be null)
    orders |> should be Empty 


[<Test>]
let ``parse_open_orders`` () =

    let json = loadApiResponse "list Open Orders response (one limit order untouched).json"
    
    let orders = parser.parse_open_orders(json)
    
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
let ``parse_withdrawal`` () =

    let json = loadApiResponse "Withdraw Funds response.json"
    
    let operationId = parser.parse_withdrawal(json)
    
    operationId |> should not' (be null)
    operationId |> should equal "AIBKMGZ-7GSRPZ-2TDTMQ"