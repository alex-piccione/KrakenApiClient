module UnitTests.currency_mapper.main

open System.Collections.Generic
open NUnit.Framework

open Alex75.Cryptocurrencies

let base_url = "https://api.kraken.com/0"

[<SetUp>]
let SetUp () =
    currency_mapper.startMapping base_url

[<Test>]
let ``getCurrency`` () =

    let shouldBeMappedTo outputCurrency currency =
        try
            match currency_mapper.getCurrency currency with
            | correct when correct = outputCurrency -> ()
            | wrong -> failwithf "Currency \"%s\" is mapped to \"%O\" instead of \"%O\"" currency wrong outputCurrency
        with | :? KeyNotFoundException -> failwithf "Currency \"%s\" not found" currency

    "ZUSD" |> shouldBeMappedTo Currency.USD
    "ZEUR" |> shouldBeMappedTo Currency.EUR
    "XXRP" |> shouldBeMappedTo Currency.XRP
    "XETH" |> shouldBeMappedTo Currency.ETH
    // special mapping
    "XXBT" |> shouldBeMappedTo Currency.BTC

[<Test>]
let ``getKrakenPair`` () =

    let shouldBeMappedTo krakenPair pair =
        try
            match currency_mapper.getKrakenPair pair with
            | correct when correct = krakenPair -> ()
            | wrong -> failwithf "CurrencyPair \"%O\" is mapped to \"%s\" instead of \"%s\"" pair wrong krakenPair
        with | :? KeyNotFoundException -> failwithf "CurrencyPair \"%O\" not found" pair

    CurrencyPair.XRP_EUR |> shouldBeMappedTo "XXRPZEUR"
    CurrencyPair.XRP_USD |> shouldBeMappedTo "XXRPZUSD"
    CurrencyPair("ada", "usd") |> shouldBeMappedTo "ADAUSD"
    // special mapping
    CurrencyPair.BTC_USD |> shouldBeMappedTo "XXBTZUSD"

[<Test>]
let ``parseKrakenAltPair`` () =

    let shouldMapTo pair altName  =
        try
            match currency_mapper.parseAltPair altName with
            | correct when correct = pair -> ()
            | wrong -> failwithf "AltName \"%s\" wrongly mapped to \"%O\" instead of \"%O\"" altName wrong pair
        with | :? KeyNotFoundException -> failwithf "AltName \"%s\" not found" altName

    "ADAEUR" |> shouldMapTo (CurrencyPair("ada", "eur"))
    "XRPEUR" |> shouldMapTo CurrencyPair.XRP_EUR
    "XRPUSD" |> shouldMapTo CurrencyPair.XRP_USD
    "XBTUSD" |> shouldMapTo CurrencyPair.BTC_USD
    "XETHEUR" |> shouldMapTo CurrencyPair.ETH_EUR
