module utils

open System
open System.Text
open Flurl
open Flurl.Http

let sha256 (data: byte[]) =
    Security.Cryptography.SHA256.Create().ComputeHash(data)

let sha512HMAC (data:byte[], messageBytes:byte[]) =
    (new Security.Cryptography.HMACSHA512(data)).ComputeHash(messageBytes)


let private getNonce () = DateTime.UtcNow.Ticks.ToString()

let internal createSignature (path:string) privateKey (data:Map<string, obj>) (nonce:string) =
    let content = data |> Map.fold (fun state k v -> state + $"&{k}={v}") ("nonce=" + nonce)
    let encoded = Encoding.UTF8.GetBytes (nonce + content)

    let message = Array.append (Encoding.UTF8.GetBytes path) (sha256 encoded)

    let base64DecodedSecred = Convert.FromBase64String(privateKey)
    let signature = sha512HMAC(base64DecodedSecred, message)

    content, Convert.ToBase64String(signature)



/// extend HttpClient method

type Net.Http.HttpClient with

    /// Call GET adding the signature for API authentication
    member self.GetAsyncWithSignature path publicKey privateKey =
        let _, signature = createSignature path privateKey Map.empty (getNonce())
        let message = new Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Get, path)
        message.SetHeader ("API-Key", publicKey)
        message.SetHeader ("API-Sign", signature)

        self.SendAsync message

    /// Call POST adding the signature for API authentication
    member self.PostAsyncWithSignature path publicKey privateKey =
        let nonce = getNonce()

        let data = Map.empty<string, obj> // [
            //"nonce", nonce
        //]

        let form = [
            System.Collections.Generic.KeyValuePair("nonce", nonce)
        ]

        let payload, signature = createSignature path privateKey data nonce
        let message = new Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, path)
        //let jsonBody = $"""{{"nonce":{nonce}}}"""
        //message.Content <- new Net.Http.StringContent(jsonBody, Encoding.UTF8, "application/json")
        //message.Content <- new Net.Http.StringContent(payload, Encoding.UTF8, "text/plain")
        message.Content <- new Net.Http.FormUrlEncodedContent(form)
        //message.SetHeader ("Content-Type", "application/json")
        message.SetHeader ("API-Key", publicKey)
        message.SetHeader ("API-Sign", signature)

        self.SendAsync message




/// extend Flurl to add API key and signature
type Flurl.Http.IFlurlRequest with
    member self.WithApi (api_path:string) (nonce_content:string) public_key secret_key =

        let base64DecodedSecred = Convert.FromBase64String(secret_key)

        let pathBytes = Encoding.UTF8.GetBytes api_path
        let hash256Bytes = sha256(Encoding.UTF8.GetBytes nonce_content)
        let z = Array.append pathBytes hash256Bytes

        let signature =  Convert.ToBase64String(sha512HMAC(base64DecodedSecred, z))

        self.WithHeader("API-Key", public_key).WithHeader("API-Sign", signature)

type String with
    member self.WithApi api_path nonce_content public_key secret_key = FlurlRequest(Url(self)).WithApi api_path nonce_content public_key secret_key