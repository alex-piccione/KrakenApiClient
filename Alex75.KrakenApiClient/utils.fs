module utils

open System
open System.Text
open Alex75.Cryptocurrencies
open Flurl
open Flurl.Http


let f = sprintf

let format_pair (main:Currency) (other:Currency) = f"%s%s" main.UpperCase other.UpperCase

let get_kraken_pair main other =
    // Kraken use the old symbol XBT for Bitcoin
    let kraken_main = if main = Currency.BTC then "XBT" else main.UpperCase
    let kraken_other = if other = Currency.BTC then "XBT" else other.UpperCase
    CurrencyPair(kraken_main, kraken_other)


let normalize_symbol kraken_value =
    match kraken_value with 
    | "XBT" -> "BTC"
    | s -> s

//let convert_to_kraken_symbol value =
//    match value.ToUpper() with 
//    | "BTC" -> "XBT"
//    | s -> s


let sha256_hash (value:string) = 
    System.Security.Cryptography.SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(value))

let getHash (keyBytes:byte[], messageBytes:byte[]) =
    (new System.Security.Cryptography.HMACSHA512(keyBytes)).ComputeHash(messageBytes)


/// extend Flurl to add API key and signature
type Flurl.Http.IFlurlRequest with
    member self.WithApi (api_path:string) (nonce:int64) content public_key secret_key =         
                         
        let base64DecodedSecred = Convert.FromBase64String(secret_key)
        let np = f"%inonce=%i%s" nonce (*+ Convert.ToChar(0) *) nonce content 

        let pathBytes = Encoding.UTF8.GetBytes(api_path)
        let hash256Bytes = sha256_hash(np)
        let z = Array.append pathBytes hash256Bytes

        let signature =  Convert.ToBase64String(getHash(base64DecodedSecred, z))

        self.WithHeader("API-Key", public_key).WithHeader( "API-Sign", signature)        
        


type String with
    member self.WithApi api_path nonce props public_key secret_key = FlurlRequest(Url(self)).WithApi api_path nonce props public_key secret_key
