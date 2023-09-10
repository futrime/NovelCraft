using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;

public class RecordLegacy : MonoBehaviour
{
    public enum PlayState
    {
        Prepare,
        Play,
        Pause,
        End
    }
    public class RecordInfo
    {
        // 20 frame per second
        public const float FrameTime = 0.05f;
        public PlayState NowPlayState = PlayState.Pause;
        public int NowTick = 0;
        /// <summary>
        /// Now record serial number
        /// </summary>
        public int NowRecordNum = 0;
        /// <summary>
        /// The speed of the record which can be negative
        /// </summary>
        public float RecordSpeed = 1f;
        public const float MinSpeed = -5f;
        public const float MaxSpeed = 5f;

        /// <summary>
        /// Contains all the item in the game
        /// </summary>
        public float NowframeTime
        {
            get
            {
                return FrameTime / RecordSpeed;
            }
        }
        /// <summary>
        /// If NowDeltaTime is larger than NowframeTime, then play the next frame
        /// </summary>
        public float NowDeltaTime = 0;
    }
    private BlockCreator _blockCreator;
    private EntityCreator _entityCreator;
    private RecordInfo _recordInfo;
    private Upload _upload = new() { };
    private Upload.OpenFileName _recordFile = new() { };
    private JArray _recordArray;

    /// <summary>
    /// Stop / Continue button
    /// </summary>
    private Button _stopButton;
    /// <summary>
    /// The stop sprite
    /// </summary>
    private Sprite _stopButtonSprite;
    private Sprite _continueButtonSprite;
    /// <summary>
    /// The slider which can change the record playing rate
    /// </summary>
    private Slider _recordSpeedSlider;
    private TMP_Text _recordSpeedText;
    private float _recordSpeedSliderMinValue;
    private float _recordSpeedSliderMaxValue;

    public RecordInfo RecordInformation
    {
        get
        {
            return this._recordInfo;
        }
    }

