module internal cache

open System
open System.Collections.Generic
open Alex75.Cryptocurrencies


type internal BalanceCache = {date:DateTime; balance:Dictionary<Currency, decimal>}

let mutable cache_balance: BalanceCache option = None

let getBalance cache_time = 
    if (cache_balance.IsSome && (DateTime.Now - cache_balance.Value.date) < cache_time) 
    then Some cache_balance.Value.balance
    else None

let setBalance balance cache_time =
    cache_balance <- Some {date=DateTime.Now; balance=balance}