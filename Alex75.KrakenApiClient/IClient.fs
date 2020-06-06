namespace Alex75.KrakenApiClient

open System.Collections.Generic
open Alex75.Cryptocurrencies

type IClient =
    inherit IApiClientPrivate
    inherit IApiClientMakeOrders
        
    abstract member ListClosedOrders: unit -> ICollection<ClosedOrder>    
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse
