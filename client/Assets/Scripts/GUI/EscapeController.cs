using NovelCraft.Sdk;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EscapeController : MonoBehaviour
{
    private GameObject _escapePanelGameObject;
    private Button _continueGameButton;
    private Button _backToMenuButton;

    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }
    // Start is called before the first frame update
    void Start()
    {
        this._escapePanelGameObject = GameObject.Find("Canvas/Escape") ?? GameObject.Find("ObserverCanvas/Escape");
        this._continueGameButton = (GameObject.Find("Canvas/Escape/ContinueGameButton") ?? GameObject.Find("ObserverCanvas/Escape/ContinueGameButton")).GetComponent<Button>();
        this._backToMenuButton = (GameObject.Find("Canvas/Escape/BackToMenuButton") ?? GameObject.Find("ObserverCanvas/Escape/BackToMenuButton")).GetComponent<Button>();

        this._continueGameButton.onClick.AddListener(() =>
        {
            this._escapePanelGameObject.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        });

        this._backToMenuButton.onClick.AddListener(() =>
        {
            // 分类
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Play")
            {
                GameObject.Find("EventHandler").GetComponent<InputHandler>().QuitClient();
            }
            else if (sceneName == "Record")
            {

            }
            // Clear HP
            foreach (var obj in GetDontDestroyOnLoadGameObjects())
            {
                //Debug.Log(obj);
                Destroy(obj);
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            SceneManager.LoadScene("Menu");
        });

        this._escapePanelGameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this._escapePanelGameObject.SetActive(!this._escapePanelGameObject.activeSelf);

            if (this._escapePanelGameObject.activeSelf)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = false;
            }
        }
    }
}
