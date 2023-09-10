using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandshakePacket : Packet
{
    private int _version; // enum ??
    private string _token;
    public string Token
    {
        get { return _token; }
    }

    private int _uniqueId;
    public int UniqueId
    {
        get { return _uniqueId; }
    }
    public HandshakePacket(string token = null)
    {
        _version = 1;
        _token = token;
    }
    public override JObject GetPacket()
    {
        JObject jsonPacket = new JObject();

        jsonPacket["bound_to"] = "serverbound";
        jsonPacket["type"] = "handshake";
        jsonPacket["protocol_version"] = _version;
        jsonPacket["token"] = _token;

        return jsonPacket;
    }
    public override bool ParsePacket(JObject serverPacket)
    {
        // Check type
        JToken typeToken = serverPacket["type"];
        if (typeToken == null || typeToken.ToString() != "handshake") return false;

        JToken token = serverPacket["token"].ToString();
        if (token == null)
            return false;
        else
        {
            this._token = token.ToString();
        }

        JToken uniqueIdToken = serverPacket["unique_id"];
        if (uniqueIdToken == null)
            return false;
        else
        {
            this._uniqueId = int.Parse(UniqueId.ToString());
        }
        return true;
    }

}
