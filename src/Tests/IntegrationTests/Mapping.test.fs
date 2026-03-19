namespace currency_mapper

open NUnit.Framework
open FsUnit

module currency_mapper =

    [<TestCase("ETH", "ETH")>]
    [<TestCase("ETH2", "ETH")>]
    let ``getCurrency contains currency`` (krakenCurrency, currency) =
        currency_mapper.startMapping().GetAwaiter().GetResult() // await to complete
        currency_mapper.getCurrency(krakenCurrency).UpperCase |> should equal currency

    [<TestCase("XETH", "ETH")>]
    [<TestCase("XXBT", "XBT")>]
    [<TestCase("XXRP", "XRP")>]
    let ``fetchAssetsAsync retrieves Krafken currency`` (krakenName, name) = task {
        let! assets = currency_mapper.mapper.fetchAssets ()
        //assets |> Seq.filter (fun (k,c) -> k.Contains("ETH") || c.Contains("ETH")) |> Seq.iter (fun asset -> printf $"{asset}")
        //  (ETH2, ETH2)(ETH2.S, ETH2.S)(ETHFI, ETHFI)(ETHW, ETHW)(WETH, WETH)(XETH, ETH)
        assets |> Seq.contains (krakenName,name) |> should be True
    }