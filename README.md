# Kraken API Client

A Simple .Net client for the **Kraken** API.  
Target frameworks: net 10

[![NuGet](https://img.shields.io/nuget/v/Alex75.KrakenApiClient.svg)](https://www.nuget.org/packages/Alex75.KrakenApiClient) 
[![Deploy](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/deploy.yml/badge.svg)](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/deploy.yml)

This library uses common types defined in Alex75.Cryptocurrencies (Currecy, CurrencyPair, Ticker).


## Functionalities

See KrakenApiClient project [README](src/KrakenApiClient/README.md).

## How to use it

Add the <a href="https://www.nuget.org/packages/Alex75.KrakenApiClient" target="_blank">NuGet package</a>.  
See the examples in <a href="src/Examples/Program.cs">Examples</a>


### Kraken documentation

- REST API documentation: https://docs.kraken.com/api/docs/rest-api/get-account-balance
- API general introduction: https://docs.kraken.com/api/docs/guides/global-intro
- API authentication: https://docs.kraken.com/api/docs/guides/spot-rest-auth

### Kraken (crazy) assets

Kraken assets can be obtains by the REST API _/Assets_ endpoint.  
An asset can be like "SOL", "SOL.S" etc..., so it needs to be parsed in some way to get a "known" cryptocurrency symbol.


## TODO

- Clean currency_mapper (use the new CurrenciesMapper type and interface)
- Move fixed cache from Client.fs to Constants.fs
- can we move the  fetchPairs and fetchAssets functions from currency mapper to Client.fs ?
  (if not write a comment)
- Replace Flurl with HttpClient
