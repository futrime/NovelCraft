using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UploadLevelButton : MonoBehaviour
{
    private FileLoaded _fileLoaded;
    private Upload _upload = new() { };
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Clicked);
        this._fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();
    }
    private void Clicked()
    {
        // Get json file
        _fileLoaded.File = this._upload.UploadNcLevel();

        // Aborted
        //// Get the type by parsing the json file //
        //// Read the json file
        //JsonTextReader reader = JsonUtility.UnzipLevel(_fileLoaded.File.File);
        //// Process the file
        //JObject jsonObject = (JObject)JToken.ReadFrom(reader);

        //try
        //{
        //    if (jsonObject["type"].ToString() == "level_data")
        //    {
        //        _fileLoaded.Type = FileLoaded.FileType.Level;
        //    }
        //    else if (jsonObject["type"].ToString() == "record")
        //    {
        //        _fileLoaded.Type = FileLoaded.FileType.Record;
        //    }
        //    else
        //    {
        //        // Show the error
        //    }
        //}
        //catch
        //{
        //    // Show the error
        //}

        if (_fileLoaded.File != null && _fileLoaded.Type != FileLoaded.FileType.None)
        {
            // Switch to scene "Record"
            SceneManager.LoadScene("Record");
        }
        else
        {
            // Show the error
        }
    }


}
