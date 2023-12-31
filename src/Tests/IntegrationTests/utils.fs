module utils

open CommunityToolkit.Diagnostics
open Microsoft.Extensions.Configuration
open Alex75.KrakenApiClient

//open FSharp.Configuration // does not work with net core 3.0/3.1

let configuration =
    ConfigurationBuilder()
        .AddUserSecrets("Alex75.KrakenApiClient-08ccac50-5aef-4bd5-b18a-707588558352") // secret file of main project
        .Build()

let publicKey = configuration["public key"]
let privateKey = configuration["private key"]

Guard.IsNotNullOrEmpty (publicKey, $@"configuration secret ""{nameof publicKey}""")
Guard.IsNotNullOrEmpty (privateKey, $@"configuration secret ""{nameof privateKey}""")

let client = Client(publicKey, privateKey) :> IClient

let getClient() = client
