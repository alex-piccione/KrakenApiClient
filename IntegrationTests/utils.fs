module utils

open System
open System.IO
//open FSharp.Configuration // does not work with net core 3.0/3.1

// this pre-build command copy the private secret key file: copy "$(ProjectDir)api keys.secret.yaml" "$(TargetDir)"
let api_key_file = if File.Exists("api keys.secret.yaml") then "api keys.secret.yaml" else
                   if File.Exists("api keys.yaml") then "api keys.yaml" else
                   failwith "api key file not found"


let mutable public_key = ""
let mutable secret_key = ""
for line in File.ReadAllLines(api_key_file) do
    let values = line.Split(":")
    match values.[0] with
    | "public key" -> public_key <- values.[1].Trim().Replace("\"", "")
    | "secret key" -> secret_key <- values.[1].Trim().Replace("\"", "")
    | _ -> ()

