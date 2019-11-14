module currency_mapping

let currency_map = Map<string, string> ([
    ("ZUSD", "USD")
    ("ZEUR", "EUR")

    ("XXRP", "XRP")
    ("XLTC", "LTC")
])


let get_currency kraken_currency = currency_map.[kraken_currency]



