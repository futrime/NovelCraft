using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NovelCraft.Sdk;
using NovelCraft.Utilities.Messages;

using System;
using System.Net.WebSockets;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEditor;

public class InputHandler : MonoBehaviour
{
    public const float RotatingSpeed = 120f;
    public const float RotatingDeltaTime = 0.05f;
    private float _rotatingNowTime = 0;

    private FileLoaded _fileLoaded;
    private Camera _camera;

    private string _token;
    private string _host;
    private int _port;

    private bool _hasCreatePlayer = false;
    private Player _player;
    private Animator _playerAnimator;
    /// <summary>
    /// Create blocks
    /// </summary>
    private BlockCreator _blockCreator;
    private EntityCreator _entityCreator;
    private bool _hasCreateInitialMap = false;
    private Vector3? _playerNowPosition;

    private IAgent.MovementKind? _movementKind = null;
    private IAgent.InteractionKind? _interactionKind = null;

    private GameObject _healthBar;
    private int _showHealth;
    private List<Image> _healthImageList = new();
    private Sprite _fullHealthImage;
    private Sprite _halfHealthImage;

    private GameObject _hungerBar;

    /// <summary>
    /// Slot background
    /// </summary>
    private MainSlots _mainSlots;
    private const int _slotNum = 9;

    public enum WalkState
    {
        Forward,
        Backward,
        Left,
        Right,
        Stop
    }
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        _camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        _fileLoaded = GameObject.Find("FileLoaded").GetComponent<FileLoaded>();
        this._blockCreator = GameObject.Find("BlockCreator").GetComponent<BlockCreator>();
        this._entityCreator = GameObject.Find("EntityCreator").GetComponent<EntityCreator>();
        this._healthBar = GameObject.Find("Canvas/Health");
        this._halfHealthImage = Resources.Load<Sprite>("GUI/Slot/HalfHealth");
        this._fullHealthImage = Resources.Load<Sprite>("GUI/Slot/FullHealth");

        // Create max health 
        for (int i = 0; i < 10; i++)
        {
            GameObject healthObject = new();
            Image healthImage = healthObject.AddComponent<Image>();
            healthImage.sprite = this._fullHealthImage;
            // Set father
            healthObject.transform.SetParent(this._healthBar.transform);
            healthObject.transform.localScale = Vector3.one;
            // Add list
            this._healthImageList.Add(healthImage);
        }
        _showHealth = 20;

        // Add Slot background
        //_mainSlots.


