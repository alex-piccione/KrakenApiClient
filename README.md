# Kraken API Client

A Simple .Net client for the **Kraken** API.  
Target frameworks: net 10

[![NuGet](https://img.shields.io/nuget/v/Alex75.KrakenApiClient.svg)](https://www.nuget.org/packages/Alex75.KrakenApiClient) 
![Build Status](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/deploy.yml/badge.svg)

[![.Net 10](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/deploy_net-10.yml/badge.svg?branch=net-10)](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/deploy_net-10.yml)

This library uses common types defined in Alex75.Cryptocurrencies, like Currecy, CurrencyPir, Ticker...


## Functionalities

Cached: The response is cached for a configurable amount of time.

| Function             | Cached | Description                                                    | Note | 
|----------------------|----|--------------------------------------------------------------------|------| 
| List Pairs           | ✔️ | List all the available currency pairs.                             |      |
| Get Ticker           | ✔️ | Retrieve the Ticker of a specific currency pair.                   |      |
| Get Balance          | ✔️ | Retrieve the owned and available amount <br/>of every currencies.  |      |
| Create Market Order  |    | Create an order at the current market price                        |      |
| Create Limit Order   |    | Create an order with a specified price                             |      |
| List open orders     |    | List all the open orders                                           |      |
| List closed orders   |    | List all the closed order                                          |      |
| Withdraw Crypto      |    | Withdraw cryptocurrency to a registered wallet.                    | The _wallet name_ must be registered in advance |

## Functionalities

Cached: The response is cached for a configurable amount of time.
| Function               | Cached | Description                                                    | Note                                                                                     |
|------------------------|--------|----------------------------------------------------------------|--------------------------------------------------------------------------|
| List Pairs             | ✔️     | List all the available currency pairs.                         |                                                                                          |
| Get Ticker             | ✔️     | Retrieve the Ticker of a specific currency pair.               |                                                                                          |
| Get Balance            |        | Retrieve the owned and available amount of every currency.     |                                                                                          |
| Create Market Order    |        | Create an order at the current market price.                    |                                                                                          |
| Create Limit Order     |        | Create an order with a specified price.                        |                                                                                          |
| List Open Orders       |        | List all the open orders.                                      |                                                                                          |
| List Closed Orders     |        | List all the closed orders.                                    |                                                                                          |
| Withdraw Crypto        |        | Withdraw cryptocurrency to a registered wallet.                | The **wallet name** must be registered in advance on Kraken.              |


## How to use it

Add the <a href="https://www.nuget.org/packages/Alex75.KrakenApiClient" target="_blank">NuGet package</a>.  
See the examples in <a href="src/Examples/Program.cs">Examples</a>


## For developers

Source code on GitHub.  
Deployment was before on MS DevOps but after the change of GitHub name it was impossible to restore a permission for the repository.  
Deployment on GitHub use a much cleaner script with only 1 "layer" of inermediate machine.  

### Kraken documentation

- REST API documentation: https://docs.kraken.com/api/docs/rest-api/get-account-balance
- APIs general introduction: https://docs.kraken.com/api/docs/guides/global-intro

### Kraken (crazy) assets

Kraken assets can be obtains by the REST API _/Assets_ endpoint.  
An asset can be like "SOL", "SOL.S" etc..., so it need to be parsed in some way to get a official cryptocurrency symbol.


## TODO

- Gitguardian: ignore keys
- GitGuardian: amend history to put ignore 
let privateKey = "kQH5HW/8p1uGOVjbgWA7FunAmGO8lsSUXNsu3eow76sz84Q18fWxnyRzBHCd3pd5nE9qa99HAZtuZuj6F1huXg=="

- Clean currency_mapper (use hte new CurrenciesMapper type and interface)
- Move fixed cache from Client.fs to Constants.fs
- can we move the  fetchPairs and fetchAssets functions from currency mapper to Client.fs ?
  (if not write a comment)
