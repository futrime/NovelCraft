using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
public class GetStatePacket : Packet
{
    private Record.PlayState _playState;
    public Record.PlayState PlayState
    {
        get { return _playState; }
    }
    public override JObject GetPacket()
    {
        JObject jsonPacket = new JObject();

        jsonPacket["bound_to"] = "serverbound";
        jsonPacket["type"] = "get_state_request";

        return jsonPacket;
    }
    public override bool ParsePacket(JObject serverPacket)
    {
        // Check type
        JToken typeToken = serverPacket["type"];
        if (typeToken == null || typeToken.ToString() != "get_state_response")
            return false;

        JToken stateToken = serverPacket["state"];
        if (stateToken == null)
            return false;

        switch (stateToken.ToString())
        {
            case "waiting":
                this._playState = Record.PlayState.Prepare;
                break;
            case "playing":
                this._playState = Record.PlayState.Play;
                break;
            case "paused":
                this._playState = Record.PlayState.Pause;
                break;
            case "finished":
                this._playState = Record.PlayState.End;
                break;
            default:
                return false;
        }
        return true;
    }
}