        if (_fileLoaded.HaveServer)
        {
            _token = _fileLoaded.Token;
            _host = _fileLoaded.ServerHost;
            _port = _fileLoaded.ServerPort;
            Sdk.Initialize(_token, _host, _port);
            Debug.Log(_token);
        }
        // Register block change event
        Sdk.Client.AfterMessageReceiveEvent += (sender, message) =>
        {
            Loom.QueueOnMainThread((param) =>
            {
                switch (message)
                {
                    // Update blocks
                    case ServerAfterBlockChangeMessage msg:
                        if (_hasCreateInitialMap)
                        {
                            var changeList = msg.ChangeList;
                            foreach (var changeInfo in changeList)
                            {
                                Vector3Int position = new Vector3Int(
                                    changeInfo.Position.X,
                                    changeInfo.Position.Y,
                                    changeInfo.Position.Z
                                );
                                short typeId = (short)changeInfo.BlockTypeId;


                                this._blockCreator.UpdateBlock(position, typeId,
                                            BlockDicts.BlockNameArray[changeInfo.BlockTypeId],
                                            out short? originalBlockId);

                                CheckVisibility.CheckSingleBlockNeighbourVisibility(this._blockCreator, new Block(typeId, position));
                            }
                        }
                        break;

                    case ServerGetBlocksAndEntitiesMessage msg:
                        // Update visible blocks;
                        if (Sdk.Blocks is null) return;

                        List<Section> newSectionList = new();

                        foreach (var section in msg.Sections)
                        {
                            Vector3Int sectionPosition = new(section.Position.X, section.Position.Y, section.Position.Z);
                            if (BlockSource.SectionDict.ContainsKey(sectionPosition / 16))
                                continue;

                            Section newSection = new(sectionPosition / 16);
                            // Convert NovelCraft.Sdk.Section into Section
                            for (int x = 0; x < 16; x++)
                            {
                                for (int y = 0; y < 16; y++)
                                {
                                    for (int z = 0; z < 16; z++)
                                    {
                                        Block block = newSection.Blocks[x, y, z] = new Block();
                                        if (block is not null)
                                        {
                                            try
                                            {
                                                block.Position = new Vector3Int(x, y, z) + sectionPosition;
                                                short id = (short)section.Blocks[256 * x + 16 * y + z];
                                                block.Id = id;
                                                block.Name = BlockDicts.BlockNameArray[id];
                                            }
                                            catch
                                            {
                                                block.Id = 0;
                                            }
                                        }
                                    }
                                }
                            }
                            BlockSource.AddSection(newSection);
                            newSectionList.Add(newSection);
                        }
                        foreach (var newSection in newSectionList)
                        {
                            Debug.Log($"new Section: {newSection.PositionIndex.x},{newSection.PositionIndex.y},{newSection.PositionIndex.z}");
                            CheckVisibility.CheckSectionVisibility(this._blockCreator, newSection);
                        }
                        Debug.Log("Update whole!");

                        // Update entity info
                        foreach (var entityInfo in msg.Entities)
                        {
                            Entity entity = EntitySource.GetEntity(entityInfo.UniqueId, out int? entityTypeId);
                            if (entity is not null)
                            {
                                if (entityTypeId == 0 && entityInfo.UniqueId != this._player.UniqueId)
                                {
                                    ((Player)entity).UpdatePosition(new Vector3((float)entityInfo.Position.X,
                                        (float)entityInfo.Position.Y,
                                        (float)entityInfo.Position.Z), 1.0f);
                                }
                                else if (entityTypeId == 1)
                                {
                                    ((Item)entity).UpdatePosition(new Vector3((float)entityInfo.Position.X,
                                        (float)entityInfo.Position.Y,
                                        (float)entityInfo.Position.Z), 1.0f);
                                }
                            }
                        }
                        break;
                    case ServerAfterEntityCreateMessage msg:
                        foreach (var entityInfo in msg.CreationList)
                        {
                            // Find in the dict
                            if (EntitySource.GetEntity(entityInfo.UniqueId, out int? uniqueId) is not null)
                                continue;

                            // Item
                            if (entityInfo.EntityTypeId == 1)
                            {
                                Item newItem = new Item(entityInfo.UniqueId,
                                    new Vector3((float)entityInfo.Position.X, (float)entityInfo.Position.Y, (float)entityInfo.Position.Z),
                                    entityInfo.ItemTypeId.Value);
                                EntitySource.AddItem(newItem);
                                this._entityCreator.CreateItem(newItem);
                            }
                            // TO DO: Check
                            else if (entityInfo.EntityTypeId == 0)
                            {
                                Player newPlayer = new Player(entityInfo.UniqueId,
                                    new Vector3((float)entityInfo.Position.X, (float)entityInfo.Position.Y, (float)entityInfo.Position.Z),
                                    entityInfo.ItemTypeId.Value);
                                EntitySource.AddPlayer(newPlayer);
                                this._entityCreator.CreatePlayer(newPlayer);
                            }
                        }
                        break;
                    case ServerAfterEntityRemoveMessage msg:
                        // TO DO:
                        foreach (var entityInfo in msg.RemovalIdList)
                        {
                            // Find in the dict
                            Item item = EntitySource.GetItem(entityInfo);
                            Player player = EntitySource.GetPlayer(entityInfo);
                            // Could not find entity
                            if (item is null && player is null)
                                continue;

                            // Item
                            if (item is not null)
                            {
                                EntitySource.ItemDict.Remove(item.UniqueId);
                                this._entityCreator.DeleteItem(item);
                            }
                            // TO DO: delete Player
                            else if (player is not null)
                            {

                            }
                        }
                        break;

                    case ServerAfterPlayerInventoryChangeMessage msg:
                        foreach (var playerInventoryInfo in msg.ChangeList)
                        {
                            if (playerInventoryInfo.PlayerUniqueId == this._player.UniqueId)
                            {
                                foreach (var changeInfo in playerInventoryInfo.ChangeList)
                                {
                                    if (changeInfo.Slot < _slotNum)
                                    {
                                        int itemTypeId = -1;
                                        if (changeInfo.ItemTypeId is not null)
                                        {
                                            itemTypeId = changeInfo.ItemTypeId.Value;
                                        }
                                        this._mainSlots.ChangeSlotItem(changeInfo.Slot, itemTypeId, changeInfo.Count);
                                        Debug.Log($"Change {changeInfo.Slot} {itemTypeId} {changeInfo.Count}");
                                    }
                                }
                            }
                        }
                        break;
                }
            }, null);
        };

