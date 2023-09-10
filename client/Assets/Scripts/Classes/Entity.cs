using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity
{
    public enum Event
    {
        None,
        Spawn,
        Despawn
    }
    public int UniqueId;
    public int EntityId;
    public int DataValue;
    public Vector3 Position;
    public GameObject EntityObject;
    public float yaw;
    public float pitch;
    public InterpolateMovement InterpolateMove;
    public Renderer[] EntityRenderers;
}
