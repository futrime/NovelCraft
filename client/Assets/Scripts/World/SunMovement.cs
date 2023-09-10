using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunMovement : MonoBehaviour
{
    public const float AnglePerSecond = 360.0f / 1200.0f;
    public Light SunLight;

    // Start is called before the first frame update
    void Start()
    {
        SunLight = GetComponent<Light>();
        SunLight.transform.eulerAngles = new Vector3(0, 30, 0);
    }

    // Update is called once per frame
    void Update()
    {
        SunLight.transform.Rotate(Time.deltaTime * AnglePerSecond, 0, 0);
    }
}
