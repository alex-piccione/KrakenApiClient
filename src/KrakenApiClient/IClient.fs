namespace Alex75.KrakenApiClient

open System.Threading.Tasks
open Alex75.Cryptocurrencies
open Alex75.Cryptocurrencies.Exchanges

type IClient =
    inherit IApiClientV2  // <- new async interfaces

    // legacy not-async interfaces
    //inherit IApiClient
    //inherit IApiClientPrivate
    //inherit IApiClientWithInfo
    inherit IApiClientMakeOrders
    //inherit IApiClientListOrders
    //inherit IApiClientWithdrawals

    // Custom
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse

    // TODO: take from IApiClientV2 when available
    abstract member ListOpenOrders: unit -> Task<OpenOrder array>
    abstract member ListClosedOrders: unit -> Task<ClosedOrder array> 

    abstract member CreateMarketOrder_new: CreateOrderRequest -> Task<CreateOrderResult>