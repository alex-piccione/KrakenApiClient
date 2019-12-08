namespace Alex75.KrakenApiClient

open Alex75.Cryptocurrencies

type IClient =
    abstract member GetTicker: main:Currency * other:Currency -> TickerResponse
    abstract member GetTicker: pair:CurrencyPair -> TickerResponse

    abstract member GetBalance: currencies:Currency[] -> BalanceResponse
    abstract member CreateMarketOrder: pair:CurrencyPair * action:OrderSide * buyAmount:decimal -> CreateMarketOrderResponse
    abstract member ListOpenOrders: unit -> OpenOrdersResponse
    abstract member Withdraw: currency:Currency * amount:decimal * walletName:string -> WithdrawalResponse