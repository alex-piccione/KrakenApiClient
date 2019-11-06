import json

print("start to create mapping")

try:

    mapping = {}

    with open("GET pairs response.json", "r") as fr:
        j = json.load(fr, strict=False)

    result = j["result"]
    for kraken_pair in result:
        pairJ = result[kraken_pair]
        pair_name = pairJ["altname"]
        #main = pairJ["base"]
        #other = pairJ["quote"]
        mapping[pair_name] = kraken_pair   

    # create F# file mapping
    with open("F# mapping.fs", "w+") as fw:
        for k,v in mapping.items():
            fw.write(f'    ("{k}","{v}")\n')

except Exception as exc:
    print("Failed." + str(exc))

 
print("mapping created")
