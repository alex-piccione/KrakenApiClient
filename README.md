# Kraken API Client

A Simple .Net client for the **Kraken** API.  
Developed mainly for _XRP_.  
Target frameworks: _.NET Standard 2.0_, _.NET Core 3.1_, .net 6.0

[![NuGet](https://img.shields.io/nuget/v/Alex75.KrakenApiClient.svg)](https://www.nuget.org/packages/Alex75.KrakenApiClient) 
![Build Status](https://github.com/alex-piccione/KrakenApiClient/actions/workflows/dotnet.yml/badge.svg)

## Functionalities

**Public** | &nbsp; | &nbsp; 
---                 | ---                                              | ---
ListPairs           | List available currency pairs                    | 
Get Ticker          | Retrieve the Ticker of a specific currency pair. | The response is cached for a configurable amount of time.


**Private** | &nbsp; | &nbsp;
---                  | ---                                                                | ---
Get Balance          | Retrieve the owned and available amount <br/>of every currencies.  | 
Create Market Order  | Create an order at the current market price                        | 
Create Limit Order   | Create an order with a specified price                             | 
List open orders     | List all the open orders                                           | 
List closed orders   | List all the closed order                                          |
Withdrawal Crypto    | Withdrawal cryptocurrency to a registered wallet.                  | The _wallet name_ must be registered in advance


## How to use it

Add the <a href="https://www.nuget.org/packages/Alex75.KrakenApiClient" target="_blank">NuGet package</a>.  
See the examples in <a href="Example/Program.cs">Examples</a>


## For developers

Source code on GitHub.  
Deployment was before on MS DevOps but after the change of GitHub name it was impossible to restore a permission for the repository.  
Deployment on GitHub use a much cleaner script with only 1 "layer" of inermediate machine.  

### Kraken documentation

REST API documentation: https://www.kraken.com/features/api  
WebSocket API documentation: https://www.kraken.com/en-gb/features/websocket-api  
API support page: https://support.kraken.com/hc/en-us/categories/360000080686-API  

C# examples: https://bitbucket.org/arrivets/krakenapi/src/master/

  
  

[![HitCount](http://hits.dwyl.io/alex-piccione/alex-piccione/KrakenApiClient.svg)](http://hits.dwyl.io/alex-piccione/alex-piccione/KrakenApiClient)

