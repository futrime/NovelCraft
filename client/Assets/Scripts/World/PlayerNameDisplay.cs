using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerNameDisplay : MonoBehaviour
{
    private TMP_Text _nameText;
    private Camera _mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    public void SetName(string name)
    {
        _nameText = this.GetComponent<TMP_Text>();
        this._nameText.text = name;
    }
    private void Update()
    {
        _nameText.transform.LookAt(_mainCamera.transform);
    }
}
