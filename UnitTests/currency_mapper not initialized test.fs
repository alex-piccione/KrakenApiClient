[<NUnit.Framework.Category("mapping")>]
module currency_mapper_not_initialized_test

open NUnit.Framework
open FsUnit
open Alex75.Cryptocurrencies

[<Test>]
let ``any call when mapper not initialized return a clear error`` () =
    (fun () -> currency_mapper.getKrakenPair(CurrencyPair.XRP_EUR) |> ignore) |> should throw typeof<System.Exception>
