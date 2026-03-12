module utils

open Microsoft.Extensions.Configuration
open Alex75.KrakenApiClient

let configuration =
    ConfigurationBuilder()
        .AddUserSecrets("Alex75.KrakenApiClient-08ccac50-5aef-4bd5-b18a-707588558352") // same of main project
        .Build()

let private getValue (key: string) =
    match configuration[key] with
    | null -> failwithf @"configuration value ""%s"" is missing" key
    | value -> value

let client = Client(getValue "public key", getValue "private key") :> IClient

let getClient() = client
