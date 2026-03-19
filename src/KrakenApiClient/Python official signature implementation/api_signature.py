import urllib.parse
import hashlib
import hmac
import base64

# from: https://docs.kraken.com/api/docs/guides/spot-rest-auth/

def get_kraken_signature(urlpath, data, secret):

    if isinstance(data, str):
        encoded = (str(json.loads(data)["nonce"]) + data).encode()
    else:
        encoded = (str(data["nonce"]) + urllib.parse.urlencode(data)).encode()
    message = urlpath.encode() + hashlib.sha256(encoded).digest()

    #print(encoded)
    #encoded_bytes = hashlib.sha256(encoded).digest()  
    #print(' '.join(f'{b}' for b in encoded_bytes))

    #print(message)
    #print(' '.join(f'{b}' for b in message))
    #for v in message:
    #    print(v)

    mac = hmac.new(base64.b64decode(secret), message, hashlib.sha512)
    sigdigest = base64.b64encode(mac.digest())
    return sigdigest.decode()

api_sec = "kQH5HW/8p1uGOVjbgWA7FunAmGO8lsSUXNsu3eow76sz84Q18fWxnyRzBHCd3pd5nE9qa99HAZtuZuj6F1huXg=="

payload = {
        "nonce": "1616492376594", 
        "ordertype": "limit", 
        "pair": "XBTUSD",
        "price": 37500, 
        "type": "buy",
        "volume": 1.25
        }

signature = get_kraken_signature("/0/private/AddOrder", payload, api_sec)
print("API-Sign: {}".format(signature))
