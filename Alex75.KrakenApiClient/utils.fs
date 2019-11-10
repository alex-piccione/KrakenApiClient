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


let sha256_hash (value:string) = 
    System.Security.Cryptography.SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(value))

let getHash (keyBytes:byte[], messageBytes:byte[]) =
    (new System.Security.Cryptography.HMACSHA512(keyBytes)).ComputeHash(messageBytes)


/// extend Flurl to add API key and signature
type Flurl.Http.IFlurlRequest with
    member self.WithApi (api_path:string) public_key secret_key = 
        
        let nonce = DateTime.Now.Ticks
        let props = f"nonce=%i" nonce

        let base64DecodedSecret = Convert.FromBase64String secret_key
        let np = f"%i%c%s" nonce (Convert.ToChar(0)) props
        
        let pathBytes:byte[] = Encoding.UTF8.GetBytes(api_path)

        
        let hash256bytes = sha256_hash(np)
        //let z:byte[] = [||] //array.CreateInstance(typeof<byte>, 5) :> byte[]
        //pathBytes.CopyTo(z, 0)
        //hash256bytes.CopyTo(z, pathBytes.Length)

        let zz = Array.append pathBytes hash256bytes

        let signature = Convert.ToBase64String( getHash(base64DecodedSecret, zz) )

        self.WithHeader("API-Key", public_key).WithHeader( "API-Sign", signature)   
        


type String with
    member self.WithApi api_path public_key secret_key = FlurlRequest(Url(self)).WithApi api_path public_key secret_key