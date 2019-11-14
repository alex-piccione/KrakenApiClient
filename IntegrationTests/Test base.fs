module Test_base

open FSharp.Configuration

type ApiKeys = YamlConfig<"api keys.secret.yaml", true, "", false>

