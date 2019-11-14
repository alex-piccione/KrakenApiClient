namespace Alex75.KrakenApiClient

open System.Collections.Generic
open Alex75.Cryptocurrencies
open api.response.models



[<AbstractClass>]
type Response (isSuccess:bool, error:string) = 
    member __.IsSuccess = isSuccess
    member __.Error = error

type TickerResponse (isSuccess:bool, error:string, ticker:Option<Ticker>) =
    inherit Response (isSuccess, error)
    member __.Ticker = ticker



type BalanceResponse (isSuccess:bool, error:string, currenciesBalance:IDictionary<Currency, decimal>) =
    inherit Response (isSuccess, error)
    member __.CurrenciesBalance = currenciesBalance