        this._mainSlots = GameObject.Find("Canvas/SlotBackground").GetComponent<MainSlots>();

        Cursor.visible = false; // Make the mouse ptr disappear
        Cursor.lockState = CursorLockMode.Confined;
    }
    private void Walk()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (horizontal > 0.01 && _movementKind != IAgent.MovementKind.Right)
        {
            _movementKind = IAgent.MovementKind.Right;
            Sdk.Agent.SetMovement(_movementKind);
            this._playerAnimator.SetBool("IsWalking", true);
        }
        else if (horizontal < -0.01 && _movementKind != IAgent.MovementKind.Left)
        {
            _movementKind = IAgent.MovementKind.Left;
            Sdk.Agent.SetMovement(_movementKind);
            this._playerAnimator.SetBool("IsWalking", true);
        }

        if (vertical > 0.01 && _movementKind != IAgent.MovementKind.Forward)
        {
            _movementKind = IAgent.MovementKind.Forward;
            Sdk.Agent.SetMovement(_movementKind);
            this._playerAnimator.SetBool("IsWalking", true);
        }
        else if (vertical < -0.01 && _movementKind != IAgent.MovementKind.Backward)
        {
            _movementKind = IAgent.MovementKind.Backward;
            Sdk.Agent.SetMovement(_movementKind);
            this._playerAnimator.SetBool("IsWalking", true);
        }

        if (Mathf.Abs(horizontal) < 0.01 && Mathf.Abs(vertical) < 0.01 && _movementKind != null)
        {
            _movementKind = null;
            Sdk.Agent.SetMovement(_movementKind);
            this._playerAnimator.SetBool("IsWalking", false);
        }
    }
    private void Jump()
    {
        bool isJumping = Input.GetKeyDown(KeyCode.Space);

        if (isJumping)
        {
            Sdk.Agent.Jump();
        }
    }

    private void Attack()
    {
        bool isClick = Input.GetMouseButtonDown((int)MouseButton.Left);
        bool isHoldStart = Input.GetMouseButton((int)MouseButton.Left);
        bool isHoldEnd = Input.GetMouseButtonUp((int)MouseButton.Left);

        if (isClick)
        {
            _interactionKind = IAgent.InteractionKind.Click;
            Sdk.Agent.Attack(_interactionKind.Value);
        }
        if (isHoldStart && _interactionKind == IAgent.InteractionKind.Click)
        {
            _interactionKind = IAgent.InteractionKind.HoldStart;
            Sdk.Agent.Attack(_interactionKind.Value);
            this._playerAnimator.SetBool("IsAttacking", true);
        }
        if (isHoldEnd && _interactionKind == IAgent.InteractionKind.HoldStart)
        {
            _interactionKind = IAgent.InteractionKind.HoldEnd;
            Sdk.Agent.Attack(_interactionKind.Value);
            this._playerAnimator.SetBool("IsAttacking", false);
        }
    }
    private void Use()
    {
        if (Input.GetMouseButtonDown((int)MouseButton.Right))
        {
            Sdk.Agent.Use(IAgent.UseKind.Click);
        }
    }
    private void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");
        decimal? pitch = null, yaw = null;

        if (Mathf.Abs(mouseX) > 0.01 || Mathf.Abs(mouseY) > 0.01)
        {
            // Get delta vector
            decimal deltaX = (decimal)(RotatingSpeed * mouseX * Time.deltaTime);
            decimal deltaY = (decimal)(RotatingSpeed * mouseY * Time.deltaTime);
            if (this._player.Head is not null)
            {
                pitch = (decimal)this._player.Head.transform.eulerAngles.x + deltaY;
                yaw = (decimal)this._player.EntityObject.transform.eulerAngles.y + deltaX;

                this._player.EntityObject.transform.eulerAngles = new Vector3(0, (float)yaw, 0);
                this._player.Head.transform.eulerAngles = new Vector3((float)pitch, 0, 0);

                this._camera.transform.eulerAngles = new Vector3((float)pitch, (float)yaw, 0);
            }
        }

        // Send to server
        if (_rotatingNowTime > RotatingDeltaTime)
        {
            if (pitch is not null && yaw is not null)
            {
                NovelCraft.Sdk.Position<decimal> GetForwardVector()
                {
                    // Normalized forward vector: (A, B, C)
                    decimal A = (decimal)(Math.Cos(-(double)pitch * Math.PI / 180) * Math.Sin((double)yaw * Math.PI / 180));
                    decimal B = (decimal)(Math.Sin(-(double)pitch * Math.PI / 180));
                    decimal C = (decimal)(Math.Cos(-(double)pitch * Math.PI / 180) * Math.Cos((double)yaw * Math.PI / 180));

                    return new NovelCraft.Sdk.Position<decimal>(A, B, C);
                }

                NovelCraft.Sdk.Position<decimal> agentPosition = new NovelCraft.Sdk.Position<decimal>(Sdk.Agent.Position);
                NovelCraft.Sdk.Position<decimal> forwardVector = GetForwardVector();
                Sdk.Agent.LookAt(new NovelCraft.Sdk.Position<decimal>(
                    agentPosition.X + forwardVector.X,
                    agentPosition.Y + forwardVector.Y + 1.62m,
                    agentPosition.Z + forwardVector.Z
                ));
                _rotatingNowTime = 0;
            }
        }
        _rotatingNowTime += Time.deltaTime;
    }
    private void UpdatePositionAndOrientation()
    {
        if (this._player is null || this._player.EntityObject is null) return;

        var iPosition = Sdk.Agent.Position;
        var position = new Vector3((float)iPosition.X, (float)iPosition.Y, (float)iPosition.Z);
        if (Vector3.Distance(position, this._playerNowPosition.Value) > 1e-7)
        {
            // Next position
            this._playerNowPosition = position;
            this._player.EntityObject.transform.position = position;
            this._camera.transform.position = new Vector3(position.x, position.y + 1.62f, position.z);
        }
        else if (this._playerNowPosition is not null && this._movementKind != null)
        {
            // Interpolation
            var newPosition = this._player.EntityObject.transform.position +
                new Vector3((float)Sdk.Agent.Velocity.X * Time.deltaTime, (float)Sdk.Agent.Velocity.Y * Time.deltaTime, (float)Sdk.Agent.Velocity.Z * Time.deltaTime);

            this._player.EntityObject.transform.position = newPosition;

            this._camera.transform.position = new Vector3(newPosition.x, newPosition.y + 1.62f, newPosition.z);
        }

        // Update player position and camera position
        // Adjust player orientation and camera orientation if the bias is large
    }
    private void UpdateHealthBar()
    {
        if (Sdk.Agent is null) return;

        // Compare the current health with the health of agent
        if (this._showHealth != (int)Sdk.Agent.Health)
        {
            // Update health bar
            this._showHealth = (int)Sdk.Agent.Health;
            for (int i = 0; i < this._healthImageList.Count; i++)
            {
                if (this._showHealth >= 2 * i)
                {
                    _healthImageList[i].sprite = this._fullHealthImage;
                    _healthImageList[i].color = new Color(1, 1, 1, 1);
                }
                else if (this._showHealth == 2 * i - 1)
                {
                    _healthImageList[i].sprite = this._halfHealthImage;
                    _healthImageList[i].color = new Color(1, 1, 1, 1);
                }
                else
                {
                    _healthImageList[i].sprite = null;
                    // Transparent
                    _healthImageList[i].color = new Color(1, 1, 1, 0);
                }
            }
        }
    }
    private void InitializeBlock()
    {
        if (this._hasCreateInitialMap == true) return;

        if (Sdk.Blocks is null) return;

        NovelCraft.Sdk.Section[] allSections = Sdk._blockSource.GetAllSections();
        foreach (NovelCraft.Sdk.Section section in allSections)
        {
            Section newSection = new(new Vector3Int(section.Position.X / 16, section.Position.Y / 16, section.Position.Z / 16));
            // Convert NovelCraft.Sdk.Section into Section
            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    for (int z = 0; z < 16; z++)
                    {
                        IBlock block = section[(new NovelCraft.Sdk.Position<int>(x, y, z))];
                        newSection.Blocks[x, y, z] = new Block();
                        if (block is not null)
                        {
                            try
                            {
                                newSection.Blocks[x, y, z].Position = new Vector3Int(block.Position.X, block.Position.Y, block.Position.Z);
                                newSection.Blocks[x, y, z].Id = (short)block.TypeId;
                                newSection.Blocks[x, y, z].Name = BlockDicts.BlockNameArray[block.TypeId];
                            }
                            catch
                            {
                                newSection.Blocks[x, y, z].Id = 0;
                            }
                        }
                    }
                }
            }
            BlockSource.AddSection(newSection);
        }

        CheckVisibility.CheckInnerVisibility(this._blockCreator);
        CheckVisibility.CheckNeighbourVisibility(this._blockCreator);
        this._hasCreateInitialMap = true;
    }
    private void InitializePlayer()
    {
        if (_hasCreatePlayer) return;

        if (Sdk.Agent is null) return;

        Vector3 position = new((float)Sdk.Agent.Position.X, (float)Sdk.Agent.Position.Y, (float)Sdk.Agent.Position.Z);

        Player player = new(Sdk.Agent.UniqueId, position, (float)Sdk.Agent.Orientation.Yaw, (float)Sdk.Agent.Orientation.Pitch);

        if (this._entityCreator.CreatePlayer(player) == true)
        {
            _hasCreatePlayer = true;
            _player = player;
            _playerAnimator = player.EntityObject.GetComponent<Animator>();
        }
        this._playerNowPosition = position;
        this._camera.transform.position = new Vector3(position.x, position.y + 1.62f, position.z);
    }
    private void Update()
    {
        if (Cursor.visible)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        InitializeBlock();
        InitializePlayer();

        if (Sdk.Agent is not null && this._player is not null)
        {
            Walk();
            Jump();
            Rotate();
            Attack();
            Use();
            UpdatePositionAndOrientation();
            UpdateHealthBar();
        }
    }
    public void QuitClient()
    {
        if (Sdk._client is not null)
        // Close websocket
        {
            Sdk._client.ClientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                                                "Closing",
                                                CancellationToken.None);
            Sdk._client.CloseReceiveMessageTask();
        }
        // Close the server process
        if (_fileLoaded.ServerProcess is not null)
        {
            _fileLoaded.ServerProcess.Kill();
            _fileLoaded.ServerProcess.Dispose();
        }
        // Close Timers
        Sdk.CloseTimers();
    }
    private void OnApplicationQuit()
    {
        QuitClient();
    }
}
