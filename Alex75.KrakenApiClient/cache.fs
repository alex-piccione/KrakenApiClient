module internal cache

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Alex75.Cryptocurrencies
open Alex75.KrakenApiClient



type internal TickerCache = {date:DateTime; ticker:Ticker}

let cache_tickers = ConcurrentDictionary<string, TickerCache>() 

let create_key (pair:CurrencyPair) = sprintf "%O%O" pair.Main pair.Other

let getTicker currency_pair cache_time = 
    let found, item = cache_tickers.TryGetValue (create_key currency_pair)
    if found && (DateTime.Now - item.date) < cache_time 
    then Some item.ticker
    else None

let setTicker currency_pair ticker =

    let tickerCache = { date=DateTime.Now; ticker=ticker }
    let key = create_key currency_pair
    if cache_tickers.ContainsKey key 
    then cache_tickers.[key] <- tickerCache
    else cache_tickers.TryAdd(key, tickerCache) |> ignore


type internal BalanceCache = {date:DateTime; balance:Dictionary<Currency, decimal>}

let mutable cache_balance: BalanceCache option = None

let getBalance cache_time = 
    if (cache_balance.IsSome && (DateTime.Now - cache_balance.Value.date) < cache_time) 
    then Some cache_balance.Value.balance
    else None

let setBalance balance cache_time =
    cache_balance <- Some {date=DateTime.Now; balance=balance}