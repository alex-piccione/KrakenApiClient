namespace currency_mapper

open NUnit.Framework
open FsUnit

module currency_mapper =

    let base_url = "https://api.kraken.com/0"

    [<TestCase("ETH", "ETH")>]
    [<TestCase("ETH2", "ETH")>]
    let ``getCurrency contains currency`` (krakenCurrency, currency) =
        currency_mapper.startMapping(base_url)
        currency_mapper.getCurrency(krakenCurrency).UpperCase |> should equal currency


    [<TestCase("XETH", "ETH")>]
    [<TestCase("XXBT", "XBT")>]
    [<TestCase("XXRP", "XRP")>]
    let ``fetchAssetsAsync retrieves Krafken currency`` (krakenName, name) = async {
        let! assets = currency_mapper.mapper.fetchAssetsAsync(base_url)
        //assets |> Seq.filter (fun (k,c) -> k.Contains("ETH") || c.Contains("ETH")) |> Seq.iter (fun asset -> printf $"{asset}")
        //  (ETH2, ETH2)(ETH2.S, ETH2.S)(ETHFI, ETHFI)(ETHW, ETHW)(WETH, WETH)(XETH, ETH)
        assets |> Seq.contains (krakenName,name) |> should be True
    }
