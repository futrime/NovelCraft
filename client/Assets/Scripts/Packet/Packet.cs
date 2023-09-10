using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Bound
{
    Serverbound,
    Clinetbound
}
abstract public class Packet
{
    /// <summary>
    /// Get packet to be transmitted in the form of json
    /// </summary>
    public abstract JObject GetPacket();
    /// <summary>
    /// Parse the json packet sent from server
    /// </summary>
    /// <param name="serverPacket"></param>
    /// <returns>False if the packet is wrong</returns>
    public abstract bool ParsePacket(JObject serverPacket);
}
