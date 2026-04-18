# Kraken API Client

A very simple .Net client for the **Kraken** exchange REST API.  

## Functionalities

Cached: The response is cached for a configurable amount of time.

| Function             | Cached | Description                                                    | Note | 
|----------------------|----|--------------------------------------------------------------------|------| 
| List Pairs           | ✔️ | List all the available currency pairs.                             |      |
| Get Ticker           | ✔️ | Retrieve the Ticker of a specific currency pair.                   |      |
| Get Balance          | ✔️ | Retrieve the owned and available amount of every currencies.       |      |
| Create Market Order  |    | Create an order at the current market price                        |      |
| Create Limit Order   |    | Create an order with a specified price                             |      |
| List open orders     |    | List all the open orders                                           |      |
| List closed orders   |    | List all the closed order                                          |      |
| Withdraw Crypto      |    | Withdraw cryptocurrency to a registered wallet.                    | The _wallet name_ must be registered in advance |


## How to use it

Add the <a href="https://www.nuget.org/packages/Alex75.KrakenApiClient" target="_blank">NuGet package</a>.  
See the examples in <a href="src/Examples/Program.cs">Examples</a>