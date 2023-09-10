using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaler : MonoBehaviour
{
    private int _screenHeight;
    private int _screenWidth;
    public float rate = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        _screenHeight = UnityEngine.Screen.height;
        GetComponent<UnityEngine.UI.CanvasScaler>().scaleFactor = rate * (_screenHeight / (float)512);
    }
}
