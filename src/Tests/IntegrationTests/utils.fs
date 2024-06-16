module utils

open System
open Microsoft.Extensions.Configuration
open Alex75.KrakenApiClient

//open FSharp.Configuration // does not work with net core 3.0/3.1

let configuration =
    ConfigurationBuilder()
        .AddUserSecrets("Alex75.KrakenApiClient-08ccac50-5aef-4bd5-b18a-707588558352")
        .Build()

let publicKey = configuration.["public key"]
let secretKey = configuration.["private key"]

let client = Client(publicKey, secretKey) :> IClient
