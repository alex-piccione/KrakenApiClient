module api.response.models

open System


//[<AllowNullLiteralAttribute>]
//type Balance (amount:decimal) =
//    member __.Amount = amount

//    (*
//    eb = equivalent balance (combined balance of all currencies)
//    tb = trade balance (combined balance of all equity currencies)
//    m = margin amount of open positions
//    n = unrealized net profit/loss of open positions
//    c = cost basis of open positions
//    v = current floating valuation of open positions
//    e = equity = trade balance + unrealized net profit/loss
//    mf = free margin = equity - initial margin (maximum margin available to open new positions)
//    ml = margin level = (equity / initial margin) * 100
//    *)

type ClosedOrder(closeDate:DateTime, amount:decimal, price:decimal) =
    member this.CloseDate = closeDate
    member this.Amount = amount
    member this.Price = price