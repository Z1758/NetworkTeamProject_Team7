using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WHS_InventoryUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI potionCountText;
    [SerializeField] Button potionButton;
    [SerializeField] GameObject hpPotionPrefab;

    private WHS_Inventory inventory;

    private void Start()
    {
        inventory = GetComponent<WHS_Inventory>();
        potionButton.onClick.AddListener(UsePotion);
    }
    
    private void Update()
    {
        UpdateUI();

        if (Input.GetKeyDown(KeyCode.H))
        {
            UsePotion();
        }
    }

    private void UpdateUI()
    {
        int potionCount = inventory.GetItemCount(ItemType.HP);
        potionCountText.text = $"{potionCount}";
        potionButton.interactable = potionCount > 0;
    }

    private void UsePotion()
    {
        inventory.UseItem(ItemType.HP);
    }
}
