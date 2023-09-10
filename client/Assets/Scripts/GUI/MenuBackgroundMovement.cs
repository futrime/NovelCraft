using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackgroundMovement : MonoBehaviour
{
    private RectTransform _rectTransform;
    private float _time;
    public const float Scale = 0.25f;
    private float _scaleTime;
    public const float RandomWalkRate = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _time = 0;
        _scaleTime = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 adder = new(
            Scale * (1 - Mathf.Cos(2 * Mathf.PI / _scaleTime * this._time)),
            Scale * (1 - Mathf.Cos(2 * Mathf.PI / _scaleTime * this._time)),
            Scale * (1 - Mathf.Cos(2 * Mathf.PI / _scaleTime * this._time))
        );
        this._rectTransform.localScale = Vector3.one + adder;
        this._rectTransform.position += Random.insideUnitSphere * RandomWalkRate;
        _time += Time.deltaTime;
    }
}
