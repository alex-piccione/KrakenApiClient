import json

print("start to create mapping")

try:
    ## Pairs (symbol) mapping
    pairs_mapping = {}
    with open("AssetPairs response.json", "r", encoding="utf-8-sig") as fr:  # utf-8-sig takes care of BOM
        result = json.load(fr, strict=False)["result"]
        
    for kraken_pair in result:
        pair_name = result[kraken_pair]["altname"]
        pairs_mapping[pair_name] = kraken_pair   

    # create F# file mapping
    with open("F# pairs mapping.fs", "w+") as fw:
        for c,k in pairs_mapping.items():
            fw.write(f'    ("{c}","{k}")\n')


    # Currencies mapping 
    currencies_mapping = {}
    with open("Assets response.json", "r", encoding="utf-8-sig") as fr:
        result = json.load(fr, strict=False)["result"]

    for kraken_currency in result:
        altname = result[kraken_currency]["altname"]
        currencies_mapping[kraken_currency] = altname

    with open("F# currencies mapping.fs", "w+") as fw:
        for k,c in currencies_mapping.items():
            fw.write(f'    ("{c}","{k}")\n')

    with open("F# Kraken currencies mapping.fs", "w+") as fw:
        for k,c in currencies_mapping.items():
            fw.write(f'    ("{k}","{c}")\n')


except Exception as exc:
    print("Failed. " + str(exc))
    exit(1)
 
print("mapping created")
exit(0)