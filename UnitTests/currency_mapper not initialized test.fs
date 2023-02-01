module UnitTests.currency_mapper.not_initialized

open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies

[<Test>]
let ``any call when mapper not initialized return a clear error`` () =
    let x = currency_mapper
    (fun () -> currency_mapper.getKrakenPair(CurrencyPair.XRP_EUR) |> ignore) |> should throw typeof<System.Exception>
