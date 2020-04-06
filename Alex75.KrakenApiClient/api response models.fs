module api.response.models

open System
open Alex75.Cryptocurrencies


type ClosedOrder(id:String, ``type``:string, side:string, 
        openTime:DateTime, closeTime:DateTime, status:string, reason:string,
        buyQuantity:decimal, payQuantity:decimal, price:decimal, fee:decimal) =

    member this.Id = id

    member this.Type = match ``type`` with  
                       | "market" -> OrderType.Market 
                       | "limit" -> OrderType.Limit
                       | _ -> failwithf "Invalid value for Type: %s" ``type``
    
    member this.Side = match side with 
                       | "buy" -> OrderSide.Buy 
                       | "sell" -> OrderSide.Sell 
                       | _ -> failwithf "Invalid value for Side: %s" side


    

    member this.OpenTime = openTime
    member this.CloseTime = closeTime
    member this.Status = status
    member this.Reason = reason

    member this.BuyQuantity = buyQuantity
    member this.PayQuantity = payQuantity
    member this.Price = price
    member this.Fee = fee

