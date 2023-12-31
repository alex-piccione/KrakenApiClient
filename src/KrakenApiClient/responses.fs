namespace Alex75.KrakenApiClient

open Alex75.Cryptocurrencies

[<AbstractClass>]
type Response (isSuccess:bool, error:string) =
    member __.IsSuccess = isSuccess
    member __.Error = error

type TickerResponse (isSuccess:bool, error:string, ticker:Option<Ticker>) =
    inherit Response (isSuccess, error)
    member __.Ticker = ticker

type CreateMarketOrderResponse (isSuccess:bool, error:string, orderIds:string[], amount:decimal) =
    inherit Response (isSuccess, error)
    member __.OrderIds = orderIds
    member __.Amount = amount

    static member Fail error = CreateMarketOrderResponse(false, error, null, 0m)

type WithdrawalResponse (isSuccess:bool, error:string, operationId:string) =
    inherit Response(isSuccess, error)
    member __.OperationId = operationId