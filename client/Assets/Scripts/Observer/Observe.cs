using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class Observe : MonoBehaviour
{
    public enum ObserveState
    {
        Free,
        Spectator,
        FirstPerson
    };
    public float MoveSpeed;
    public float RotateSpeed;
    public static Vector3 InitialPosition = new(0, 128, 0);
    /// <summary>
    /// When observer press 'tab', it can set the GUI 
    /// </summary>
    public bool IsSettingGUI;

    public Canvas Canvas;

    public ObserveState NowObserveState;
    public Entity FollowingEntity;
    public const float SpectatorDistance = 5f;
    public const float FreeMaxPitch = 80;
    public int NowFollowerId;

    /// <summary>
    /// Main Slot bar
    /// </summary>
    private MainSlots _mainSlots;


    private GameObject _healthBar;
    private int _showHealth;
    private List<Image> _healthImageList = new();
    private Sprite _fullHealthImage;
    private Sprite _halfHealthImage;

    // Start is called before the first frame update
    private void Start()
    {
        MoveSpeed = 20f;
        RotateSpeed = 120f;
        IsSettingGUI = false;
        Cursor.visible = false; // Make the mouse ptr disappear
        Cursor.lockState = CursorLockMode.Confined;
        this.transform.position = InitialPosition;
        transform.eulerAngles = new();
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Canvas.enabled = (false);
        NowObserveState = ObserveState.Free;

        this._mainSlots = GameObject.Find("ObserverCanvas/MainSlots").GetComponent<MainSlots>();

        this._healthBar = GameObject.Find("ObserverCanvas/Health");
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
    }
    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move when "w a s d" is pressed
        if (Mathf.Abs(vertical) > 0.01)
        {
            Vector3 fowardVector = transform.forward;
            fowardVector = new Vector3(fowardVector.x, 0, fowardVector.z).normalized;
            // move forward
            transform.Translate(MoveSpeed * Time.deltaTime * vertical * fowardVector, Space.World);
        }
        if (Mathf.Abs(horizontal) > 0.01)
        {
            Vector3 rightVector = transform.right;
            rightVector = new Vector3(rightVector.x, 0, rightVector.z).normalized;
            // move aside 
            transform.Translate(MoveSpeed * Time.deltaTime * horizontal * rightVector, Space.World);
        }

        // Fly up if space is clicked
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * Vector3.up, Space.World);
        }
        // Fly down if left shift is clicked
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(MoveSpeed * Time.deltaTime * Vector3.down, Space.World);
        }
    }
    private void SpectatorFollow()
    {
        if (FollowingEntity != null && FollowingEntity.EntityObject != null)
        {
            Vector3 followingEntityCentralPosition = new Vector3(
                FollowingEntity.EntityObject.transform.position.x,
                FollowingEntity.EntityObject.transform.position.y + 1,
                FollowingEntity.EntityObject.transform.position.z)
                - SpectatorDistance * this.transform.forward;

            this.transform.position = Vector3.Lerp(this.transform.position, followingEntityCentralPosition, 0.4f);
        }
    }
    private void FirstPersonFollow()
    {
        if (FollowingEntity != null && FollowingEntity.EntityObject != null)
        {
            Vector3 followingEntityHeadPosition =
                new Vector3(FollowingEntity.Position.x, FollowingEntity.Position.y + 1.5f, FollowingEntity.Position.z)
                + 0.2f * FollowingEntity.EntityObject.transform.forward;

            Vector3 headPosition = 0.2f * new Vector3(
                Mathf.Sin(FollowingEntity.pitch) * Mathf.Sin(FollowingEntity.yaw),
                Mathf.Cos(FollowingEntity.yaw),
                Mathf.Cos(FollowingEntity.pitch) * Mathf.Sin(FollowingEntity.yaw)
            );
            this.transform.position = Vector3.Lerp(this.transform.position, followingEntityHeadPosition + headPosition, 0.4f);
        }
    }
    private void Rotate()
    {
        // Rotate when the IsSettingGUI is false 
        if (IsSettingGUI == false)
        {
            float MouseX = Input.GetAxis("Mouse X");
            float MouseY = Input.GetAxis("Mouse Y");
            if ((Mathf.Abs(MouseX) > 0.01 || Mathf.Abs(MouseY) > 0.01))
            {
                if (NowObserveState == ObserveState.Free)
                {
                    transform.Rotate(new Vector3(0, MouseX * RotateSpeed * Time.deltaTime, 0), Space.World);

                    float rotatedPitch = transform.eulerAngles.x - MouseY * RotateSpeed * Time.deltaTime * 1f;
                    if (Mathf.Abs(rotatedPitch > 180 ? 360 - rotatedPitch : rotatedPitch) < FreeMaxPitch)
                    {
                        transform.Rotate(new Vector3(-MouseY * RotateSpeed * Time.deltaTime * 1f, 0, 0));
                    }
                    else
                    {
                        if (transform.eulerAngles.x < 180)
                            transform.eulerAngles = new Vector3((FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
                        else
                            transform.eulerAngles = new Vector3(-(FreeMaxPitch - 1e-6f), transform.eulerAngles.y, 0);
                    }
                }
                else if (NowObserveState == ObserveState.Spectator)
                {
                    if (this.FollowingEntity != null && FollowingEntity.EntityObject != null)
                    {
                        transform.Rotate(new Vector3(0, MouseX * RotateSpeed * Time.deltaTime, 0), Space.World);
                        transform.Rotate(new Vector3(-MouseY * RotateSpeed * Time.deltaTime * 1f, 0, 0));
                        transform.position = new Vector3(
                            FollowingEntity.EntityObject.transform.position.x,
                            FollowingEntity.EntityObject.transform.position.y + 1,
                            FollowingEntity.EntityObject.transform.position.z)
                            - SpectatorDistance * this.transform.forward;
                    }
                }
            }
            if (NowObserveState == ObserveState.FirstPerson)
            {
                if (this.FollowingEntity != null && FollowingEntity.EntityObject != null)
                {
                    //this.transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles, new Vector3(FollowingEntity.pitch, FollowingEntity.yaw, 0), 0.5f);
                    this.transform.eulerAngles = new Vector3(FollowingEntity.pitch, FollowingEntity.yaw, 0);
                }
            }
        }
    }
    private void ListeningTab()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            IsSettingGUI = !IsSettingGUI;
    }
    private void ControlGUIDisplay()
    {
        if (IsSettingGUI)
        {
            Canvas.enabled = (true);
            Cursor.visible = true; // Make the mouse ptr appear
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Canvas.enabled = (false);
            Cursor.visible = false; // Make the mouse ptr disappear
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
    private void ObserveStateMachine()
    {
        if (!IsSettingGUI && Input.GetMouseButtonDown(1))
        {
            if (this.NowObserveState == ObserveState.Free)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));//射线
                if (Physics.Raycast(ray, out RaycastHit hit))//发射射线(射线，射线碰撞信息，射线长度，射线会检测的层级)
                {
                    foreach (Entity player in EntitySource.PlayerDict.Values)
                    {
                        if (player.EntityObject == hit.collider.gameObject)
                        {
                            this.FollowingEntity = player;
                            this.NowObserveState = ObserveState.Spectator;
                        }
                    }
                }
                if (EntitySource.PlayerDict.Values.Count > 0)
                {
                    this.FollowingEntity ??= EntitySource.PlayerDict.Values.ToList()[0];
                    this.NowObserveState = ObserveState.Spectator;
                }
            }
            else if (this.NowObserveState == ObserveState.Spectator)
            {
                this.NowObserveState = ObserveState.FirstPerson;
            }
            else if (this.NowObserveState == ObserveState.FirstPerson)
            {
                this.NowObserveState = ObserveState.Free;
                // Clear main slots
                this._mainSlots.ClearAllSlots();
            }
        }
    }
    private void JumpToAnotherPlayer()
    {
        float mouseScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if ((mouseScrollWheel) > 0.01f)
        {
            //System.Random rand = new System.Random();
            this.FollowingEntity = EntitySource.PlayerDict.Values.ToList()[NowFollowerId];
            NowFollowerId++;
            if (NowFollowerId >= EntitySource.PlayerDict.Values.Count) NowFollowerId -= EntitySource.PlayerDict.Values.Count;

            // Update main slots
            _mainSlots.UpdateMainSlots(((Player)this.FollowingEntity).Inventory);
            // Update health
            UpdateHealthBar((Player)this.FollowingEntity);
        }
        else if ((mouseScrollWheel) < -0.01f)
        {
            this.FollowingEntity = EntitySource.PlayerDict.Values.ToList()[NowFollowerId];
            NowFollowerId--;
            if (NowFollowerId < 0) NowFollowerId += EntitySource.PlayerDict.Values.Count;

            // Update main slots
            _mainSlots.UpdateMainSlots(((Player)this.FollowingEntity).Inventory);
            // Update health
            UpdateHealthBar((Player)this.FollowingEntity);
        }
    }
    public void UpdateHealthBar(Player player)
    {
        if ((Player)this.FollowingEntity != player || this.NowObserveState == ObserveState.Free) return;

        // Update health bar
        this._showHealth = (int)player.Health;
        Debug.Log(this._showHealth);
        for (int i = 0; i < this._healthImageList.Count; i++)
        {
            if (this._showHealth >= 2 * (i + 1))
            {
                _healthImageList[i].sprite = this._fullHealthImage;
                _healthImageList[i].color = new Color(1, 1, 1, 1);
            }
            else if (this._showHealth == 2 * i + 1)
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
    // Update is called once per frame
    private void Update()
    {
        if (NowObserveState == ObserveState.Free)
        {
            Move();
        }
        else if (NowObserveState == ObserveState.Spectator)
        {
            SpectatorFollow();
        }
        else if (NowObserveState == ObserveState.FirstPerson)
        {
            FirstPersonFollow();
        }

        Rotate();
        ListeningTab();
        ControlGUIDisplay();
        ObserveStateMachine();
        if (this.NowObserveState == ObserveState.Spectator ||
            this.NowObserveState == ObserveState.FirstPerson)
        {
            JumpToAnotherPlayer();
        }
    }
}
