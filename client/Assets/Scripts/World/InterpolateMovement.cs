using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// The Interpolate Movement of an entity
/// </summary>
public class InterpolateMovement : MonoBehaviour
{
    private Vector3? _targetPosition;
    /// <summary>
    /// The velocity of target position
    /// </summary>
    private Vector3? _targetVelocity;

    private float _recordSpeed;

    public const float DisplacementRate = 0.05f;

    float lastTime;
    /// <summary>
    /// If the distance between now position and target position is smaller than LerpMinDistance, stop lerping.
    /// </summary>
    //public const float LerpMinDistance = 0.1f;
    public const float InterpolationMinDotProduct = 0f;
    public void SetTargetPosition(Vector3 targetPosition, float recordSpeed)
    {
        this._recordSpeed = recordSpeed;
        if (this._targetPosition != null)
        {
            this.transform.position = this._targetPosition.Value;
            this._targetVelocity = (targetPosition - this._targetPosition) / Record.RecordInfo.FrameTime;
        }
        this._targetPosition = targetPosition;
    }
    private void Interpolation()
    {
        if (_targetPosition == null || this._targetVelocity == null)
        {
            return;
        }

        this.transform.position += this._targetVelocity.Value * this._recordSpeed * Time.deltaTime;

        if (Vector3.Dot(this._targetPosition.Value - this.transform.position, this._targetVelocity.Value) /
            (this._targetVelocity.Value.magnitude * (this.transform.position - this._targetPosition.Value).magnitude) < InterpolationMinDotProduct)
        {
            this._targetVelocity = new Vector3();
        }

        //if (((Vector3)this._targetVelocity).magnitude > InterpolationMinVelocity)
        //{
        //    Vector3 velocity = (Vector3)this._targetVelocity + ((Vector3)this._targetPosition - this.transform.position) * DisplacementRate;
        //    this.transform.Translate(velocity * Time.deltaTime);
        //}
        //else
        //{
        // this.transform.position = (Vector3)this._targetPosition;
        //}

        // Close The Interpolation Temporarily
    }
    void Update()
    {
        //if (Vector3.Distance(this.transform.position, this._targetPosition) > LerpMinDistance)
        Interpolation();
    }
}
