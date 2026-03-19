namespace Alex75.KrakenApiClient

open Alex75.Cryptocurrencies
open Alex75.Cryptocurrencies.Exchanges

type IClient =
    // new async interfaces
    inherit IApiClientV2

    // legacy not-async interfaces
    //inherit IApiClient
    //inherit IApiClientPrivate
    //inherit IApiClientWithInfo
    inherit IApiClientMakeOrders
    inherit IApiClientListOrders
    //inherit IApiClientWithdrawals

    // Custom
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse
