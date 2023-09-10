using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using System.Linq;
using NovelCraft.Sdk;
using System;

public class Record : MonoBehaviour
{
    public enum PlayState
    {
        Prepare,
        Play,
        Pause,
        End,
        Jump
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

        /// <summary>
        /// The target tick to jump
        /// </summary>
        public int JumpTargetTick = int.MaxValue;
        /// <summary>
        /// Current max tick
        /// </summary>
        public int MaxTick;
        public void Reset()
        {
            //this.RecordSpeed = 1f;
            this.NowTick = 0;
            this.NowRecordNum = 0;
            JumpTargetTick = int.MaxValue;
        }
    }
    private BlockCreator _blockCreator;
    private EntityCreator _entityCreator;
    private RecordInfo _recordInfo;
    private Upload _upload = new() { };
    private Upload.OpenFileName _recordFile = new() { };
    private JArray _recordArray;
    private Dictionary<string, JArray> _recordDict = new();

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
    /// Replay button
    /// </summary>
    private Button _replayButton;
    /// <summary>
    /// The slider which can change the record playing rate
    /// </summary>
    private Slider _recordSpeedSlider;
    private TMP_Text _recordSpeedText;
    private float _recordSpeedSliderMinValue;
    private float _recordSpeedSliderMaxValue;

    /// <summary>
    /// 
    /// </summary>
    private Slider _processSlider;

    private TMP_Text _jumpTargetTickText; // The target tick text in Unity 
    private TMP_Text _maxTickText; // The text of max tick in Unity


    /// <summary>
    /// Shade on or off
    /// </summary>
    private Button _shadeButton;
    private TMP_Text _shadeButtonText;
    private Light _light;
    private bool _isShadeOn;

    private Button _flashlightButton;
    private TMP_Text _flashlightButtonText;
    private Light _flashlight;
    private bool _isFlashlightOn;

    /// <summary>
    /// Main Slot bar
    /// </summary>
    private MainSlots _mainSlots;
    public RecordInfo RecordInformation
    {
        get
        {
            return this._recordInfo;
        }
    }

    private Observe _obeserve;

    /// <summary>
    /// Registered agents
    /// </summary>
    private Dictionary<int, string> _registeredAgents = new();

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
        this._obeserve = GameObject.Find("Main Camera").GetComponent<Observe>();

        // GUI //

        // Get stop button 
        this._stopButton = GameObject.Find("Canvas/StopButton").GetComponent<Button>();
        // Get stop button sprites
        this._stopButtonSprite = Resources.Load<Sprite>("GUI/Button/StopButton");
        this._continueButtonSprite = Resources.Load<Sprite>("GUI/Button/ContinueButton");
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

        // Get Replay button
        //this._replayButton = GameObject.Find("Canvas/ReplayButton").GetComponent<Button>();
        //this._replayButton.onClick.AddListener(() =>
        //{
        //    this._recordInfo.Reset();
        //    this._entityCreator.DeleteAllEntities();
        //});


        // Record playing rate slider
        this._recordSpeedSlider = GameObject.Find("Canvas/RecordSpeedSlider").GetComponent<Slider>();
        this._recordSpeedText = GameObject.Find("Canvas/RecordSpeedSlider/Value").GetComponent<TMP_Text>();

