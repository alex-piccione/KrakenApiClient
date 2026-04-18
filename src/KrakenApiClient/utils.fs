module utils

open System
open System.Text
open System.Net.Http
open System.Collections.Generic
open Flurl
open Flurl.Http

let sha256 (data: byte[]) =
    Security.Cryptography.SHA256.Create().ComputeHash(data)

let sha512HMAC (data:byte[], messageBytes:byte[]) =
    (new Security.Cryptography.HMACSHA512(data)).ComputeHash(messageBytes)

let public invariantCulture = System.Globalization.CultureInfo.InvariantCulture

let private getNonce () = DateTime.UtcNow.Ticks.ToString()

// for LEGACY methods
let create_content (properties:IDictionary<string, string>) =
    let nonce = getNonce()

    let content = properties
                  |> Seq.map (fun kv -> sprintf "&%s=%s" kv.Key kv.Value)
                  |> Seq.fold (+) ("nonce=" + nonce)

    let nonce_content = nonce + content
    (nonce_content, content)

let internal createSignature (path:string) privateKey (data:KeyValuePair<string, string> seq) (nonce:string) =
    // cntent MUST be: nonce + "k=v&k=v&k=v..."
    let values = data |> Seq.map(fun kv -> $"{kv.Key}={kv.Value}")
    let content = nonce + String.Join('&', values)
    let encoded = Encoding.UTF8.GetBytes (content)

    let message = Array.append (Encoding.UTF8.GetBytes path) (sha256 encoded)

    let base64DecodedSecred = Convert.FromBase64String(privateKey)
    let signature = sha512HMAC(base64DecodedSecred, message)

    Convert.ToBase64String(signature)

/// extend HttpClient method

type HttpClient with

    /// Call GET adding the signature for API authentication
    member self.GetAsyncWithSignature path publicKey privateKey =
        let signature = createSignature path privateKey Map.empty (getNonce())
        let message = new HttpRequestMessage(HttpMethod.Get, path)
        message.SetHeader ("API-Key", publicKey)
        message.SetHeader ("API-Sign", signature)

        self.SendAsync message

    /// Call POST adding the signature for API authentication
    member self.PostAsyncWithSignature path publicKey privateKey (data:IDictionary<string, string> option) = // (data:IDictionary<string, string>) =
        let nonce = getNonce()

        let nonceKeyValue = Seq.singleton (KeyValuePair("nonce", nonce))

        // form data has to contain the nonce too
        let form =
            match data with
            | None -> nonceKeyValue
            | Some values ->  // Dictionary are immutable, need to convert to a sequence
                values
                |> Seq.cast<KeyValuePair<string, string>>
                |> Seq.append nonceKeyValue

        let signature = createSignature path privateKey form nonce
        let message = new HttpRequestMessage(HttpMethod.Post, path)

        
        message.Content <- new FormUrlEncodedContent(form)
        //message.SetHeader ("Content-Type", "application/x-www-form-urlencoded")
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