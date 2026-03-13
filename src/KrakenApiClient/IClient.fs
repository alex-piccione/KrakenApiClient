namespace Alex75.KrakenApiClient

open System.Collections.Generic
open System.Threading.Tasks
open Alex75.Cryptocurrencies

type IClient =
    inherit IApiClient
    inherit IApiClientPrivate
    //inherit IApiClientWithInfo
    inherit IApiClientMakeOrders
    inherit IApiClientListOrders
    //inherit IApiClientWithdrawals

    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse

    // ASYNC methods
    abstract ListPairsAsync: unit -> Task<ICollection<CurrencyPair>>
    abstract GetTickerAsync: CurrencyPair -> Task<Ticker>
  