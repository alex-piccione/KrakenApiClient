module utils

open Alex75.Cryptocurrencies

let f = sprintf

let format_pair (main:Currency) (other:Currency) = f"%s%s" main.UpperCase other.UpperCase

let get_kraken_pair main other =
    // Kraken use the old symbol XBT for Bitcoin
    let kraken_main = if main = Currency.BTC then "XBT" else main.UpperCase
    let kraken_other = if other = Currency.BTC then "XBT" else other.UpperCase
    CurrencyPair(kraken_main, kraken_other)