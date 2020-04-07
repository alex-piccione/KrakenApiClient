# Kraken API Client

A Simple .Net client for the **Kraken** API.  
Developed mainly for _XRP_.  
Target frameworks: _.NET Standard 2.0_ & _.NET Core 3.1_

[![NuGet](https://img.shields.io/nuget/v/Alex75.KrakenApiClient.svg)](https://www.nuget.org/packages/Alex75.KrakenApiClient) 
[![Build Status](https://alex75.visualstudio.com/Kraken%20API%20Client/_apis/build/status/Build%20and%20publish%20Package%20v0.1?branchName=master)](https://alex75.visualstudio.com/Kraken%20API%20Client/_build/latest?definitionId=18&branchName=master)


## Public functions
&nbsp; | &nbsp; | &nbsp; 
---                 | ---                                              | ---
ListPairs           | List available currency pairs                    | 
Get Ticker          | Retrieve the Ticker of a specific currency pair. | The response is cached for a configurable amount of time.



## Private functions 
An API key is required.  

&nbsp; | &nbsp; | &nbsp;
---                 | ---                                               | ---
Get Balance         | Retrieve the owned and available amount of every currencies. | 
Create Market Order | Create an order at the current market price       | 
List open orders    | List all the open orders                          | 
List closed orders  | List all the closed order                         |
Withdrawal Crypto   | Withdrawal cryptocurrency to a registered wallet. | The _wallet name_ must be registered in your Kraken account in advance.


<dl style="display:none">
  <dt>ListPairs</dt>
    <dd>List available currency pairs</dd>
  <dt>Get Ticker</dt>
    <dd>Retrieve the Ticker of a specific currency pair.
    <br>The response is cached for a configurable amount of time.</dd>
  <dt>Get Balance <img src="./api key lock.svg" height=12 title="API key required"></dt>
    <dd>Retrieve the owned and available amount for the specified currencies.
    <br>The response is cached for a configurable amount of time.</dd>
  <dt>Create Market Order <img src="./api key lock.svg" height=12 title="API key required"></dt>
    <dd>Create an order at the current market price</dd>
  <dt>List open orders <img src="./api key lock.svg" height=12 title="API key required"></dt>
    <dd>List all the open orders</dd>
  <dt>List closed orders <img src="./api key lock.svg" height=12 title="API key required"></dt>
    <dd>List all the closed orders</dd>
  <dt>Withdrawal Crypto <img src="./api key lock.svg" height=12 title="API key required"></dt>
    <dd>Withdrawal cryptocurrency to a registered wallet. The wallet name must be registered in your Kraken account</dd>
</dl>


## How to use it

Add the <a href="https://www.nuget.org/packages/Alex75.KrakenApiClient" target="_blank">NuGet package</a>.  
See the examples in <a href="Example/Program.cs">Examples</a>


## For developers

Source code on GitHub.

### Kraken documentation

REST API documentation: https://www.kraken.com/features/api  
WebSocket API documentation: https://www.kraken.com/en-gb/features/websocket-api  
API support page: https://support.kraken.com/hc/en-us/categories/360000080686-API  

C# examples: https://bitbucket.org/arrivets/krakenapi/src/master/

### Get account balance

### Get trade balance

### Known issues 

None.


<span style="float:right">
[![HitCount](http://hits.dwyl.io/alex75it/alex75it/KrakenApiClient.svg)](http://hits.dwyl.io/alex75it/alex75it/KrakenApiClient)
</span>
