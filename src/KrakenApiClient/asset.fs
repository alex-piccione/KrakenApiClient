module asset

type AssetType = Normal | Stacking | Flexible | YieldBearing | OptInReward | Tokenized | Hold | Other

type Asset = { Name:string; AltName:string; (*; Decimals:int; DisplayDecimals:int; Status:string; MarginRate:decimal; AssetType:AssetType *) }