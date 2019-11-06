namespace Alex75.KrakenApiClient

open Alex75.Cryptocurrencies

[<AbstractClass>]
type Response (isSuccess:bool, error:string) = 
    member __.IsSuccess = isSuccess
    member __.Error = error

type TickerResponse (isSuccess:bool, error:string, ticker:Option<Ticker>) =
    inherit Response (isSuccess, error)
    member __.Ticker = ticker