        this._recordSpeedSliderMinValue = this._recordSpeedSlider.minValue;
        this._recordSpeedSliderMaxValue = this._recordSpeedSlider.maxValue;
        // Set the default slider speed to 1;
        // Linear: 0~1
        float speedRate = (1 - RecordInfo.MinSpeed) / (RecordInfo.MaxSpeed - RecordInfo.MinSpeed);
        this._recordSpeedSlider.value = this._recordSpeedSliderMinValue + (this._recordSpeedSliderMaxValue - this._recordSpeedSliderMinValue) * speedRate;
        // Add listener
        this._recordSpeedSlider.onValueChanged.AddListener((float value) =>
        {
            // Linear
            float sliderRate = (value - this._recordSpeedSliderMinValue) / (this._recordSpeedSliderMaxValue - this._recordSpeedSliderMinValue);
            // Compute current speed
            this._recordInfo.RecordSpeed = RecordInfo.MinSpeed + (RecordInfo.MaxSpeed - RecordInfo.MinSpeed) * sliderRate;
            // Update speed text
            _recordSpeedText.text = $"Speed: {Mathf.Round(this._recordInfo.RecordSpeed * 100) / 100f:F2}";
            foreach (Player player in EntitySource.PlayerDict.Values)
            {
                player.PlayerAnimations.SetAnimatorSpeed(this._recordInfo.RecordSpeed);
            }
        });


        // Check
        if (this._recordFile == null)
        {
            Debug.Log("Loading file error!");
            return;
        }
        this._recordArray = LoadRecordData();
        this._recordInfo.MaxTick = (int)this._recordArray.Last["tick"];
        // Generate record Dict according to record array
        foreach (JToken eventJson in this._recordArray)
        {
            string identifier = eventJson["identifier"].ToString();
            if (this._recordDict.ContainsKey(identifier))
            {
                this._recordDict[identifier].Add(eventJson);
            }
            else
            {
                this._recordDict.Add(identifier, new JArray(eventJson));
            }
        }

        // Process slider
        this._processSlider = GameObject.Find("Canvas/ProcessSlider").GetComponent<Slider>();
        this._processSlider.value = 1;
        this._jumpTargetTickText = GameObject.Find("Canvas/ProcessSlider/Handle Slide Area/Handle/Value").GetComponent<TMP_Text>();
        this._maxTickText = GameObject.Find("Canvas/ProcessSlider/Max").GetComponent<TMP_Text>();
        this._recordInfo.MaxTick = (int)(this._recordArray.Last["tick"]);
        this._maxTickText.text = $"{this._recordInfo.MaxTick}";
        // Add listener
        this._processSlider.onValueChanged.AddListener((float value) =>
        {
            int nowTargetTick = (int)(value * this._recordInfo.MaxTick) + 1; // Add 1 owing to interpolation
            if (PlayState.Play == this._recordInfo.NowPlayState && Mathf.Abs(this._recordInfo.NowTick - nowTargetTick) > 1)
            {
                // Jump //
                // Reset the scene if the jump tick is smaller than now tick
                if (this._recordInfo.NowTick > nowTargetTick)
                {
                    this._recordInfo.Reset();
                    this._entityCreator.DeleteAllEntities();
                    // Reset All blocks;
                    // foreach (JToken blockChangeEventJson in this._recordDict["after_block_change"])
                }
                // Change current state
                this._recordInfo.NowPlayState = PlayState.Jump;
                // Change target tick
                this._recordInfo.JumpTargetTick = nowTargetTick;

                _registeredAgents.Clear();

            }
        });

        // Shade
        _light = GameObject.Find("Directional Light").GetComponent<Light>();
        _shadeButton = GameObject.Find("Canvas/Shade").GetComponent<Button>();
        _shadeButtonText = GameObject.Find("Canvas/Shade/ShadeText").GetComponent<TMP_Text>();
        _isShadeOn = true;
        _shadeButton.onClick.AddListener(() =>
        {
            this._isShadeOn = !this._isShadeOn;
            if (this._isShadeOn == true)
            {
                this._light.shadows = LightShadows.Soft;
                this._shadeButtonText.text = "Shade on";
            }
            else
            {
                this._light.shadows = LightShadows.None;
                this._shadeButtonText.text = "Shade off";
            }
        });

