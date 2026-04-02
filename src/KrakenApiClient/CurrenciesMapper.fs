module CurrenciesMapper

// TODO: replace "currency mapper.fs"

(*
Since Kraken use weird currency symbols we need a mapper that translate known currency to Kraken currencies and the other way.
*)

type ICurrenciesMspper =
    abstract member GetKnownCurrency: string -> string
    abstract member GetKrakenCurrency: string -> string

type CurrenciesMapper () =

    interface ICurrenciesMspper with

        member this.GetKnownCurrency (knownCurrency:string) = ""
        member this.GetKrakenCurrency (krakenCurrency:string) = ""