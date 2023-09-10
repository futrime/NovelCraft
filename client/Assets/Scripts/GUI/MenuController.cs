using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// The path of the total project
    /// </summary>
    private string _projectPath;

    /// <summary>
    /// Button sound controller
    /// </summary>
    private AudioSource _buttonSound;


    private Button _startGameButton;
    private GameObject _title;
    /// <summary>
    /// Load file objects
    /// </summary>
    private Upload _upload = new();
    private FileLoaded _fileLoaded;
    /// <summary>
    /// Switch button
    /// </summary>
    private Button _recordSwitchButton;
    private Button _playSwitchButton;

    /// <summary>
    /// The content in the scroll view
    /// </summary>
    private GameObject _scrollViewContent;
    private GameObject _scrollView;

    /// <summary>
    /// The address of all nclevels
    /// </summary>
    private string[] _nclevels;

    /// <summary>
    /// The button that is displayed in the 'record' column
    /// </summary>
    [SerializeField]
    private GameObject _recordButtonPrefab;
    private List<Button> _recordButtons = new();

    /// <summary>
    /// Close button
    /// </summary>
    private Button _closeButton;

    /// <summary>
    /// The parameters of connection with the server
    /// </summary>
    private bool _haveServer = false;
    private string _server;
    private string _token = "Steve";
    private string _host = "localhost";
    private int _port = 14514;

    /// <summary>
    /// Add external server
    /// </summary>
    private GameObject _addExternalServerWindow;
    private TMP_InputField _addExternalServerIPText;
    private TMP_InputField _addExternalServerPortText;
    private Button _addExternalServerConfirmButton;
    private Button _addExternalServerCancelButton;
    private List<(string IP, int port)> _addedExternalServers = new();
    private string _addedserversJsonPath;
    private JToken _addedserversJson;

    /// <summary>
    /// Help
    /// </summary>
    private Button _helpButton;
    private GameObject _helpDocumentWindow;
    private Button _closeHelpButton;

    /// <summary>
    /// Quit game
    /// </summary>
    private Button _quitButton;

    // Start is called before the first frame update
    void Start()
    {
        _projectPath = Directory.GetCurrentDirectory();
        this._fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();


        this._helpButton = GameObject.Find("Canvas/Help").GetComponent<Button>();
        this._helpButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            _helpDocumentWindow.SetActive(true);
        });
        _helpDocumentWindow = GameObject.Find("Canvas/HelpDocument");
        this._closeHelpButton = GameObject.Find("Canvas/HelpDocument/CloseButton").GetComponent<Button>();
        this._closeHelpButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            _helpDocumentWindow.SetActive(false);
        });
        _helpDocumentWindow.SetActive(false);

        this._quitButton = GameObject.Find("Canvas/Quit").GetComponent<Button>();
        this._quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        // Check the local server
        try
        {
            DirectoryInfo parentPath = Directory.GetParent(this._projectPath);
            string[] servers = Directory.GetFiles(parentPath is null ? parentPath.FullName : this._projectPath, "Server.exe", SearchOption.AllDirectories);
            if (servers is not null && servers.Length > 0)
            {
                this._server = servers[0];
                this._haveServer = true;
                // local host
                _fileLoaded.HaveServer = true;
                _fileLoaded.ServerHost = _host;
                _fileLoaded.Token = _token;
                _fileLoaded.ServerPort = _port;
            }
        }
        catch (Exception e)
        {

        }

        _buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();
        _buttonSound.clip ??= Resources.Load<AudioClip>("Music/Sound/Button/ButtonClick");

        _scrollView = GameObject.Find("Canvas/Scroll View");
        _scrollViewContent = GameObject.Find("Canvas/Scroll View/Viewport/Content");
        _recordButtonPrefab ??= Resources.Load<GameObject>("GUI/Buttton/NclevelButton");

        // Add server buttons
        this._addExternalServerWindow = GameObject.Find("Canvas/AddServer");
        _addExternalServerConfirmButton = GameObject.Find("Canvas/AddServer/ConfirmButton").GetComponent<Button>();
        _addExternalServerCancelButton = GameObject.Find("Canvas/AddServer/CancelButton").GetComponent<Button>();
        _addExternalServerIPText = GameObject.Find("Canvas/AddServer/Input/IP").GetComponent<TMP_InputField>();
        _addExternalServerPortText = GameObject.Find("Canvas/AddServer/Input/Port").GetComponent<TMP_InputField>();

        this._addExternalServerWindow.SetActive(false);

        // Read the servers json
        string[] serverJsonPaths = Directory.GetFiles(this._projectPath, "external_servers.json", SearchOption.AllDirectories);

        if (serverJsonPaths.Length == 0)
        {
            // Create a new json file
            var fs = File.Create(Path.Join(this._projectPath, "external_servers.json"));
            fs.Write(System.Text.Encoding.UTF8.GetBytes("[]"));
            fs.Close();
            serverJsonPaths.Append(Path.Join(this._projectPath, "external_servers.json"));
        }

        _addedserversJsonPath = serverJsonPaths[0];
        System.IO.StreamReader file = System.IO.File.OpenText(_addedserversJsonPath);

        this._addedserversJson = JToken.ReadFrom(new JsonTextReader(file));
        foreach (JToken serverInfo in (JArray)_addedserversJson)
        {
            this._addedExternalServers.Add(new((string)serverInfo["IP"], (int)serverInfo["port"]));
        }
        file.Close();

        // Define the event of start button
        _title = GameObject.Find("Canvas/Title");
        _startGameButton = GameObject.Find("Canvas/StartGameButton").GetComponent<Button>();
        _startGameButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            // Hide the title and start game button self.
            _title.SetActive(false);
            _startGameButton.gameObject.SetActive(false);
            _quitButton.gameObject.SetActive(false);
            _helpButton.gameObject.SetActive(false);
            // Set the _scrollView active
            this._scrollView.SetActive(true);
            // In default, list all the nclevels.
            ListAllNclevels();
        });

        // Define the event of close button
        this._closeButton = GameObject.Find("Canvas/Scroll View/CloseButton").GetComponent<Button>();
        _closeButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            // Open the title and start game button self.
            _title.SetActive(true);
            _startGameButton.gameObject.SetActive(true);
            _quitButton.gameObject.SetActive(true);
            _helpButton.gameObject.SetActive(true);
            // Hide the _scrollView
            this._scrollView.SetActive(false);
        });

        void ClearChildrenInContent()
        {
            this._recordButtons.Clear();

            // Clear the content of scroll bar //
            Transform scrollViewContentTransform = _scrollViewContent.transform;
            // Add all the children into the list
            List<GameObject> scrollViewContentChildren = new List<GameObject>();
            for (int i = 0; i < scrollViewContentTransform.childCount; i++)
            {
                scrollViewContentChildren.Add(scrollViewContentTransform.GetChild(i).gameObject);
            }
            // Destroy all the children
            foreach (GameObject child in scrollViewContentChildren)
            {
                Destroy(child);
            }
        }

        void ListAllNclevels(bool startServer = false, bool isRecord = true)
        {
            // Prior: find folders
            List<string> nclevelFolders = Directory.GetDirectories($"{this._projectPath}/worlds").ToList();
            // Next: find files
            string[] allNclevels = Directory.GetFiles($"{this._projectPath}/worlds", "*.nclevel", SearchOption.AllDirectories);
            // Compare them
            foreach (string file in allNclevels)
            {
                bool haveFolder = false;
                foreach (string folder in nclevelFolders)
                {
                    if (folder == file) { haveFolder = true; break; }
                }
                if (!haveFolder)
                {
                    nclevelFolders.Add(file);
                }
            }
            this._nclevels = nclevelFolders.ToArray();

            foreach (string fileName in _nclevels)
            {
                Debug.Log(fileName);
                // Create record button objects 
                GameObject newRecordButtonObject = Instantiate(this._recordButtonPrefab);
                Button newRecordButton = newRecordButtonObject.GetComponent<Button>();
                TMP_Text recordText = newRecordButtonObject.GetComponentInChildren<TMP_Text>();
                // Get nclevel name
                int index = Math.Max(fileName.LastIndexOf('/'), fileName.LastIndexOf('\\'));
                string name = fileName[(index + 1)..];
                recordText.text = $" {name}";

                // Bind the event onto the button
                newRecordButton.onClick.AddListener(() =>
                {
                    // Start local Server
                    if (startServer == true)
                    {
                        // Write the config.json before starting the server  
                        // string mapName = Regex.Replace(newRecordButton.GetComponentInChildren<TMP_Text>().text, "[\r\n\t ]", string.Empty, RegexOptions.Compiled); ;
                        // Read config.json
                        string configJsonPath = $"{Directory.GetParent(this._server)}/config.json";
                        System.IO.StreamReader file = System.IO.File.OpenText(configJsonPath);

                        var configJson = JToken.ReadFrom(new JsonTextReader(file));
                        file.Close();
                        configJson["level_name"] = name;
                        File.WriteAllText(configJsonPath, configJson.ToString(), System.Text.Encoding.UTF8);


                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        process.StartInfo.FileName = this._server;
                        //process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

                        // Add when release
                        //process.StartInfo.CreateNoWindow = true;

                        //process.StartInfo.UseShellExecute = false;
                        //process.StartInfo.RedirectStandardInput = true;
                        process.StartInfo.WorkingDirectory = Path.GetDirectoryName(this._server);
                        process.Start();
                        _fileLoaded.ServerProcess = process;
                    }

                    // Play sound
                    _buttonSound.Play();
                    // Add the target file to _fileLoaded object
                    Upload.OpenFileName selectedFile = new Upload.OpenFileName();
                    selectedFile.File = fileName;
                    _fileLoaded.File = selectedFile;

                    if (isRecord)
                        // Switch to scene "Record"
                        SceneManager.LoadScene("Record");
                    else
                        // Switch to scene "Play"
                        SceneManager.LoadScene("Play");

                });

                // Put the button into the content
                newRecordButtonObject.transform.SetParent(this._scrollViewContent.transform);
                newRecordButtonObject.transform.localScale = Vector3.one;
                this._recordButtons.Add(newRecordButton);
            }
        }


        // Define the event of record switch button
        _recordSwitchButton = GameObject.Find("Canvas/Scroll View/RecordSwitchButton").GetComponent<Button>();
        _recordSwitchButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            // Clear the content of scroll bar
            ClearChildrenInContent();
            // Find and display the nclevels in the path
            ListAllNclevels();
        });


        // Define the event of play switch button
        void UpdateServerList()
        {
            // Clear the content of scroll bar
            ClearChildrenInContent();

            // Add external server choice
            GameObject AddExternalServerButtonObject = Instantiate(this._recordButtonPrefab);
            Button AddExternalServerButton = AddExternalServerButtonObject.GetComponent<Button>();
            TMP_Text AddExternalServerText = AddExternalServerButtonObject.GetComponentInChildren<TMP_Text>();
            AddExternalServerText.text = $" +Add external server";

            // Bind the event onto the button
            AddExternalServerButton.onClick.AddListener(() =>
            {
                // Play sound
                _buttonSound.Play();
                // Make the window visible
                this._addExternalServerWindow.SetActive(true);
                this._scrollView.SetActive(false);

                // Clear the input text

                this._addExternalServerPortText.text = this._addExternalServerIPText.text = "";
            });

            // Put the button into the content
            AddExternalServerButtonObject.transform.SetParent(this._scrollViewContent.transform);
            AddExternalServerButtonObject.transform.localScale = Vector3.one;

            if (this._haveServer)
            {
                // Create record local server button objects if the sever.exe exists
                GameObject localServerButtonObject = Instantiate(this._recordButtonPrefab);
                Button localServerButton = localServerButtonObject.GetComponent<Button>();
                TMP_Text localServerText = localServerButtonObject.GetComponentInChildren<TMP_Text>();
                localServerText.text = $" Local server";

                // Bind the event onto the button
                localServerButton.onClick.AddListener(() =>
                {
                    // Play sound
                    _buttonSound.Play();
                    // Choose map
                    ClearChildrenInContent();
                    ListAllNclevels(startServer: true, isRecord: false);
                });

                // Put the button into the content
                localServerButtonObject.transform.SetParent(this._scrollViewContent.transform);
                localServerButtonObject.transform.localScale = Vector3.one;
            }

            // Add external server
            foreach (var externalServer in this._addedExternalServers)
            {
                GameObject externalServerButtonObject = Instantiate(this._recordButtonPrefab);
                Button externalServerButton = externalServerButtonObject.GetComponent<Button>();
                TMP_Text externalServerText = externalServerButtonObject.GetComponentInChildren<TMP_Text>();
                externalServerText.text = $" External Server\n {externalServer.IP}:{externalServer.port}";

                // Bind the event onto the button
                externalServerButton.onClick.AddListener(() =>
                {
                    // Play sound
                    _buttonSound.Play();
                    // Change ip and port
                    this._port = externalServer.port;
                    this._host = externalServer.IP;
                    _fileLoaded.HaveServer = true;
                    _fileLoaded.ServerHost = _host;
                    _fileLoaded.Token = _token;
                    _fileLoaded.ServerPort = _port;
                    SceneManager.LoadScene("Play");
                });

                // Put the button into the content
                externalServerButtonObject.transform.SetParent(this._scrollViewContent.transform);
                externalServerButtonObject.transform.localScale = Vector3.one;
            }

        }
        // Update
        _playSwitchButton = GameObject.Find("Canvas/Scroll View/PlaySwitchButton").GetComponent<Button>();
        _playSwitchButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            UpdateServerList();
        });

        // Add Server Buttons
        this._addExternalServerCancelButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();

            this._scrollView.SetActive(true);
            this._addExternalServerWindow.SetActive(false);
        });
        this._addExternalServerConfirmButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();

            this._scrollView.SetActive(true);
            this._addExternalServerWindow.SetActive(false);
            // Check the IP and port 
            string ip = this._addExternalServerIPText.text;
            string processedPortString = this._addExternalServerPortText.text;
            for (int i = 0; i < processedPortString.Length; i++)
            {
                if (processedPortString[i] < '0' || processedPortString[i] > '9')
                {
                    processedPortString = processedPortString[..i];
                    break;
                }
            }
            bool portParseSuccess = int.TryParse(processedPortString, out int port);
            //Debug.Log($"{this._addExternalServerPortText.text.Length}:{port},{portParseSuccess}"); 

            if (!portParseSuccess)
            {
                return;
            }
            // Write the info into a new button 
            _addedExternalServers.Add(new(ip, port));
            // Write the new server info into the config json
            ((JArray)_addedserversJson).Add(new JObject() {
                { "IP", ip },
                {"port",port }
            });
            File.WriteAllText(this._addedserversJsonPath, _addedserversJson.ToString(), System.Text.Encoding.UTF8);

            // Refresh the content
            UpdateServerList();
        });

        // Close the scroll view before the start game button is pressed
        _scrollView.SetActive(false);

    }


}