        // Flash light
        _flashlight = GameObject.Find("Main Camera/Spot Light").GetComponent<Light>();
        _flashlightButton = GameObject.Find("Canvas/FlashLight").GetComponent<Button>();
        _flashlightButtonText = GameObject.Find("Canvas/FlashLight/FlashLightText").GetComponent<TMP_Text>();
        _isFlashlightOn = true;
        _flashlightButton.onClick.AddListener(() =>
        {
            this._isFlashlightOn = !this._isFlashlightOn;
            if (this._isFlashlightOn == true)
            {
                this._flashlight.gameObject.SetActive(true);
                this._flashlightButtonText.text = "Flashlight on";
            }
            else
            {
                this._flashlight.gameObject.SetActive(false);
                this._flashlightButtonText.text = "Flashlight off";
            }
        });

        // MainSlots
        this._mainSlots = GameObject.Find("ObserverCanvas/MainSlots").GetComponent<MainSlots>();
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
    #region Event Definition

    private void AfterAgentRegisterEvent(JObject eventDataJson)
    {
        JArray agentList = (JArray)eventDataJson["agent_list"];

        foreach (JObject agentJson in agentList)
        {
            Debug.Log(agentJson);
            int uniqueId = (int)agentJson["unique_id"];

            string name = agentJson["name"].ToString();
            _registeredAgents.Add(uniqueId, name);
            //try
            //{

            //}
            //catch
            //{

            //}
        }

    }

