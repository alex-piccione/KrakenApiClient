﻿module utils

open System
open System.Text
open Alex75.Cryptocurrencies
open Flurl
open Flurl.Http

let f = sprintf

let format_pair (main:Currency) (other:Currency) = f"%s%s" main.UpperCase other.UpperCase

//let get_kraken_pair (pair:CurrencyPair) =
//    let kraken_main = currency_mapping.get_kraken_currency pair.Main.UpperCase
//    let kraken_other = currency_mapping.get_kraken_currency pair.Other.UpperCase
//    CurrencyPair(kraken_main, kraken_other).AAABBB

(*
API calls that require currency assets can be referenced using their ISO4217-A3 names in the case of ISO registered names,
their 3 letter commonly used names in the case of unregistered names,
or their X-ISO4217-A3 code (see http://www.ifex-project.org/).
*)

let sha256_hash (value:string) =
    System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value))

let getHash (keyBytes:byte[], messageBytes:byte[]) =
    (*let hasher = System.Security.Cryptography.HMACSHA512.Create()
    hasher.Key <- keyBytes
    hasher.ComputeHash(messageBytes)*)
    (new System.Security.Cryptography.HMACSHA512(keyBytes)).ComputeHash(messageBytes)

/// extend Flurl to add API key and signature
type Flurl.Http.IFlurlRequest with
    member self.WithApi (api_path:string) nonce_content public_key secret_key =

        let base64DecodedSecred = Convert.FromBase64String(secret_key)

        let pathBytes = Encoding.UTF8.GetBytes(api_path)
        let hash256Bytes = sha256_hash(nonce_content)
        let z = Array.append pathBytes hash256Bytes

        let signature =  Convert.ToBase64String(getHash(base64DecodedSecred, z))

        self.WithHeader("API-Key", public_key).WithHeader("API-Sign", signature)

type String with
    member self.WithApi api_path nonce_content public_key secret_key = FlurlRequest(Url(self)).WithApi api_path nonce_content public_key secret_key