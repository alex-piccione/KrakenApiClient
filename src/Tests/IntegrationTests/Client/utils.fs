module utils

open System.Reflection
open Microsoft.Extensions.Configuration
open Alex75.KrakenApiClient

let configuration =
    ConfigurationBuilder()
        .AddUserSecrets(Assembly.GetExecutingAssembly()) // assembly is required to give the runtime the correct one where to find the UserSecretsId
        .Build()

let private getValue (key: string) =
    match configuration[key] with
    | null -> failwithf @"configuration value ""%s"" is missing" key
    | value -> value

let client = Client(getValue "public key", getValue "private key") :> IClient