    /// <summary>
    /// Change the position of entity
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntityCreateEvent(JObject eventDataJson)
    {
        JArray creationList = (JArray)eventDataJson["creation_list"];
        foreach (JObject entityJson in creationList)
        {
            int entityId = (int)entityJson["entity_type_id"];
            int uniqueId = (int)entityJson["unique_id"];
            Vector3 position = new(
                (float)entityJson["position"]["x"],
                (float)entityJson["position"]["y"],
                (float)entityJson["position"]["z"]
            );
            float yaw = (float)entityJson["orientation"]["yaw"];
            float pitch = (float)entityJson["orientation"]["pitch"];

            if (entityId == 0)
            {
                // Player
                Player player = new Player(uniqueId, position, yaw, pitch);
                if (this._entityCreator.CreatePlayer(player) == true)
                {
                    Debug.Log($"Create Player (id: 0, unique_id: {uniqueId}, yaw: {yaw}, pitch: {pitch}) successfully!");
                    if (this._registeredAgents.ContainsKey(uniqueId))
                        player.EntityObject.GetComponentInChildren<PlayerNameDisplay>().SetName(this._registeredAgents[uniqueId]);
                }
                else
                {
                    Debug.Log($"Create Player (id: 0, unique_id: {uniqueId}) error!");
                }

            }
            else if (entityId == 1)
            {
                // Item
                int itemTypeId = (int)(entityJson["item_type_id"] ?? 12);

                if (this._entityCreator.CreateItem(new Item(uniqueId, position, itemTypeId)) == true)
                {
                    Debug.Log($"Create item (id: {itemTypeId}, unique_id: {uniqueId}) successfully!");
                }
                else
                {
                    Debug.Log($"Create item (id: {itemTypeId}, unique_id: {uniqueId}) error!");
                }
            }
        }
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntityPositionChangeEvent(JObject eventDataJson)
    {
        JArray changeList = (JArray)eventDataJson["change_list"];

        foreach (JObject entityJson in changeList)
        {
            int uniqueId = (int)entityJson["unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            if (entityTypeId == 0)
            {
                // Update the position
                Vector3 newPosition = new Vector3(
                    (float)entityJson["position"]["x"],
                    (float)entityJson["position"]["y"],
                    (float)entityJson["position"]["z"]
                );
                ((Player)entity).UpdatePosition(newPosition, this._recordInfo.RecordSpeed);
            }
            else if (entityTypeId == 1)
            {
                // Update the position
                Vector3 newPosition = new Vector3(
                    (float)entityJson["position"]["x"],
                    (float)entityJson["position"]["y"],
                    (float)entityJson["position"]["z"]
                );
                ((Item)entity).UpdatePosition(newPosition, this._recordInfo.RecordSpeed);
            }
        }
    }

    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntityOrientationChangeEvent(JObject eventDataJson)
    {
        JArray changeList = (JArray)eventDataJson["change_list"];

        foreach (JObject entityJson in changeList)
        {
            int uniqueId = (int)entityJson["unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            float pitch = (float)entityJson["orientation"]["pitch"];
            float yaw = (float)entityJson["orientation"]["yaw"];

            if (entityTypeId == 0)
            {
                // Update the orientation
                ((Player)entity).UpdateOrientation(pitch, yaw);
            }
            else if (entityTypeId == 1)
            {
                // Update the orientation
                ((Item)entity).UpdateOrientation(pitch, yaw);
            }
        }
    }
    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntityRemoveEvent(JObject eventDataJson)
    {
        JArray removalList = (JArray)eventDataJson["removal_list"];

        foreach (var entityJson in removalList)
        {
            int uniqueId = (int)entityJson["unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            if (entityTypeId == 0)
            {
                this._entityCreator.DeletePlayer((Player)entity);
            }
            else if (entityTypeId == 1)
            {
                this._entityCreator.DeleteItem((Item)entity);
            }
        }
    }
    /// <summary>
    /// Create an entity
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterBlockChange(JObject eventDataJson)
    {
        JArray changeList = (JArray)eventDataJson["change_list"];

        foreach (var blockJson in changeList)
        {
            int x = (int)blockJson["position"]["x"];
            int y = (int)blockJson["position"]["y"];
            int z = (int)blockJson["position"]["z"];

            short newId = (short)blockJson["block_type_id"];
            Block block = this._blockCreator.UpdateBlock(new Vector3Int(x, y, z), newId, BlockDicts.BlockNameArray[newId], out short? originalTypeId);
            if (block != null)
            {
                // Check visibility of other blocks which are around this block;
                if ((originalTypeId == 0 && block.Id != 0) || (originalTypeId != 0 && block.Id == 0))
                {
                    CheckVisibility.CheckSingleBlockNeighbourVisibility(this._blockCreator, block);
                }
                //Debug.Log($"Change block ({x},{y},{z}) from {originalTypeId} to {newId}!");
            }
            else
            {
                //Debug.Log($"Cannot get block ({x},{y},{z})!");
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntitySpawn(JObject eventDataJson)
    {
        JArray spawnList = (JArray)eventDataJson["spawn_list"];
        foreach (JToken entityJson in spawnList)
        {
            int uniqueId = (int)entityJson["unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            this._entityCreator.SpawnEntity(entity);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterEntityDespawn(JObject eventDataJson)
    {
        JArray spawnList = (JArray)eventDataJson["despawn_list"];
        foreach (JToken entityJson in spawnList)
        {
            int uniqueId = (int)entityJson["unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            StartCoroutine(this._entityCreator.DespawnEntity(entity, (int)entityTypeId));
        }
    }

    private void AfterEntityAttack(JObject eventDataJson)
    {
        JArray attackList = (JArray)eventDataJson["attack_list"];
        foreach (JToken entityJson in attackList)
        {
            int uniqueId = (int)entityJson["attacker_unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            if (entityTypeId == 0)
            {
                string attackKind = entityJson["attack_kind"].ToString();
                switch (attackKind)
                {
                    case "click":
                        ((Player)entity).PlayerAnimations.AttackAnimationPlayer(isContinuous: false, isAttackStart: true);
                        break;
                    case "hold_start":
                        ((Player)entity).PlayerAnimations.AttackAnimationPlayer(isContinuous: true, isAttackStart: true);
                        break;
                    case "hold_end":
                        ((Player)entity).PlayerAnimations.AttackAnimationPlayer(isContinuous: true, isAttackStart: false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    private void AfterEntityHurt(JObject eventDataJson)
    {
        JArray hurtList = (JArray)eventDataJson["hurt_list"];
        foreach (JToken entityJson in hurtList)
        {
            int uniqueId = (int)entityJson["victim_unique_id"];

            Entity entity = EntitySource.GetEntity(uniqueId, out int? entityTypeId);
            if (entityTypeId == null) continue;

            if (entityTypeId == 0)
            {
                int damage = (int)entityJson["damage"];

                // Update health bar
                if (entity is Player player)
                {
                    player.Health -= damage;
                    _obeserve.UpdateHealthBar(player);
                }
                StartCoroutine(((Player)entity).PlayerHurt(this._recordInfo.RecordSpeed));
            }
        }
    }

    private void AddItemOnPlayerHand(Player player)
    {
        // Change main hand obj according to the item of main_hand_slot
        // Find right hand
        Transform rightHandTransform = player.RightArms.transform.GetChild(0);

        // Clear the item obj in the right hand
        if (rightHandTransform.childCount != 0)
        {
            Destroy(rightHandTransform.GetChild(0).gameObject);
        }

        int nowMainHandItemId = player.Inventory.Slots[player.MainHandSlot].ItemId;
        // If current main hand item is air
        if (nowMainHandItemId == 0)
        {
            return;
        }

        // New Item Obj
        GameObject itemObject = (GameObject)Instantiate(this._entityCreator.ItemPrefabs[nowMainHandItemId]);
        itemObject.transform.parent = rightHandTransform;
        itemObject.transform.localScale = new Vector3(itemObject.transform.localScale.x / this._entityCreator.ItemScaleRate,
            itemObject.transform.localScale.y / this._entityCreator.ItemScaleRate,
            itemObject.transform.localScale.z / this._entityCreator.ItemScaleRate
        );

        itemObject.transform.position = rightHandTransform.position + new Vector3(0.4f, 0.7f, 0.4f);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventDataJson"></param>
    private void AfterPlayerInventoryChange(JObject eventDataJson)
    {
        JArray changeList = (JArray)eventDataJson["change_list"];
        foreach (JToken entityJson in changeList)
        {
            int playerUniqueId = (int)entityJson["player_unique_id"];
            Player player = EntitySource.GetPlayer(playerUniqueId);
            if (player == null) continue;

            // Get slots change list
            JArray slotChangeList = (JArray)entityJson["change_list"];
            foreach (JToken slotChangeJson in slotChangeList)
            {
                int slot = (int)slotChangeJson["slot"];
                int count = (int)slotChangeJson["count"];

                if (slot < 0 || slot >= Inventory.SlotNum)
                {
                    Debug.Log("Slot index out of bounds!");
                    continue;
                }

                // Check if the item type id exists
                JToken itemTypeIdJson = slotChangeJson["item_type_id"];
                if (itemTypeIdJson == null && count != 0)
                {
                    Debug.Log("No itemId but its count is not 0");
                    continue;
                }

                // Id:
                int itemTypeId = (int)(itemTypeIdJson ?? 0);

                // Change slots
                player.Inventory.Slots[slot].Count = count;
                if (itemTypeIdJson != null)
                    player.Inventory.Slots[slot].ItemId = itemTypeId;

                // ?
                // Change main slots bar
                if (slot < 9)
                {
                    this._mainSlots.ChangeSlotItem(slot, itemTypeId, count);
                    Debug.Log($"Change slot {slot}, item: {itemTypeId}, count: {count}");
                    foreach (var slotInfo in player.Inventory.Slots)
                    {
                        Debug.Log($"Slot Info: Slot {slotInfo.SlotIndex}, item: {slotInfo.ItemId}, count: {slotInfo.Count}");
                    }
                }

                if (slot == player.MainHandSlot)
                {
                    AddItemOnPlayerHand(player);
                }
            }
        }
    }

    private void AfterPlayerSwitchMainHand(JObject eventDataJson)
    {
        JArray changeList = (JArray)eventDataJson["change_list"];
        foreach (JToken entityJson in changeList)
        {
            int playerUniqueId = (int)entityJson["player_unique_id"];
            Player player = EntitySource.GetPlayer(playerUniqueId);
            if (player == null) continue;

            // Change the main hand slot
            player.MainHandSlot = (int)entityJson["new_main_hand"];
            AddItemOnPlayerHand(player);
        }
    }
    #endregion

    #region Record Update
    #endregion
    /// <summary>
    /// 
    /// </summary>
    private void UpdateTick()
    {
        // Play
        try
        {
            if (this._recordInfo.RecordSpeed > 0)
            {
                List<JObject> nowEventsJson = new();

                // Find all the events at now tick
                for (; this._recordInfo.NowRecordNum < this._recordArray.Count; this._recordInfo.NowRecordNum++)
                {
                    JObject nowEvent = (JObject)this._recordArray[this._recordInfo.NowRecordNum];
                    if (this._recordInfo.NowTick == (int)nowEvent["tick"])
                    {
                        nowEventsJson.Add(nowEvent);
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (var nowEventJson in nowEventsJson)
                {

                    if (nowEventJson["type"].ToString() == "event")
                    {
                        JObject nowEventDataJson = (JObject)nowEventJson["data"];
                        switch (nowEventJson["identifier"].ToString())
                        {
                            case "after_entity_create":
                                this.AfterEntityCreateEvent(nowEventDataJson);
                                break;
                            case "after_entity_position_change":
                                this.AfterEntityPositionChangeEvent(nowEventDataJson);
                                break;
                            case "after_entity_remove":
                                this.AfterEntityRemoveEvent(nowEventDataJson);
                                break;
                            case "after_block_change":
                                this.AfterBlockChange(nowEventDataJson);
                                break;
                            case "after_entity_orientation_change":
                                this.AfterEntityOrientationChangeEvent(nowEventDataJson);
                                break;
                            case "after_entity_spawn":
                                this.AfterEntitySpawn(nowEventDataJson);
                                break;
                            case "after_entity_despawn":
                                this.AfterEntityDespawn(nowEventDataJson);
                                break;
                            case "after_entity_attack":
                                this.AfterEntityAttack(nowEventDataJson);
                                break;
                            case "after_entity_hurt":
                                this.AfterEntityHurt(nowEventDataJson);
                                break;
                            case "after_player_inventory_change":
                                this.AfterPlayerInventoryChange(nowEventDataJson);
                                break;
                            case "after_player_switch_main_hand":
                                this.AfterPlayerSwitchMainHand(nowEventDataJson);
                                break;
                            case "after_agent_register":
                                this.AfterAgentRegisterEvent(nowEventDataJson);
                                break;
                            default:
                                break;
                        }
                    }
                }
                // Ticks
                this._recordInfo.NowTick++;
                this._jumpTargetTickText.text = $"Tick\n{this._recordInfo.NowTick}"; // Update process slider text
                                                                                     // move the process slider
                this._processSlider.value = (this._recordInfo.NowTick / (float)this._recordInfo.MaxTick);
                // Jump end if now tick reaches JumpTargetTick
                if (this._recordInfo.NowTick >= this._recordInfo.JumpTargetTick &&
                    this._recordInfo.NowPlayState == PlayState.Jump)
                { this._recordInfo.NowPlayState = PlayState.Play; }
            }
            // Upend
            else
            {

            }
        }
        catch
        {
            int a = 0;
        }

    }

    private void Update()
    {
        if ((this._recordInfo.NowPlayState == PlayState.Play && this._recordInfo.NowRecordNum < this._recordArray.Count) ||
            this._recordInfo.NowPlayState == PlayState.Jump)
        {
            if (this._recordInfo.NowDeltaTime > this._recordInfo.NowframeTime || this._recordInfo.NowPlayState == PlayState.Jump)
            {
                UpdateTick();
                this._recordInfo.NowDeltaTime = 0;
            }
            this._recordInfo.NowDeltaTime += Time.deltaTime;
        }
    }
}

