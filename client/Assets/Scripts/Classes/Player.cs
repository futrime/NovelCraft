using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity
{
    public enum InstantAction
    {
        Jump,
        AttackClick,
        AttackStart,
        AttackEnd,
        UseClick,
        UseStart,
        UseEnd
    }
    public const float PlayerHurtChangeColorTime = 0.5f;
    public const int PlayerMaxHealth = 20;
    public float Health;
    public int Experiments;
    public Inventory Inventory = new();
    public int MainHandSlot;
    public int Id = 0;

    public GameObject Head;
    public GameObject LeftArms;
    public GameObject RightArms;
    public GameObject LeftLegs;
    public GameObject RightLegs;

    public PlayerAnimations PlayerAnimations;

    public Player(int uniqueId, Vector3 position, float yaw = 0, float pitch = 0)
    {
        this.UniqueId = uniqueId;
        this.EntityId = 0;
        this.Position = position;
        this.yaw = yaw;
        this.pitch = pitch;
    }
    public Player()
    {
        this.EntityId = 0;
        this.yaw = 0;
        this.pitch = 0;
    }
    public void UpdatePosition(Vector3 newPosition, float recordSpeed)
    {
        PlayerAnimations.WalkAnimationPlayer(this.Position, newPosition);
        // To be changed
        this.Position = newPosition;
        if (this.EntityObject != null)
        {
            if (this.InterpolateMove != null)
            {
                // Interpolation movement
                this.InterpolateMove.SetTargetPosition(newPosition, recordSpeed);
            }
            else
            {
                this.InterpolateMove = this.EntityObject.GetComponent<InterpolateMovement>();
                // No Interpolation movement
                this.EntityObject.transform.position = newPosition;
            }
        }
    }
    public void UpdateBodyGameObject()
    {
        if (this.EntityObject == null) return;

        // Find the head, arms, legs
        this.Head = this.EntityObject.transform.Find("Head").gameObject;
        this.LeftArms = this.EntityObject.transform.Find("LeftArms").gameObject;
        this.RightArms = this.EntityObject.transform.Find("RightArms").gameObject;
        this.LeftLegs = this.EntityObject.transform.Find("LeftLegs").gameObject;
        this.RightLegs = this.EntityObject.transform.Find("RightLegs").gameObject;
    }
    public void UpdateOrientation(float pitch, float yaw)
    {
        this.pitch = pitch;
        this.yaw = yaw;

        if (this.EntityObject != null)
        {
            this.EntityObject.transform.eulerAngles = new Vector3(0, yaw, 0);
        }
        if (this.Head != null)
        {
            this.Head.transform.localEulerAngles = new Vector3(pitch, 0, 0);
        }
    }
    private void SetEntityMaterialRed()
    {
        foreach (var entityRenderer in this.EntityRenderers)
            if (entityRenderer != null)
                entityRenderer.material.color = Color.red;
    }
    private void SetMaterialWhite()
    {
        foreach (var entityRenderer in this.EntityRenderers)
            if (entityRenderer != null)
                entityRenderer.material.color = Color.white;
    }
    public IEnumerator PlayerHurt(float recordSpeed)
    {
        this.SetEntityMaterialRed();
        yield return new WaitForSeconds(PlayerHurtChangeColorTime / recordSpeed);
        this.SetMaterialWhite();
    }
}
