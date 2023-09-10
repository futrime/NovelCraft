using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRotation : MonoBehaviour
{
    public float RotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        RotateSpeed = 90;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotation
        this.transform.Rotate(new Vector3(0, RotateSpeed * Time.deltaTime, 0));
    }
}
