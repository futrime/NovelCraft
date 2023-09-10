using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class FPSDisplay : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    private TMP_Text _FPSText;
    private TMP_Text _positionText;
    private float _fpsByDeltatime = 0.2f;
    private float _passedTime = 0.0f;
    private int _frameCount = 0;
    private float _realtimeFPS = 0.0f;
    private void Start()
    {
        this._FPSText = (GameObject.Find("ObserverCanvas/FPS") ?? GameObject.Find("Canvas/FPS")).GetComponent<TMP_Text>();

        this._positionText = (GameObject.Find("ObserverCanvas/Position") ?? GameObject.Find("Canvas/Position")).GetComponent<TMP_Text>();
        this._camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void Update()
    {
        GetFPS();
        UpdatePosition();
    }
    private void SetFPS()
    {
        //如果QualitySettings.vSyncCount属性设置，这个值将被忽略。
        //设置应用平台目标帧率为 60
        //Application.targetFrameRate = 60;
    }
    private void GetFPS()
    {
        if (_FPSText == null) return;

        _frameCount++;
        _passedTime += Time.deltaTime;
        if (_passedTime >= _fpsByDeltatime)
        {
            _realtimeFPS = _frameCount / _passedTime;
            _FPSText.text = $"FPS: {_realtimeFPS:f1}";
            _passedTime = 0.0f;
            _frameCount = 0;
        }
    }
    private void UpdatePosition()
    {
        _positionText.text = $"Position: ({(int)(this._camera.transform.position.x)},{(int)this._camera.transform.position.y},{(int)this._camera.transform.position.z})";
    }

}
