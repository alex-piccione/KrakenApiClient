namespace Alex75.KrakenApiClient

open System
open System.Collections.Generic
open Alex75.Cryptocurrencies
open api.response.models


type IClient =

    inherit IApiClientPrivate

    //abstract member GetBalance: currencies:Currency[] -> BalanceResponse

    abstract member CreateMarketOrder: pair:CurrencyPair * action:OrderSide * buyAmount:decimal -> CreateMarketOrderResponse
    abstract member ListOpenOrders: unit -> OpenOrdersResponse

    abstract member ListClosedOrders: unit -> IEnumerable<ClosedOrder>
    
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse
