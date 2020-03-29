[<NUnit.Framework.Category("mapping")>]
module currency_mapping_test

open NUnit.Framework
open FsUnit
open System.Collections.Generic

[<Test>]
let ``get_currency`` () =

    let shouldBeMappedTo outputCurrency currency =
        try 
            match currency_mapping.get_currency currency with
            | correct when correct = outputCurrency -> ()
            | wrong -> failwithf "Currency \"%s\" is mapped to \"%s\" instead of \"%s\"" currency wrong outputCurrency
        with | :? KeyNotFoundException -> failwithf "Currency \"%s\" not found" currency


    "ZUSD" |> shouldBeMappedTo "USD"
    "ZEUR" |> shouldBeMappedTo "EUR"
    "Zusd" |> shouldBeMappedTo "USD"
    // special mapping
    "XXBT" |> shouldBeMappedTo "BTC"


[<Test>]
let ``get_kraken_currency`` () =

    let shouldBeMappedTo outputCurrency currency =
        try 
            match currency_mapping.get_kraken_currency currency with
            | correct when correct = outputCurrency -> ()
            | wrong -> failwithf "Currency \"%s\" is mapped to \"%s\" instead of \"%s\"" currency wrong outputCurrency
        with | :? KeyNotFoundException -> failwithf "Currency \"%s\" not found" currency

    "USD" |> shouldBeMappedTo "ZUSD"
    "BTC" |> shouldBeMappedTo "XXBT"

