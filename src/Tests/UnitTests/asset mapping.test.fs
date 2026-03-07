module UnitTests.AssetMapping

open NUnit.Framework
open FsUnit
open asset

// Helper to convert string to DU for testing
let private parseAssetType = function
    | "Normal" -> AssetType.Normal
    | "Stacking" -> AssetType.Stacking
    | "Flexible" -> AssetType.Flexible
    | "YieldBearing" -> AssetType.YieldBearing
    | "OptInReward" -> AssetType.OptInReward
    | "Tokenized" -> AssetType.Tokenized
    | _ -> AssetType.Other

[<TestCase("LUNA2", "LUNA2", "Normal")>]
[<TestCase("LUNA2.S", "LUNA2", "Stacking")>]
[<TestCase("LUNA213.S", "LUNA2", "Stacking")>]
[<TestCase("SOL28.S", "SOL", "Stacking")>]
[<TestCase("SOL.F", "SOL", "Flexible")>]
let ``getCurrencyInfo returns the `right currency and type`` (asset:string, expectedCurrency:string, expectedTypeString: string) =
    assets_mapping.getCurrencyInfo asset
    |> should equal (expectedCurrency, parseAssetType expectedTypeString)