    private void Start()
    {
        // Initialize the _recordInfo
        this._recordInfo = new();
        // Initialize the BlockCreator
        this._blockCreator = GameObject.Find("BlockCreator").GetComponent<BlockCreator>();
        // Initialize the ItemCreator
        this._entityCreator = GameObject.Find("EntityCreator").GetComponent<EntityCreator>();
        // Get json file
        var fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();
        // Check if the file is Level json
        this._recordFile = fileLoaded.File;

        // GUI //

        // Get stop button 
        this._stopButton = GameObject.Find("Canvas/StopButton").GetComponent<Button>();
        // Get stop button sprites
        this._stopButtonSprite = Resources.Load<Sprite>("GUI/Button/StopButton");
        this._continueButtonSprite = Resources.Load<Sprite>("GUI/Button/ContinueButton");
        Debug.Log(this._stopButtonSprite);
        // Pause at beginning
        this._stopButton.GetComponent<Image>().sprite = _continueButtonSprite;
        // Add listener to stop button
        this._stopButton.onClick.AddListener(() =>
        {
            if (this._recordInfo.NowPlayState == PlayState.Play)
            {
                this._stopButton.GetComponent<Image>().sprite = this._continueButtonSprite;
                this._recordInfo.NowPlayState = PlayState.Pause;
            }
            else if (this._recordInfo.NowPlayState == PlayState.Pause)
            {
                this._stopButton.GetComponent<Image>().sprite = this._stopButtonSprite;
                this._recordInfo.NowPlayState = PlayState.Play;
            }
        });


        // Record playing rate slider
        this._recordSpeedSlider = GameObject.Find("Canvas/RecordSpeedSlider").GetComponent<Slider>();
        this._recordSpeedText = GameObject.Find("Canvas/RecordSpeedSlider/Value").GetComponent<TMP_Text>();

        this._recordSpeedSliderMinValue = this._recordSpeedSlider.minValue;
        this._recordSpeedSliderMaxValue = this._recordSpeedSlider.maxValue;
        // Set the default slider speed to 1;
        // Linear: 0~1
        float speedRate = (1 - RecordInfo.MinSpeed) / (RecordInfo.MaxSpeed - RecordInfo.MinSpeed);
        this._recordSpeedSlider.value = this._recordSpeedSliderMinValue + (this._recordSpeedSliderMaxValue - this._recordSpeedSliderMinValue) * speedRate;
        // Add listenr
        this._recordSpeedSlider.onValueChanged.AddListener((float value) =>
        {
            // Linear
            float sliderRate = (value - this._recordSpeedSliderMinValue) / (this._recordSpeedSliderMaxValue - this._recordSpeedSliderMinValue);
            // Compute current speed
            this._recordInfo.RecordSpeed = RecordInfo.MinSpeed + (RecordInfo.MaxSpeed - RecordInfo.MinSpeed) * sliderRate;
            // Update speed text
            _recordSpeedText.text = $"Speed: {Mathf.Round(this._recordInfo.RecordSpeed * 100) / 100f:F2}";
        });


        // Check
        if (this._recordFile == null)
        {
            Debug.Log("Loading file error!");
            return;
        }
        this._recordArray = LoadRecordData();
    }
    private JArray LoadRecordData()
    {
        JObject recordJsonObject = JsonUtility.UnzipRecord(this._recordFile.File);
        // Load the record array
        JArray recordArray = (JArray)recordJsonObject["records"];

        if (recordArray == null)
        {
            Debug.Log("Record file is empty!");
            return null;
        }
        Debug.Log(recordArray.ToString());
        return recordArray;
    }
    /// <summary>
    /// 
    /// </summary>
    private void UpdateTick()
    {
        // Play
        if (this._recordInfo.RecordSpeed > 0)
        {

            JToken nowRecord = this._recordArray[this._recordInfo.NowRecordNum];
            int nextRecordTick = int.Parse(nowRecord["ticks"].ToString());
            // If the next record tick is greater than now tick, this implies there is nothing to update in this tick 
            if (nextRecordTick > this._recordInfo.NowTick)
            {
                return;
            }
            // Else 
            this._recordInfo.NowRecordNum++;

            // Blocks
            JToken blocks = nowRecord["blocks"];
            if (blocks != null)
            {
                // Update block
                //blocks = (JArray)blocks;
                foreach (JObject block in blocks)
                {
                    int x = int.Parse(block["x"].ToString());
                    int y = int.Parse(block["y"].ToString());
                    int z = int.Parse(block["z"].ToString());
                    short id = short.Parse(block["id"].ToString());
                    this._blockCreator.UpdateBlock(new Vector3Int(x, y, z), id, BlockDicts.BlockNameArray[id], out short? originalBlockId);
                }
            }

            // Entities
            JToken entities = nowRecord["entities"];
            if (entities != null)
            {
                //Debug.Log(entities.ToString());
                foreach (JObject entity in entities)
                {
                    //Debug.Log(entity.ToString());
                    int entityId = int.Parse(entity["entity_id"].ToString());// 0: player; 1: item
                    int uniqueId = int.Parse(entity["unique_id"].ToString());

                    Vector3 position = new();
                    int itemType = 8;

                    // Judge whether this entity will be spawned in this tick so that we can get its spawning position 
                    Entity.Event entityEvent = Entity.Event.None;
                    JToken entityEventToken = entity["event"];
                    if (entityEventToken != null)
                    {
                        string eventName = entityEventToken.ToString();
                        if (eventName == "spawn")
                        {
                            entityEvent = Entity.Event.Spawn;
                        }
                        else if (eventName == "despawn")
                        {
                            entityEvent = Entity.Event.Despawn;
                        }
                    }
                    // Data value: the type of item
                    JToken itemTypeToken = entity["data_value"];
                    if (itemTypeToken != null)
                    {
                        itemType = int.Parse(itemTypeToken.ToString());
                    }

                    // Postion
                    JToken positionToken = entity["position"];
                    if (positionToken != null)
                    {
                        if (entityId == 1)
                        {
                            // Find if the token exist
                            Item item = EntitySource.GetItem(uniqueId);
                            if (item != null || entityEvent == Entity.Event.Spawn)
                            {
                                // Get the position of x,y,z
                                float itemX = float.Parse(positionToken["x"].ToString());
                                float itemY = float.Parse(positionToken["y"].ToString());
                                float itemZ = float.Parse(positionToken["z"].ToString());
                                // Update the position
                                position = new Vector3(itemX, itemY, itemZ);
                                if (item != null)
                                {
                                    item.UpdatePosition(position, this._recordInfo.RecordSpeed);
                                }
                            }
                        }
                    }
                    // Orientation: (to be checked)
                    JToken orientationToken = entity["orientation"];
                    if (orientationToken != null)
                    {
                        if (entityId == 1)
                        {
                            // Find if the token exist
                            Item item = EntitySource.GetItem(uniqueId);
                            if (item != null)
                            {
                                // Get the pitch, yaw
                                int pitch = int.Parse(orientationToken["pitch"].ToString());
                                int yaw = int.Parse(orientationToken["yaw"].ToString());
                                // Update the orientation
                                item.UpdateOrientation(pitch, yaw);
                            }
                        }
                    }

                    // Event: spawn / despawn
                    if (entityEventToken != null || this._recordInfo.NowTick == 0)
                    {
                        if (entityEvent == Entity.Event.Spawn && entityId == 0)
                        {
                            // Player
                        }
                        else if ((entityEvent == Entity.Event.Spawn && entityId == 1) || this._recordInfo.NowTick == 0)
                        {
                            if (this._entityCreator.CreateItem(new Item(uniqueId, position, itemType)) == false)
                            {
                                Debug.Log("Create item error!");
                            }
                            else
                            {
                                Debug.Log("Create item successfully!");
                            }
                        }
                        else if (entityEvent == Entity.Event.Despawn && entityId == 0)
                        {

                        }
                        else if (entityEvent == Entity.Event.Despawn && entityId == 1)
                        {
                            if (this._entityCreator.DeleteItem(new Item(uniqueId, position, itemType)) == false)
                            {
                                Debug.Log("Delete item error!");
                            }
                        }
                    }
                }
            }
            // Ticks
            this._recordInfo.NowTick++;
            // Player nowRecord
        }
        // Upend
        else
        {

        }

    }
    private void Update()
    {
        if (this._recordInfo.NowPlayState == PlayState.Play &&
            this._recordInfo.NowRecordNum < this._recordArray.Count)
        {
            if (this._recordInfo.NowDeltaTime > this._recordInfo.NowframeTime)
            {
                UpdateTick();
                this._recordInfo.NowDeltaTime = 0;
            }
            else
            {
                this._recordInfo.NowDeltaTime += Time.deltaTime;
            }
        }
    }
}

