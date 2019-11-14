module api.response.models

open System


[<AllowNullLiteralAttribute>]
type Balance (amount:decimal) =
    member __.Amount = amount

    (*
    eb = equivalent balance (combined balance of all currencies)
    tb = trade balance (combined balance of all equity currencies)
    m = margin amount of open positions
    n = unrealized net profit/loss of open positions
    c = cost basis of open positions
    v = current floating valuation of open positions
    e = equity = trade balance + unrealized net profit/loss
    mf = free margin = equity - initial margin (maximum margin available to open new positions)
    ml = margin level = (equity / initial margin) * 100
    *)


//type Ticker (pair_name:String ) = 
    
   // member __.Pair = pair

    (*
    <pair_name> = pair name
    a = ask array(<price>, <whole lot volume>, <lot volume>),
    b = bid array(<price>, <whole lot volume>, <lot volume>),
    c = last trade closed array(<price>, <lot volume>),
    v = volume array(<today>, <last 24 hours>),
    p = volume weighted average price array(<today>, <last 24 hours>),
    t = number of trades array(<today>, <last 24 hours>),
    l = low array(<today>, <last 24 hours>),
    h = high array(<today>, <last 24 hours>),
    o = today's opening price

    {
    "error": [],
    "result": {
      "XXRPZEUR": {
        "a": [ "0.26076000", "17300", "17300.000" ],
        "b": [ "0.26075000", "77", "77.000" ],
        "c": [ "0.26075000", "1386.03215000" ],
        "v": [ "985104.92724731", "2259841.10057655" ],
        "p": [ "0.26212413", "0.26350512" ],
        "t": [ 751, 1749 ],
        "l": [ "0.25921000", "0.25921000" ],
        "h": [ "0.26559000", "0.26700000" ],
        "o": "0.26430000"
      }
    }
    }

    *)


