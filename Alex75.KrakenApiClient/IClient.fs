namespace Alex75.KrakenApiClient

open System.Collections.Generic
open Alex75.Cryptocurrencies

type IClient =
    inherit IApiClientPrivate
    inherit IApiClientMakeOrders

    //abstract member CreateMarketOrder: pair:CurrencyPair * action:OrderSide * buyAmount:decimal -> CreateMarketOrderResponse
    abstract member ListClosedOrders: unit -> ICollection<ClosedOrder>    
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse
