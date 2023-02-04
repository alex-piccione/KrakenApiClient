module UnitTests.currency_mapper.not_initialized

open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies
open System

// this test has to be in a separate module to not have "mappings" initialized by other tests
[<Test>]
let ``any call when mapper not initialized return a clear error`` () =
    let x =  currency_mapper.mapper.mapping <- None
    (fun () -> currency_mapper.getKrakenPair(CurrencyPair.XRP_EUR) |> ignore) |> should throw typeof<Exception>

[<Test>]
let ``any call when mapper not initialized and there is a defined error return the error`` () =
    let x =  currency_mapper.mapper.mapping <- None
    let x =  currency_mapper.mapper.lastError <- Some(Exception("error 1"))
    (fun () -> currency_mapper.getKrakenPair(CurrencyPair.XRP_EUR) |> ignore) |> should throw typeof<Exception>