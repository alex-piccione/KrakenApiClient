module currency_mapping_test

open NUnit.Framework
open FsUnit

[<Test; Category("mapping")>]
let ``get_currency`` () =

    currency_mapping.get_currency "ZUSD" |> should equal "USD"
    currency_mapping.get_currency "ZEUR" |> should equal "EUR"
    currency_mapping.get_currency "Zusd" |> should equal "USD"


[<Test; Category("mapping")>]
let ``get_kraken_currency`` () =

    currency_mapping.get_kraken_currency "USD" |> should equal "ZUSD"

