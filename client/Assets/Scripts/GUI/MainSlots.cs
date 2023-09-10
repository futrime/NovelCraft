using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class MainSlots : MonoBehaviour
{
    private GameObject _mainSlots;
    private List<Image> _mainSlotItems = new();
    public static int MainSlotsNum = Inventory.SlotNum / 4;
    public static int MaxItemId = EntityCreator.ItemArray.Length;
    private List<Sprite> _itemSprites = new();

    // Start is called before the first frame update
    void Start()
    {
        this._mainSlots = GameObject.Find("ObserverCanvas/MainSlots") ?? GameObject.Find("Canvas/SlotBackground");

        for (int i = 0; i < MainSlotsNum; i++)
        {
            GameObject newItemImageObject = new($"Slot{i}");
            Image image = newItemImageObject.AddComponent<Image>();
            ClearSlot(image);
            //RectTransform rectTransform = newItemImageObject.GetComponent<RectTransform>();
            //rectTransform.localPosition = new Vector3(rectTransform.sizeDelta.x / 8, 0, 0);
            newItemImageObject.transform.SetParent(_mainSlots.transform);
            newItemImageObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            this._mainSlotItems.Add(image);

            // Add Text
            GameObject newItemTextObject = new($"Text{i}");
            TMP_Text text = newItemTextObject.AddComponent<TextMeshPro>();
            text.fontStyle = FontStyles.Bold;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.fontSize = 150;
            text.text = "1";
            newItemTextObject.transform.SetParent(newItemImageObject.transform);
            newItemTextObject.transform.localScale = Vector3.one;
            newItemTextObject.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
        }
        // Initialize Item Sprites
        // Find Texture2D in  and 
        foreach (var itemName in EntityCreator.ItemArray)
        {

            Texture2D texture = Resources.Load<Texture2D>($"Items/{itemName}/{itemName}");
            if (texture == null)
            {
                string underscoreName = StringUtility.CamelCaseToUnderscore(itemName);
                texture = Resources.Load<Texture2D>($"Items/{itemName}/{underscoreName}");
            }
            if (texture == null)
            {
                // Block item
                texture = Resources.Load<Texture2D>($"Blocks/{itemName}/{itemName}");
            }

            if (texture == null)
            {
                _itemSprites.Add(null);
                continue;
            }

            _itemSprites.Add(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f)));
            Debug.Log(_itemSprites.Last());
        }
    }
    private void ClearSlot(Image image)
    {
        image.sprite = null;
        image.color = new Color(1, 1, 1, a: 0);
    }
    private void AddSlotItem(Image image, int itemId, int count)
    {
        if (itemId < 0 || itemId > MaxItemId || itemId >= _itemSprites.Count) return;

        image.sprite = this._itemSprites[itemId];
        image.color = new Color(1, 1, 1, 1);
    }
    public void ClearAllSlots()
    {
        foreach (var image in this._mainSlotItems)
        {
            this.ClearSlot(image);
        }
    }
    public void ChangeSlotItem(int slotNumber, int itemId, int count)
    {
        Image image = this._mainSlotItems[slotNumber];
        if (count > 0)
            this.AddSlotItem(image, itemId, count);
        else
        {
            this.ClearSlot(image);
        }
    }
    public void UpdateMainSlots(Inventory inventory)
    {
        this.ClearAllSlots();
        for (int i = 0; i < MainSlotsNum; i++)
        {
            int id = inventory.Slots[i].ItemId;
            int count = inventory.Slots[i].Count;
            if (id != 0)
                AddSlotItem(this._mainSlotItems[i], id, count);
        }
    }
}
