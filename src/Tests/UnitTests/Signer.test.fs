module UnitTests.Signer

open NUnit.Framework
open FsUnit

[<Test>]
let ``createSignature produce expected signature as per official documentation example`` () =

    // ggshield: ignore
    let privateKey = "kQH5HW/8p1uGOVjbgWA7FunAmGO8lsSUXNsu3eow76sz84Q18fWxnyRzBHCd3pd5nE9qa99HAZtuZuj6F1huXg=="
    let nonce = "1616492376594"
    let path = "/0/private/AddOrder"
    let expectedPayload = "nonce=1616492376594&ordertype=limit&pair=XBTUSD&price=37500&type=buy&volume=1.25"
    let expectedSignature = "4/dpxb3iT4tp/ZCVEwSnEsLxx0bqyhLpdfOpc6fn7OR8+UClSV5n9E6aSS8MPtnRfp32bAb0nmbRn6H8ndwLUQ=="

    let data = Map.ofSeq<string, obj> [
        //"nonce", "1616492376594"
        "ordertype", "limit"
        "pair","XBTUSD"
        "price", "37500"
        "type", "buy"
        "volume", "1.25"
    ]

    let payload, signature = utils.createSignature path privateKey data nonce
    payload |> should equal expectedPayload
    signature |> should equal expectedSignature