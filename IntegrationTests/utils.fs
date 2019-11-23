module utils
open FSharp.Configuration

type ApiKeys = YamlConfig<"api keys.secret.yaml", true, "", false>

let apiKeys = ApiKeys()

let public_key = apiKeys.``public key``
let secret_key = apiKeys.``secret key``


// swith to True to run tests that involves real money movements (market orders)
let runPaidTest = true
