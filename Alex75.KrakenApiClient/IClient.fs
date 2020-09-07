namespace Alex75.KrakenApiClient

open Alex75.Cryptocurrencies

type IClient =
    inherit IApiClientPrivate
    //inherit IApiClientWithInfo
    inherit IApiClientMakeOrders
    inherit IApiClientListOrders
    //inherit IApiClientWithdrawals
          
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse
