module assets_mapping

open System.Text.RegularExpressions
open asset

/// Get the known currency form the Kraken asset
let getCurrencyInfo (asset:string) =
    // Asset can be LUNA228.S where the 28.S means "Staking 28 days", but we want to map it to LUNA2.
    // Last 2 digits (optionals) followed by .S, .F, .T ...
    let m = Regex.Match(asset, @"(?:(\d{2})*)(?:\.)([A-Z])$")
    match m.Success with
    | false -> asset, AssetType.Normal
    | true -> 
        let asset = asset.Substring(0, asset.Length - m.Value.Length)
        let assetType =
            match m.Groups[2].Value with 
            | "S" -> AssetType.Stacking
            | "F" -> AssetType.Flexible
            | "B" -> AssetType.YieldBearing
            | "M" -> AssetType.OptInReward
            | "T" -> AssetType.Tokenized
            | "Hold" -> AssetType.Hold
            | _ -> AssetType.Other

        asset, assetType
