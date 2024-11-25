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
    private int potionCount;
    private WHS_Inventory inventory;

    private void Start()
    {
        potionButton.onClick.AddListener(UsePotion);
        StartCoroutine(FindPlayer());
    }

    private void Update()
    {
        if (inventory != null)
        {
            UpdateUI();

            if (Input.GetKeyDown(KeyCode.H))
            {
                UsePotion();
            }
        }
    }

    // 인벤토리 컴포넌트를 가진 플레이어 찾기
    IEnumerator FindPlayer()
    {
        while (inventory == null)
        {
            yield return new WaitForSeconds(1f);
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<WHS_Inventory>();
                Debug.Log("인벤토리 등록");
            }
        }
    }

    public void UpdateUI()
    {
        potionCount = inventory.GetItemCount(ItemType.HP);
        potionCountText.text = $"{potionCount}";
        potionButton.interactable = potionCount > 0;
    }

    private void UsePotion()
    {
        if (potionCount > 0)
        {
            Debug.Log("포션 사용");
            inventory.UseItem(ItemType.HP);
        }
        else
        {
            Debug.Log("보유중인 포션이 없습니다.");
        }
    }
}
